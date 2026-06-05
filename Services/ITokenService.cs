using CareSync.Models;

namespace CareSync.Services
{
    public interface ITokenService
    {
        public string CreateToken(User user);
    }
}
