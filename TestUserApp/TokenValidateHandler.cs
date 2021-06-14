using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TestUserApp.Controllers;
using DataAccessLayer;

namespace TestUserApp
{
    internal class TokenValidateHandler : DelegatingHandler
    {
        private static bool IsValidRetriveToken(HttpRequestMessage httpRequestMessage, out string token)
        {
            token = null;
            if (!httpRequestMessage.Headers.TryGetValues("Autorization", out IEnumerable<string> authHeader) || authHeader.Count() > 1)
            {
                return false;
            }
            var bearerToken = authHeader.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            if (!IsValidRetriveToken(requestMessage, out string token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return base.SendAsync(requestMessage, cancellationToken);
            }

            try
            {
                string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
                var now = DateTime.UtcNow;
                var mySecurityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(key));
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                TokenValidationParameters tokenValidattionPrameters = new TokenValidationParameters()
                {
                    ValidAudience = "http://localhost:51422",
                    ValidIssuer = "http://localhost:51422",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey=true,
                    LifetimeValidator=this.LifeTimeValidator,
                    IssuerSigningKey=mySecurityKey
                };

                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, tokenValidattionPrameters, out SecurityToken securityToken);
                HttpContext.Current.User=tokenHandler.ValidateToken(token, tokenValidattionPrameters, out securityToken);

                string[] credentials = HttpContext.Current.User.Identity.Name.Split('|');
                string email = credentials[0];
                string password = credentials[1];
                string id = credentials[2];
                AccountController.id = id;

                if ((new DALAccounts()).Validate(email,password))
                {
                    return base.SendAsync(requestMessage, cancellationToken);
                }
                else
                {
                    statusCode = HttpStatusCode.Unauthorized;
                }
            }
            catch
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }

        private bool LifeTimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if(expires!=null)
            {
                if (DateTime.UtcNow < expires)
                    return true;
            }
            return false;
        }
    }
}