using DataAccessLayer.Models;
using ModelAccessLayer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DALAccounts
    {
        public LoginUser UserLogin(Login loginRequest, out MainResponse response)
        {
            try
            {
                LoginUser loginUser = null;
                response = new MainResponse();
                using (var db = new TestUserDbEntities())
                {
                    var profile = db.EmployeeLogins.Where(x => x.Email == loginRequest.Email).FirstOrDefault();
                    if (profile != null)
                    {
                        string sha256 = SHA256(loginRequest.Password);
                        if (sha256 == profile.Password)
                        {
                            loginUser = new LoginUser()
                            {
                                Email = profile.Email,
                                Token = CreateToken(loginRequest.Email, loginRequest.Password, profile.Id),
                                Message = "User Login Successfully."
                            };
                            response.Code = 200;
                            response.Status = "Sucess";
                            response.Data = profile;
                        }
                    }
                }
                return loginUser;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public MainResponse UserRegister(Register registerRequest)
        {
            try
            {
                var response = new MainResponse();
                using (var db = new TestUserDbEntities())
                {
                    var profile = db.EmployeeLogins.Where(x => x.Email == registerRequest.Email).FirstOrDefault();
                    if (profile == null)
                    {
                        EmployeeLogin insertProfile = new EmployeeLogin()
                        {
                            Email = registerRequest.Email,
                            Password = SHA256(registerRequest.Password),
                            EmployeeName = registerRequest.EmployeeName,
                            City = registerRequest.City,
                            Department = registerRequest.Department
                        };
                        db.EmployeeLogins.Add(insertProfile);
                        db.SaveChanges();
                       
                        int id=insertProfile.Id;
                        var UserDetails = db.EmployeeLogins.Where(x => x.Id == id).FirstOrDefault();
                        response.Code = 200;
                        response.Status = "Sucess";
                        response.Status = "User Register Successfully.";
                        response.Data = UserDetails;
                    }
                    else
                    {
                        response.Code = 400;
                        response.Status = "BadRequest";
                        response.Message = "Email Already Registered.";
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string CreateToken(string email, string password, int id)
        {
            DateTime time = DateTime.UtcNow;
            DateTime expire = DateTime.UtcNow.AddYears(7);
            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new Claim[]
           {
            new Claim(ClaimTypes.Name, email+'|'+password+'|'+ id)
           });
            string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var now = DateTime.UtcNow;
            var mySecurityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(key)); ;
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(mySecurityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);
            var token = tokenHandler.CreateJwtSecurityToken(
                 issuer: "http://localhost:51422",
                 audience: "http://localhost:51422",
                 subject: claimsIdentity,
                 expires: expire,
                 signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        private string SHA256(string password)
        {
            SHA256Managed crypt = new SHA256Managed();
            StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password + "APP"), 0, Encoding.UTF8.GetByteCount(password + "APP"));
            foreach (byte bytes in crypto)
            {
                hash.Append(bytes.ToString("x2"));
            }
            return hash.ToString();
        }

        public bool Validate(string email, string password)
        {
            try
            {
                bool IsValid = false;
                using (var db = new TestUserDbEntities())
                {
                    string sha256 = SHA256(password);
                    IsValid = db.EmployeeLogins.Any(x => x.Email == email && x.Password == sha256);
                    return IsValid;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
