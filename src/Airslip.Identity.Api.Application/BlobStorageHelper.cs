namespace Airslip.Identity.Api.Application
{
    public static class BlobStorageHelper
    {
        public static string GetUserProfilePhotoPath(string userId)
        {
            return $"user/{userId}";
        }
    }
}