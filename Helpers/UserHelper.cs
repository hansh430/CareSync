using CareSync.Models;
using System.Security.Claims;

namespace CareSync.Helpers
{
    public class UserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            return int.Parse(
                user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        }
    }
}