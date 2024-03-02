using System.Security.Claims;

namespace CardService.API.Identity
{
    public class JwtUser : ClaimsPrincipal
    {
        public JwtUser(ClaimsPrincipal principal) : base(principal) { }

        private string? GetClaimValueLike(string key)
        {
            var identity = Identity as ClaimsIdentity;
            if (identity == null)
            {
                return default;
            }

            var claim = identity.Claims.FirstOrDefault(c => c.Type.Contains(key));
            return claim?.Value;
        }

        public string UserId
        {
            get
            {
                return GetClaimValueLike(ClaimTypes.Name);
            }
        }

        public string? Email
        {
            get
            {
                return GetClaimValueLike(ClaimTypes.Email);
            }
        }

        public string? Role
        {
            get
            {
                return GetClaimValueLike(ClaimTypes.Role);
            }
        }
    }
}
