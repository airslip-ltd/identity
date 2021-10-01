using Airslip.Common.Types;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Responses;
using JetBrains.Annotations;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class GetUserProfilePhotoQueryHandler : IRequestHandler<GetUserProfilePhotoQuery, IResponse>
    {
        private readonly IStorage<BlobStorageModel> _blobStorage;

        public GetUserProfilePhotoQueryHandler(IStorage<BlobStorageModel> blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<IResponse> Handle(GetUserProfilePhotoQuery command, CancellationToken cancellationToken)
        {
            (Stream? stream, string? contentType) =
                await _blobStorage.DownloadToStreamAsync(
                    $"{BlobStorageHelper.GetUserProfilePhotoPath(command.UserId)}");

            return contentType != null
                ? new StreamResponse(stream, contentType)
                : Success.Instance;
        }
    }
}