using System;
using System.Collections.Generic; // to use Lists

namespace WeddingPlanner.Models
{
    public class User :BaseEntity
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<RSVP> Weddings { get; set; }
        public List<Wedding> PlannedWeddings { get;set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User()
        {
            Weddings = new List<RSVP>();
            PlannedWeddings = new List<Wedding>();
        }
    }
}