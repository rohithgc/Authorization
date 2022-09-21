using AuthorizationMicroservice.Models;
using AuthorizationMicroservice.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace AuthorizationMicroservice.Repository
{
    public class UserRepo : IUserRepo
    {

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(UserRepo));
        private readonly AuthContext _context;
        private readonly IConfiguration _configuration;


        public UserRepo(AuthContext context,IConfiguration configuration)
        {
            _log4net.Info("User Repository Constructor Initiated");

            _context = context;
            _configuration = configuration;
        }
        public User AuthenticateUser(User login)
        {
            _log4net.Info("Authentication Started");
           
            User user = _context.Users.FirstOrDefault(z => z.EmployeeId == login.EmployeeId && z.Password == login.Password);
            return user;
        }

        public string GenerateJSONWebToken(User userInfo)
        {
            _log4net.Info("Token Generation Started");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
