using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Failures;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Utilities.Extensions;
using Airslip.Email.Client;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Microsoft.Extensions.Options;
using Serilog;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IUserManagerService _userManagerService;
    private readonly PublicApiSetting _publicApiSettings;
    private readonly ILogger _logger;

    public EmailNotificationService(
        IUserManagerService userManagerService,
        IEmailSender emailSender,
        IOptions<PublicApiSettings> publicApiOptions,
        ILogger logger)
    {
        _userManagerService = userManagerService;
        _emailSender = emailSender;
        _logger = logger;
        _publicApiSettings = publicApiOptions.Value.GetSettingByName("UI");
    }

    public Task<IResponse> SendNewUserEmail(string email, string relativeEndpoint)
    {
        return _sendEmail(email, relativeEndpoint, PasswordEmailConstants.NewUserSubject,
            PasswordEmailConstants.GetNewUserPlainTextContent());
       
    }
    
    public Task<IResponse> SendPasswordReset(string email, string relativeEndpoint)
    {
        return _sendEmail(email, relativeEndpoint, PasswordEmailConstants.ForgotSubject,
            PasswordEmailConstants.GetForgotPlainTextContent());
    }

    private async Task<IResponse> _sendEmail(string email, string relativeEndpoint, string subject, string body)
    {
        ApplicationUser? user = await _userManagerService.FindByEmail(email);
            
        if (user is null)
            return Success.Instance;

        string token = await _userManagerService.GeneratePasswordResetToken(user);

        string resetPasswordUrl = PasswordEmailConstants.GetPasswordResetUrl(
            _publicApiSettings.BaseUri, 
            relativeEndpoint,
            token, 
            user.Email);

        body = body.Replace("{password_url}", resetPasswordUrl);

        EmailOutcome outcome = await _emailSender.SendEmail(
            new [] {new EmailAddressRecipient(user.Email, user.UserName)},
            subject,
            body,
            string.Empty);

        if(!outcome.Success)
            _logger.Error("Error sending email due to {Error}", outcome.ErrorReason);

        return Success.Instance;
    }
}