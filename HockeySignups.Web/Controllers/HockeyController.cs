using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HockeySignups.Data;
using HockeySignups.Web.Models;

namespace HockeySignups.Web.Controllers
{
    public class HockeyController : Controller
    {
        public ActionResult Index()
        {
            HomePageViewModel vm = new HomePageViewModel();
            if (TempData["Message"] != null)
            {
                vm.Message = (string)TempData["Message"];
            }
            else if (TempData["Error"] != null)
            {
                vm.ErrorMesage = (string)TempData["Error"];
            }

            return View(vm);
        }

        public ActionResult LatestEvent()
        {
            var db = new HockeySignupsDb(Properties.Settings.Default.ConStr);
            Event latestEvent = db.GetLatestEvent();
            EventSignupViewModel vm = new EventSignupViewModel();
            vm.Event = latestEvent;
            vm.EventStatus = db.GetEventStatus(latestEvent);

            vm.Signup = new EventSignup();
            if (Request.Cookies["firstName"] != null)
            {
                vm.Signup.FirstName = Request.Cookies["firstName"].Value;
                vm.Signup.LastName = Request.Cookies["lastName"].Value;
                vm.Signup.Email = Request.Cookies["email"].Value;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EventSignup(string firstName, string lastName, string email, int eventId)
        {
            var db = new HockeySignupsDb(Properties.Settings.Default.ConStr);
            var e = db.GetEventById(eventId);
            var status = db.GetEventStatus(e);
            if (status == EventStatus.InThePast)
            {
                TempData["Error"] = "You cannot sign up to a game in the past!!";
                return RedirectToAction("Index");
            }
            if (status == EventStatus.Full)
            {
                TempData["Error"] = "Nice try!";
                return RedirectToAction("Index");
            }
            EventSignup s = new EventSignup
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EventId = eventId
            };

            HttpCookie firstNameCookie = new HttpCookie("firstName", s.FirstName);
            HttpCookie lastNameCookie = new HttpCookie("lastName", s.LastName);
            HttpCookie emailCookie = new HttpCookie("email", s.Email);

            Response.Cookies.Add(firstNameCookie);
            Response.Cookies.Add(lastNameCookie);
            Response.Cookies.Add(emailCookie);

            db.AddEventSignup(s);

            TempData["Message"] =
                "You have successfully signed up for this weeks game, looking forward to checking you into the boards!";
            return RedirectToAction("Index");
        }

        public ActionResult NotificationSignup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NotificationSignup(string firstName, string lastName, string email)
        {
            var db = new HockeySignupsDb(Properties.Settings.Default.ConStr);
            var ns = new NotificationSignup
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            db.AddNotificationSignup(ns);
            return View("NotificationSignupConfirmation");
        }
    }
}