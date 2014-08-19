using System;
using Orchard.ContentManagement;
using Orchard.Users.Models;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.Models
{
    public class SmartWalkUserPart : ContentPart<SmartWalkUserRecord>
    {
        public string FirstName
        {
            get { return Record.FirstName; }
            set { Record.FirstName = value; }
        }

        public string LastName
        {
            get { return Record.LastName; }
            set { Record.LastName = value; }
        }

        public bool IsVerificationRequested
        {
            get { return Record.IsVerificationRequested; }
            set { Record.IsVerificationRequested = value; }
        }

        public DateTime LastLoginAt {
            get { return Record.LastLoginAt; }
            set { Record.LastLoginAt = value; }
        }

        public DateTime CreatedAt
        {
            get { return Record.CreatedAt; }
            set { Record.CreatedAt = value; }
        }

        public UserPart User
        {
            get { return this.As<UserPart>(); }
        }   
    }
}