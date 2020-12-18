using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalBudget.API.DTO
{
    public class BudgetDTO : StatusResponse
    {
        public string BudgetId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public int Budget { get; set; }
        public int Expense { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class BudgetResponse : StatusResponse
    {
        public List<BudgetDTO> BudgetList { get; set; }
    }
}
