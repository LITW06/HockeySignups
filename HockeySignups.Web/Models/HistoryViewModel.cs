using System.Collections.Generic;

namespace HockeySignups.Web.Models
{
    public class HistoryViewModel
    {
        public IEnumerable<EventWithCount> Events { get; set; }
    }
}