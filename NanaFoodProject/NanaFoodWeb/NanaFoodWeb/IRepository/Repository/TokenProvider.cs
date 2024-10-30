using NanaFoodWeb.Utility;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NanaFoodWeb.IRepository.Repository
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void ClearCartCount()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete("CartCount");
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.TokenCookie);
        }

        public string? GetCartCount()
        {
            string? cartcount = null;

            bool? hasCart = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.TokenCookie, out cartcount);

            return hasCart is true ? cartcount : "0";
        }

        public string? GetToken()
        {
            string? token = null;

            bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.TokenCookie, out token);

            return hasToken is true ? token : null;
        }

        public string? ReadToken(string type, string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var value = jwt.Claims.FirstOrDefault(u => u.Type == type)?.Value;

            return value?.ToString() ?? string.Empty;
        }

        public void SetCartCount(string cartCount)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append("CartCount", cartCount);
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.TokenCookie, token);
        }


    }
}
