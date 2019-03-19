using HockeySignups.Data;

namespace HockeySignups.Web.Models
{
    public class EventSignupViewModel
    {
        public Event Event { get; set; }
        public EventStatus EventStatus { get; set; }
        public EventSignup Signup { get; set; }
    }
}