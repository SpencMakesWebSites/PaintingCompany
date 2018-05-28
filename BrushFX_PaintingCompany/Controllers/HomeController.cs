using BrushFX_PaintingCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace BrushFX_PaintingCompany.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string Username, string EmailAddress, string Message, string PhoneNumber)
        {
            ContactInfo cI = new ContactInfo();
            cI.Username = Username;
            cI.EmailAddress = EmailAddress;
            cI.Message = Message;
            cI.PhoneNumber = PhoneNumber;
            ViewBag.Result = "Thank you! We will receive your request within 24 business hours, and will respond within 48 business hours.";

            if (ModelState.IsValid)
            {
                using (BrushFX_DBEntities1 db_ContactInfo = new BrushFX_DBEntities1())
                {
                    db_ContactInfo.ContactInfoes.Add(cI);
                    db_ContactInfo.SaveChanges();
                }
                return View();
            }
            else{
                ModelState.AddModelError("contactInfoInsertFailed", "The information didn't send.");
                return View();
            }



            return View();
        }

        public ActionResult Commercial()
        {
            return View();
        }

        public ActionResult Residential()
        {
            return View();
        }
    }
}