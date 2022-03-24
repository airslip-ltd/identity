using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Utilities.Extensions;
using Airslip.Email.Client;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IUserManagerService _userManagerService;
    private readonly PublicApiSetting _publicApiSettings;
    private readonly ILogger _logger;
    private readonly IIdentityContext _context;

    public EmailNotificationService(
        IIdentityContext context,
        IUserManagerService userManagerService,
        IEmailSender emailSender,
        IOptions<PublicApiSettings> publicApiOptions,
        ILogger logger)
    {
        _userManagerService = userManagerService; 
        _emailSender = emailSender;
        _logger = logger;
        _context = context;
        _publicApiSettings = publicApiOptions.Value.GetSettingByName("UI");
    }

    public Task<IResponse> SendNewUserEmail(string email, string firstName, string relativeEndpoint)
    {
        return _sendEmail(email, firstName, relativeEndpoint, _emailSender.NewUser);
    }
    
    public Task<IResponse> SendPasswordReset(string email, string relativeEndpoint)
    {
        return _sendEmail(email, null, relativeEndpoint, _emailSender.ForgotPassword);
    }
    
    private async Task<IResponse> _sendEmail(string email, string? firstName, string relativeEndpoint, 
        Func<string, string, string, Task<EmailOutcome>> emailFunction)
    {
        ApplicationUser? applicationUser = await _userManagerService.FindByEmail(email);
        
        if (applicationUser is null)
            return Success.Instance;

        if (firstName == null)
        {
            User? user = await _context.GetByEmail(email);
        
            if (user is null)
                return Success.Instance;

            firstName = user.FirstName;
        }
        
        string token = await _userManagerService.GeneratePasswordResetToken(applicationUser);

        string resetPasswordUrl = PasswordEmailConstants.GetPasswordResetUrl(
            _publicApiSettings.BaseUri, 
            relativeEndpoint,
            token, 
            applicationUser.Email);

        EmailOutcome outcome = await emailFunction(applicationUser.Email, 
            firstName ?? applicationUser.Email, resetPasswordUrl);

        if(!outcome.Success)
            _logger.Error("Error sending email due to {Error}", outcome.ErrorReason);

        return Success.Instance;
    }
    
}