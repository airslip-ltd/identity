using Airslip.Common.Contracts;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class UpdateUserProfilePhotoCommandHandler : IRequestHandler<UpdateUserProfilePhotoCommand, IResponse>
    {
        private readonly IStorage<BlobStorageModel> _blobStorage;

        public UpdateUserProfilePhotoCommandHandler(IStorage<BlobStorageModel> blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<IResponse> Handle(UpdateUserProfilePhotoCommand command, CancellationToken cancellationToken)
        {
            IFormFile photo = command.Photo!;
            
            BlobStorageModel blobStorageModel = new(
                photo.OpenReadStream(),
                $"{BlobStorageHelper.GetUserProfilePhotoPath(command.UserId)}",
                photo.ContentType);
            
            await _blobStorage.SaveFileAsync(blobStorageModel);

            return Success.Instance;
        }
    }
}