using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts
{
    public interface IUserManagerService
    {
        Task<bool> TryToLogin(string email, string password);
    }
}