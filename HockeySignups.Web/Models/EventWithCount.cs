using HockeySignups.Data;

namespace HockeySignups.Web.Models
{
    public class EventWithCount
    {
        public Event Event { get; set; }
        public int SignupCount { get; set; }
    }
}