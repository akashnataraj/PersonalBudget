using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalBudget.API.Models
{
    public class PBDatabaseSettings : IPBDatabaseSettings
    {
        public string AccountCollectionName { get; set; }
        public string BudgetConfigCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IPBDatabaseSettings
    {
        string AccountCollectionName { get; set; }
        string BudgetConfigCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
