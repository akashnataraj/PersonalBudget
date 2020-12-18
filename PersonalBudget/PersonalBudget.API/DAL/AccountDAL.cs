using System;
using PersonalBudget.API.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using PersonalBudget.API.DTO;
using Microsoft.Extensions.Configuration;
using PersonalBudget.API.Common;

namespace PersonalBudget.API.DAL
{
    public class AccountDAL
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<Account> _account;
        private readonly Utilities _util;
        public AccountDAL(IPBDatabaseSettings settings, IConfiguration configuration, Utilities utilities)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _account = database.GetCollection<Account>(settings.AccountCollectionName);
            _config = configuration;
            _util = utilities;
        }

        public AccountDTO RegisterUser(AccountDTO register)
        {
            try
            {
                if (_account.Find(x => x.Username.Equals(register.Username)).CountDocuments() > 0)
                {
                    register.Code = _config.GetValue<int>("ResponseStatus:DuplicateUsername:Code");
                    register.Description = _config.GetValue<string>("ResponseStatus:DuplicateUsername:Message");
                }
                else
                {
                    string[] encryptedPassword = _util.HashPassword(register.Password);
                    Account account = new Account()
                    {
                        FirstName = register.FirstName,
                        LastName = register.LastName,
                        Username = register.Username,
                        Password = encryptedPassword[0],
                        Salt = encryptedPassword[1],
                        CreatedDate = DateTime.Now,
                        RefreshToken = string.Empty
                    };

                    _account.InsertOne(account);
                    register.Code = _config.GetValue<int>("ResponseStatus:Success:Code");
                }
            }
            catch (Exception ex)
            {
                register.Code = _config.GetValue<int>("ResponseStatus:Error:Code");
                register.Description = ex.Message;
            }
            return register;
        }
        public AccountDTO Login(string username, string password, string refreshToken)
        {
            AccountDTO loginResponse = new AccountDTO();
            try
            {
                var user = _account.Find(x => x.Username.Equals(username)).FirstOrDefault();
                if (user == null)
                {
                    loginResponse.Code = _config.GetValue<int>("ResponseStatus:UsernameNotFound:Code");
                    loginResponse.Description = _config.GetValue<string>("ResponseStatus:UsernameNotFound:Message");
                }
                else
                {
                    if((!String.IsNullOrEmpty(refreshToken) && String.IsNullOrEmpty(password) && refreshToken == user.RefreshToken)
                        || (_util.HashPassword(password, user.Salt)[0] == user.Password))
                    {
                        loginResponse.UserID = user.Id;
                        loginResponse.Username = user.Username;
                        loginResponse.FirstName = user.FirstName;
                        loginResponse.LastName = user.LastName;
                        loginResponse.Code = _config.GetValue<int>("ResponseStatus:Success:Code");
                        loginResponse.ExpiresIn = _config.GetValue<int>("Jwt:ExpiresInSeconds");

                        user.RefreshToken = Guid.NewGuid().ToString();
                        _account.ReplaceOne(x => x.Id == user.Id, user);

                        loginResponse.RefreshToken = user.RefreshToken;
                    }
                    else
                    {
                        loginResponse.Code = _config.GetValue<int>("ResponseStatus:InvalidLogin:Code");
                        loginResponse.Description = _config.GetValue<string>("ResponseStatus:InvalidLogin:Message");
                    }
                }
            }
            catch (Exception ex)
            {
                loginResponse.Code = _config.GetValue<int>("ResponseStatus:Error:Code");
                loginResponse.Description = ex.Message;
            }
            return loginResponse;
        }

    }
}
