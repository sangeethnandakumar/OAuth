using Dapper.Contrib.Extensions;
using LiteDB;
using System;

namespace AuthServer.Configuration {
    public class AuthUsers {
        [BsonId]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public bool? IsActive { get; set; } = true;
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;
        public bool? IsEmailVerified { get; set; }
        public bool? IsMobileVerified { get; set; }
        public bool? EnableTwofactorAuthentication { get; set; }
    }
}