using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using HockeySignups.Data;
using HockeySignups.Web.Models;

namespace HockeySignups.Web.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateEvent(DateTime date, int maxPeople)
        {
            var db = new HockeySignupsDb(Properties.Settings.Default.ConStr);
            Event e = new Event { Date = date, MaxPeople = maxPeople };
            db.AddEvent(e);

            TempData["Message"] = "Event Successfully created, Id: " + e.Id;
            IEnumerable<NotificationSignup> signups = db.GetNotificationSignups();
            foreach (NotificationSignup signup in signups)
            {
                SendNotificationEmail(signup);
            }
            return RedirectToAction("Index", "Hockey");
        }

        private void SendNotificationEmail(NotificationSignup signup)
        {

            var fromAddress = new MailAddress("lithockeyapp@gmail.com", "Hockey App");
            var toAddress = new MailAddress(signup.Email, $"{signup.FirstName} {signup.LastName}");
            
            string fromPassword = "LitIsGreat";
            string subject = "New Game Posted!";
            string body = $"Hey {signup.FirstName}, The admin has posted a new game, sign up now to secure a spot!";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        public ActionResult HistoryWithGroupBy()
        {
            var db = new HockeySignupsDb(Properties.Settings.Default.ConStr);
            IEnumerable<EventPeopleCount> events = db.GetEventsWithCount();
            return View(events);
        }

        public ActionResult History()
        {
            var db = new HockeySignupsDb(Properties.Settings.Default.ConStr);
            IEnumerable<Event> events = db.GetEvents();
            //longer approach
            HistoryViewModel vm = new HistoryViewModel();
            //List<EventWithCount> eventsWithCounts = new List<EventWithCount>();
            //foreach (Event e in events)
            //{
            //    EventWithCount eventWithCount = new EventWithCount();
            //    eventWithCount.Event = e;
            //    eventWithCount.SignupCount = db.GetSignupCountForEvent(e.Id);
            //    eventsWithCounts.Add(eventWithCount);
            //}

            //vm.Events = eventsWithCounts;

            //shorter approach
            vm.Events = events.Select(e => new EventWithCount
            {
                Event = e,
                SignupCount = db.GetSignupCountForEvent(e.Id)
            });


            return View(vm);
        }

        public ActionResult EventDetails(int id)
        {
            var db = new HockeySignupsDb(Properties.Settings.Default.ConStr);
            Event e = db.GetEventById(id);
            IEnumerable<EventSignup> signups = db.GetEventSignups(id);
            var vm = new EventDetailsViewModel
            {
                Event = e,
                Signups = signups
            };
            return View(vm);
        }

    }
}