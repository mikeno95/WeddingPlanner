using System;
using System.Collections.Generic; // to use Lists

namespace WeddingPlanner.Models
{
    public class Wedding :BaseEntity
    {
        public int WeddingId { get;set; }
        public string WedderOne { get;set; }
        public string WedderTwo { get;set; }
        public List<RSVP> Guests { get;set; }
        public int UserId { get;set; }
        public User Planner { get;set; }
        public DateTime Date { get;set; }
        public string Address { get;set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Wedding()
        {
            Guests = new List<RSVP>();
        }
    }
}