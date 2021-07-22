namespace Airslip.BankTransactions.Domain
{
    public static class BlobStorageHelper
    {
        public static string GetMerchantLogoPath(string fileName)
        {
            return $"merchants/logo/{fileName.Replace(" ", "_")}";
        }
        
        public static string GetMerchantCategoryIconPath(string fileName)
        {
            return $"merchant-category-icons/{fileName.Replace(" ", "_")}";
        }

        public static string GetUserProfilePhotoPath(string userId)
        {
            return $"user/{userId}";
        }
    }
}