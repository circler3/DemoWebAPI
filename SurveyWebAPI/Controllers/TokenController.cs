using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SurveyWebAPI.DataContext;
using SurveyWebAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SurveyWebAPI.Controllers
{
    [EnableCors("AllowAll")]
    [Consumes("application/json", "application/x-www-form-urlencoded")]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IConfiguration _config;

        public TokenController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Post(UserInfo login)
        {
            ActionResult response = BadRequest("登录失败，请检查用户名和密码");
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { access_token = tokenString, role = user.Role });
            }

            return response;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            IActionResult response = Unauthorized();
            return response;
        }

        private string GenerateJSONWebToken(UserInfo userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userInfo.Role),
            };

            var token = new JwtSecurityToken(null,
                null,
                claims,
                expires: DateTime.Now.AddMinutes(1200),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserInfo? AuthenticateUser(UserInfo login)
        {
            UserInfo? user = null;
            if (string.IsNullOrWhiteSpace(login.Password)) return user;

            using (var context = new ManageDataContext())
            {
                var result = context.UserInfos.Where(w => w.UserName.ToLower() == login.UserName.ToLower()).FirstOrDefault();
                if (result != null)
                    if (PasswordStorage.VerifyPassword(login.Password, result.PasswordHash!)) user = result;
            }

            return user;
        }
    }
}