using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HockeySignups.Data;

namespace HockeySignups.Web.Models
{
    public class EventDetailsViewModel
    {
        public Event Event { get; set; }
        public IEnumerable<EventSignup> Signups { get; set; }
    }
}