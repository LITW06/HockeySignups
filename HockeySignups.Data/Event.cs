using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HockeySignups.Data
{
    public class Event
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MaxPeople { get; set; }
    }
}
