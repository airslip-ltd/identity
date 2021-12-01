using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Failures;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{

    public class RepositoryControllerBase<TEntity, TModel> : ApiControllerBase 
        where TEntity : class, IEntity 
        where TModel : class, IModel
    {
        protected readonly IRepository<TEntity, TModel> Repository;

        public RepositoryControllerBase(
            ITokenDecodeService<UserToken> tokenDecodeService, 
            IOptions<PublicApiSettings> publicApiOptions,
            IRepository<TEntity, TModel> repository,
            ILogger logger) : base(tokenDecodeService, publicApiOptions, logger)
        {
            Repository = repository;
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            IResponse response = await Repository.Get(id);

            return CommonResponseHandler<SuccessfulActionResultModel<TModel>>(response);
        }
        
        [HttpPost("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] TModel model)
        {
            IResponse response = await Repository.Update(id, model);

            return CommonResponseHandler<SuccessfulActionResultModel<TModel>>(response);
        }
        
        [HttpPut("")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] TModel model)
        {
            IResponse response = await Repository.Add(model);

            return CommonResponseHandler<SuccessfulActionResultModel<TModel>>(response);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            IResponse response = await Repository.Delete(id);

            return CommonResponseHandler<SuccessfulActionResultModel<TModel>>(response);
        }
    }

    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    [Route("v{version:apiVersion}/user")]
    public class UserController : RepositoryControllerBase<User, UserModel>
    {
        public UserController(ITokenDecodeService<UserToken> tokenDecodeService, 
            IOptions<PublicApiSettings> publicApiOptions, IRepository<User, UserModel> repository, ILogger logger) : 
            base(tokenDecodeService, publicApiOptions, repository, logger)
        {
            
        }
        
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            IResponse response = await Repository.Get(Token.UserId);

            return CommonResponseHandler<SuccessfulActionResultModel<UserModel>>(response);
        }
    }
}