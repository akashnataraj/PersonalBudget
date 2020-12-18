using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalBudget.API.Common;
using PersonalBudget.API.DAL;
using PersonalBudget.API.DTO;

namespace PersonalBudget.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly Utilities _util;
        private readonly AccountDAL _account;
        private IConfiguration _config { get; }
        public AccountController(IConfiguration config, AccountDAL account, Utilities utilities)
        {
            _config = config;
            _account = account;
            _util = utilities;
        }

        [Route("api/Account/Login")]
        [HttpPost]
        public ActionResult<AccountDTO> Login([FromBody]AccountDTO login)
        {
            AccountDTO loginResponse = new AccountDTO();
            try
            {
                loginResponse = _account.Login(login.Username, login.Password, login.RefreshToken);

                if (loginResponse.Code == _config.GetValue<Int32>("ResponseStatus:Success:Code"))
                    loginResponse.Token = _util.GenerateToken(loginResponse);

                return (loginResponse.Code != _config.GetValue<Int32>("ResponseStatus:Error:Code"))
                       ? (loginResponse.Code == _config.GetValue<Int32>("ResponseStatus:InvalidLogin:Code")
                         ? StatusCode(StatusCodes.Status401Unauthorized, loginResponse)
                         : Ok(loginResponse))
                       : StatusCode(StatusCodes.Status500InternalServerError, loginResponse.Description);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("api/Account/Register")]
        [HttpPost]
        public ActionResult<AccountDTO> Register([FromBody]AccountDTO register)
        {
            try
            {
                AccountDTO regResponse = _account.RegisterUser(register);
                return (regResponse.Code != _config.GetValue<Int32>("ResponseStatus:Error:Code"))
                       ? StatusCode(StatusCodes.Status201Created, regResponse) : StatusCode(StatusCodes.Status500InternalServerError, regResponse.Description);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}