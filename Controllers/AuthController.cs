using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using intro2NET.API.Data;
using intro2NET.API.DTOs;
using intro2NET.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace intro2NET.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;

        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (User2RegisterDTO user2RegisterDTO)
        {
            user2RegisterDTO.Username = user2RegisterDTO.Username.ToLower();

            if(await _repository.UserExistsCheck(user2RegisterDTO.Username))
            {
                return BadRequest("Username Already Exists!");
            }

            User userToBeCreated = new User
            {
                Username = user2RegisterDTO.Username
            };

            User createdUser = await _repository.Register(userToBeCreated, user2RegisterDTO.Password);

            return StatusCode(201);
            //return CreatedAtRoute();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User2LoginDTO user2LoginDTO)
        {
            User userFromDB = await _repository.Login(user2LoginDTO.Username.ToLower(), user2LoginDTO.Password);

            if (userFromDB == null) return Unauthorized();

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromDB.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromDB.Username)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescription);

            return Ok( new {token = tokenHandler.WriteToken(token)} );
        }
    }
}
