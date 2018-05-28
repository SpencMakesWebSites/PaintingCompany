using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BrushFX_PaintingCompany.Models;

namespace BrushFX_PaintingCompany.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(SecureUser acc)
        {

            if (ModelState.IsValid)
            {
                using (BrushFX_DBEntities db = new BrushFX_DBEntities())
                {
                   
                    var hashedPass = HashPass(acc.EmailAddress.ToString());
                    //Check if that user already exists

                    SecureUser emailCheck = db.SecureUsers.FirstOrDefault(u => u.EmailAddress.ToLower().ToString() == acc.EmailAddress.ToLower().ToString());

                    if (emailCheck == null)
                    {
                        //Call method to register account
                        SaveChanges(acc);
                        //Take them to look at pictures on successful registration
                        SmtpClient mailClient = new SmtpClient("smtp.gmail.com",587);

                        //remove mine once Nick gets it
                        mailClient.Credentials = new NetworkCredential("brushfxpc@gmail.com","Password123!1");
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress("brushfxpc@gmail.com");
                        mailMessage.To.Add(acc.EmailAddress.ToString());
                        mailMessage.Subject = "BrushFX Sign Up";
                        mailMessage.Body = "Thank you for signing up for BrushFX! Track all of your past, and plan for your future work at our website!";

                        try
                        {
                            mailClient.Send(mailMessage);
                        } catch(SmtpFailedRecipientException e)
                        {
                            Console.WriteLine("Email message failed to send to user: " + e.FailedRecipient);
                        }

                        return RedirectToAction("Index", "Gallery");
                    }
                    else { ModelState.AddModelError("emailExists","This email already exists. Please Sign in or register a new email address");
                        return View();
                    }
                    
                }
                ModelState.Clear();
                ViewBag.Message = acc.UserName + " successfully registerd";
            }
            return View();
        }

        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string pass)
        {

            using (BrushFX_DBEntities db = new BrushFX_DBEntities())
            {
                SecureUser sU = new SecureUser();
                sU.EmailAddress = email;
                var hashPass = HashPass(pass);
                sU.PasswordHash = hashPass;

                var usr =
                   db.SecureUsers.Where(u => u.EmailAddress == email
                                 && u.PasswordHash == hashPass).FirstOrDefault();

                if(usr != null)
                {
                    Session["UserKey"] = usr.UserKey.ToString();
                    Session["Email"] = usr.EmailAddress.ToString();

                    FormsAuthentication.SetAuthCookie(sU.UserKey.ToString(),true);
                    return RedirectToAction("Index", "Gallery");
                }
             else
                {
                    ModelState.AddModelError("LogOnError", "The user name or password entered is incorrect");
                    return View();
                }

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        //Creates a new user
            public ActionResult SaveChanges(SecureUser userAcc)
        {
            BrushFX_DBEntities db = new BrushFX_DBEntities();

            SecureUser sc = new SecureUser();
            sc.UserName = userAcc.UserName;
            sc.EmailAddress = userAcc.EmailAddress;
            sc.PasswordHash = HashPass(userAcc.PasswordHash);
            sc.UserKey = userAcc.UserKey;

            Session["UserKey"] = sc.UserKey.ToString();
            Session["UserName"] = sc.UserName.ToString();

            //Signs and passes the authenticated credentials for the entirety of the session
            FormsAuthentication.SetAuthCookie(sc.UserKey.ToString(), true);

            db.SecureUsers.Add(sc);
            db.SaveChanges();

            return RedirectToAction("Index", "Gallery");
        }

        //Hashes password when loging in and registering
        public string HashPass(string password)
        {
            byte[] encodedPassword = new UTF8Encoding().GetBytes(password);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
            string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();

            return encoded;
        }

        //Signs out
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear(); //clears current keys andvalues from the session-state collection
            Session.Abandon(); //hard abandoned of all session objects

            //Clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            return RedirectToAction("Index", "Home");
        }
    }
}