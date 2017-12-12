using System;
using System.Collections.Generic; // to use Lists

namespace WeddingPlanner.Models
{
    public class RSVP : BaseEntity
    {
        public int RSVPId { get;set; }
        public int UserId  { get;set; }
        public User Guest { get;set; }
        public int WeddingId { get;set; }
        public Wedding Wedding { get;set; }
    }
}