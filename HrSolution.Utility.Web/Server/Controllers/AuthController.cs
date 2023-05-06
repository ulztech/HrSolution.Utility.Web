using HrSolution.Common.Shared.Jwt;
using HrSolution.Utility.Web.Server.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.Blazor.HeatMap;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HrSolution.Utility.Web.Server.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    { 
        //[HttpPost]
        //[Route("api/auth/register")]
        //public async Task<LoginResult> Post([FromBody] RegModel reg)
        //{
        //    if (reg.password != reg.confirmpwd)
        //        return new LoginResult { message = "Password and confirm password do not match.", success = false };
        //    User newuser = await userdb.AddUser(reg.email, reg.password);
        //    if (newuser != null)
        //        return new LoginResult { message = "Registration successful.", jwtBearer = CreateJWT(newuser), email = reg.email, success = true };
        //    return new LoginResult { message = "User already exists.", success = false };
        //}

    

        [HttpPost]
        [Route("auth/login/{username}/{password}")]
        public async Task<LoginResult> Post(string username, string password)
        { 
            if (username == "test" && password == "test")
            {
                var user = new UserModel()
                {
                    Email = "ulztech@gmail.com",
                    FirstName = "Ulysses",
                    LastName = "Consador",
                    Id = 1000001,
                    Username = "test"
                };

                user.Token = CreateJWT(user);

                return new LoginResult { message = "Login successful.", user = user, success = true };
            }
 
            return new LoginResult { message = "User/password not found.", success = false };
        }


        [HttpGet]
        [Authorize]
        [Route("users")]
        public IEnumerable<UserModel> Users()
        {
            var userList = new List<UserModel>()
            {
                new UserModel()
                {
                    Email = "ulztech@gmail.com",
                    FirstName = "Ulysses",
                    LastName = "Consador",
                    Id = 1000001,
                    Username = "test"
                }
            };
              
            return userList;
        }


        private string CreateJWT(UserModel user)
        {
            var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(LocalConfig.JwtSecretKey));
            var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

            var claims = new[] // NOTE: could also use List<Claim> here
            {
                    new Claim(ClaimTypes.Name, user.Username), // NOTE: this will be the "User.Identity.Name" value
				    new Claim(JwtRegisteredClaimNames.Sub, user.LastName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
                };

            var token = new JwtSecurityToken(issuer: LocalConfig.JwtIssuer, audience: LocalConfig.JwtAudience, claims: claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}