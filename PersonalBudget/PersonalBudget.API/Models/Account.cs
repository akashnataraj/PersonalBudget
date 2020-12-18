using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonalBudget.API.Models
{
    public partial class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
