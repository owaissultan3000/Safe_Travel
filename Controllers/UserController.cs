using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using carpool.Models;
using carpool.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace carpool.Controllers
{
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Route("[controller]")]
    public class UserController:ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;  

        public UserController(IUserService userService,IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost]  
        [EnableCors("CorsPolicy")]
        [Route("UserLogin")]  
        public async Task<IActionResult> Login([FromBody] UserLogin model)  
        {  
            IActionResult response = Unauthorized();
            
            User user = await _userService.GetUser(model.UserEmail);  
            
            if (user != null && BCrypt.Net.BCrypt.Verify(model.UserPassword, user.Passwords) == true)  
            {  
                    var tokenString = GenerateJSONWebToken(user);
                     response = Ok(new { tokenString });    
            }  
             return response;  
        }

        private string GenerateJSONWebToken(User userInfo)
             {
                 var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                 var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                 var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                   _configuration["Jwt:Issuer"],
                   null,
                   expires: DateTime.Now.AddMinutes(120),
                   signingCredentials: credentials);
                 return new JwtSecurityTokenHandler().WriteToken(token);
             } 
    

        [HttpPost("UserRegistration")]
        [EnableCors("CorsPolicy")]
        
        public async Task<IActionResult> CreateUser([FromBody]UserModel user)
        {
            if (ModelState.IsValid)
            {
                 try
                 {
                
                    var tempuser = await _userService.CreateUser(user);
                    if (tempuser != null)
                    {
                        return Ok(tempuser);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception)
                {

                    return BadRequest();
                }


            }
            else return BadRequest();
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string email)
        {
             string result = "";

            if (email == null)
            {
                return BadRequest();
            }

            try
            {
                result = await _userService.DeleteUser(email);
                if (result == "" || result == null)
                {
                    return NotFound();
                }
                return Ok(email+" Deleted Successfully");
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [Authorize]
        
        [HttpGet("AvailableRides")]
        public async Task<IActionResult> BookRide()
        {
             try
            {
                var rides = await _userService.ViewAvailableRides();
                if (rides == null)
                {
                    return NotFound();
                }

                return Ok(rides);
            }
            catch (Exception )
            {
                return BadRequest();
            }

        }
        [Authorize]
        [HttpPost("BookRide")]
        public async Task<IActionResult> BookRide([FromBody]BookRide ride)
        {
            try
            {
                var bookRide = await _userService.BookRide(ride);
                if (bookRide == null)
                {
                    return NotFound();
                }
                return Ok(bookRide);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        // [HttpPut("UpdateUser")]
        // public async Task<IActionResult> UpdateUser([FromBody]UserModel user)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             await _userService.UpdateUser(user);

        //             return Ok();
        //         }
        //         catch (Exception ex)
        //         {
        //             if (ex.GetType().FullName == "Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException")
        //             {
        //                 return NotFound();
        //             }

        //             return BadRequest();
        //         }
        //     }

        //     return BadRequest();
        // }
        
    }
}