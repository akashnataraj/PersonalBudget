using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PersonalBudget.API.DAL;
using PersonalBudget.API.DTO;

namespace PersonalBudget.API.Controllers
{
    [Authorize]
    [ApiController]
    public class BudgetController : Controller
    {
        private readonly BudgetConfigDAL _budget;
        private IConfiguration Configuration { get; }

        public BudgetController(IConfiguration config, BudgetConfigDAL budget)
        {
            Configuration = config;
            _budget = budget;
        }

        [Route("api/Budget/Create")]
        [HttpPost]
        public ActionResult<BudgetDTO> CreateBudget([FromBody]BudgetDTO budget)
        {
            try
            {
                BudgetDTO budgetRes = _budget.CreateBudget(budget);
                return (budgetRes.Code != Configuration.GetValue<Int32>("ResponseStatus:Error:Code"))
                       ? StatusCode(StatusCodes.Status201Created, budgetRes) : StatusCode(StatusCodes.Status500InternalServerError, budgetRes.Description);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("api/Budget/Get")]
        [HttpGet]
        public ActionResult<BudgetDTO> GetAllBudget(string userId,DateTime date)
        {
            try
            {
                BudgetResponse budgetRes = _budget.GetBudget(userId, date);
                return (budgetRes.Code != Configuration.GetValue<Int32>("ResponseStatus:Error:Code"))
                       ? Ok(budgetRes) : StatusCode(StatusCodes.Status500InternalServerError, budgetRes.Description);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("api/Budget/Update")]
        [HttpPut]
        public ActionResult<BudgetDTO> UpdateBudget([FromBody] BudgetDTO budget)
        {
            try
            {
                BudgetDTO budgetRes = _budget.UpdateBudget(budget);
                return (budgetRes.Code != Configuration.GetValue<Int32>("ResponseStatus:Error:Code"))
                       ? Ok(budgetRes) : StatusCode(StatusCodes.Status500InternalServerError, budgetRes.Description);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("api/Budget/Delete")]
        [HttpDelete]
        public ActionResult<BudgetDTO> DeleteBudget(string budgetId)
        {
            try
            {
                BudgetDTO budgetRes = _budget.DeleteBudget(budgetId);
                return (budgetRes.Code != Configuration.GetValue<Int32>("ResponseStatus:Error:Code"))
                       ? Ok(budgetRes) : StatusCode(StatusCodes.Status500InternalServerError, budgetRes.Description);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}