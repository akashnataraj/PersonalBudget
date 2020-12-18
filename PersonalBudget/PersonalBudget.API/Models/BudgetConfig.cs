using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonalBudget.API.Models
{
    public partial class BudgetConfig
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public int Budget { get; set; }
        public int Expense { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
