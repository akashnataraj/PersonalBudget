using Microsoft.Extensions.Configuration;
using System;
using MongoDB.Driver;
using PersonalBudget.API.Models;
using System.Linq;
using PersonalBudget.API.DTO;
using System.Collections.Generic;

namespace PersonalBudget.API.DAL
{
    public class BudgetConfigDAL
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<BudgetConfig> _budget;

        public BudgetConfigDAL(IPBDatabaseSettings settings, IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _budget = database.GetCollection<BudgetConfig>(settings.BudgetConfigCollectionName);
            _config = configuration;
        }
        public BudgetDTO CreateBudget(BudgetDTO budget)
        {
            try
            {
                DateTime now = budget.CreatedDate;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                if (_budget.Find(x => x.UserId == budget.UserId && x.CreatedDate >= startDate & x.CreatedDate < endDate
                && x.Title == budget.Title).CountDocuments() > 0)
                {
                    budget.Code = _config.GetValue<int>("ResponseStatus:DuplicateBudgetTitle:Code");
                    budget.Description = _config.GetValue<string>("ResponseStatus:DuplicateBudgetTitle:Message");
                }
                else
                {
                    BudgetConfig bconf = new BudgetConfig()
                    {
                        Title = budget.Title,
                        UserId = budget.UserId,
                        Expense = budget.Expense,
                        Budget = budget.Budget,
                        CreatedDate = now
                    };
                    _budget.InsertOne(bconf);
                    budget.Code = _config.GetValue<int>("ResponseStatus:Success:Code");
                }
            }
            catch (Exception ex)
            {
                budget.Code = _config.GetValue<int>("ResponseStatus:Error:Code");
                budget.Description = ex.Message;
            }
            return budget;
        }
        public BudgetResponse GetBudget(string userId, DateTime date)
        {
            BudgetResponse budget = new BudgetResponse();
            try
            {
                budget.BudgetList = (from b in _budget.AsQueryable<BudgetConfig>()
                                     where b.UserId == userId
                                     select new BudgetDTO
                                     {
                                         BudgetId = b.Id,
                                         Title = b.Title,
                                         Expense = b.Expense,
                                         Budget = b.Budget,
                                         CreatedDate = b.CreatedDate
                                     }).ToList();
                if (budget.BudgetList != null)
                    budget.BudgetList = budget.BudgetList.Where(x => x.CreatedDate.Month == date.Month 
                                                             && x.CreatedDate.Year == date.Year).ToList();
                budget.Code = _config.GetValue<int>("ResponseStatus:Success:Code");
            }
            catch (Exception ex)
            {
                budget.Code = _config.GetValue<int>("ResponseStatus:Error:Code");
                budget.Description = ex.Message;
            }
            return budget;
        }
        public BudgetDTO UpdateBudget(BudgetDTO budget)
        {
            try
            {
                if (_budget.Find<BudgetConfig>(x => x.Id == budget.BudgetId).FirstOrDefault() != null)
                {
                    DateTime now = _budget.Find<BudgetConfig>(x => x.Id == budget.BudgetId).FirstOrDefault().CreatedDate;
                    var startDate = new DateTime(now.Year, now.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    if (_budget.Find(x => x.UserId == budget.UserId && x.CreatedDate >= startDate & x.CreatedDate < endDate
                    && x.Title == budget.Title && x.Id != budget.BudgetId).CountDocuments() > 0)
                    {
                        budget.Code = _config.GetValue<int>("ResponseStatus:DuplicateBudgetTitle:Code");
                        budget.Description = _config.GetValue<string>("ResponseStatus:DuplicateBudgetTitle:Message");
                    }
                    else
                    {
                        BudgetConfig bconf = new BudgetConfig()
                        {
                            Id = budget.BudgetId,
                            UserId = budget.UserId,
                            Title = budget.Title,
                            Expense = budget.Expense,
                            Budget = budget.Budget,
                            CreatedDate = now
                        };

                        _budget.ReplaceOne(x => x.Id == budget.BudgetId, bconf);
                        budget.Code = _config.GetValue<int>("ResponseStatus:Success:Code");
                    }
                }
            }
            catch (Exception ex)
            {
                budget.Code = _config.GetValue<int>("ResponseStatus:Error:Code");
                budget.Description = ex.Message;
            }
            return budget;
        }
        public BudgetDTO DeleteBudget(string budgetId)
        {
            BudgetDTO budget = new BudgetDTO();
            try
            {
                _budget.DeleteOne(x => x.Id == budgetId);
                budget.Code = _config.GetValue<int>("ResponseStatus:Success:Code");
            }
            catch (Exception ex)
            {
                budget.Code = _config.GetValue<int>("ResponseStatus:Error:Code");
                budget.Description = ex.Message;
            }
            return budget;
        }
    }
}
