using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prototype_2_login_backend.Context;
using prototype_2_login_backend.Helpers;
using prototype_2_login_backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace prototype_2_login_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInUser([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(x => x.Email == userObj.Email);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            user.Token = CreateJwt(user);

            return Ok(new {
                Token = user.Token,
                Message = "Login sucess!"
            });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpUser([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);

            userObj.Role = "User";
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new { Message = "User signed up!" });
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("verylongsecretkey");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name,$"{user.Name}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
