using Microsoft.AspNetCore.Mvc;
using quotes_app.Helpers;
using quotes_app.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace quotes_app.Controllers;

    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly QuotesDbContext Context;
        private readonly JwtService JWTService;

        public AuthController(QuotesDbContext context, JwtService jwtService)
        {
            Context = context;
            JWTService = jwtService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            if (user == null)
                return BadRequest("User object is null.");

            var existingUser = await Context.Users.FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);
            if (existingUser != null)
                return BadRequest("Username or email already exists.");

            // Hashiranje sifre pre cuvanja u bazu radi sigurnosti
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            Context.Users.Add(user);
            await Context.SaveChangesAsync();

            // Generisanje JWT tokena
            var token = JWTService.Generate(user.Id);

            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await Context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                return Unauthorized("Invalid username or password.");

            // Generisanje JWT tokena
            var token = JWTService.Generate(user.Id);

            return Ok(new { Token = token });
        }

    }
