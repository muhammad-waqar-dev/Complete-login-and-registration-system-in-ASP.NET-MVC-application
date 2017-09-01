using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoginSignup_MVC.Models;
using System.Net;
using System.Net.Mail;
using System.Web.Security;


namespace LoginSignup_MVC.Controllers
{
    public class Student_Login_Controller : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //Registration POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] Student_LoginTB SL)
        {
            bool Status = false;
            string message = "";
            // Model Validation
            if(ModelState.IsValid)
            {
                #region//Email is already Exist
                var IsExist = IsEmailExist(SL.EmailID);
                if(IsExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(SL);
                }
                #endregion

                #region Generate Activation Code
                SL.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password Hashing
                SL.Password = Crypto.Hash(SL.Password);
                SL.ConfirmPassword = Crypto.Hash(SL.ConfirmPassword); //
                #endregion
                SL.IsEmailVerified = false;

                #region Save to Database
                using (DBLoginSignup_MVCEntities dc = new DBLoginSignup_MVCEntities())
                {
                    dc.Student_LoginTB.Add(SL);
                    dc.SaveChanges();

                    //Send Email to User
                    SendVerificationLinkEmail(SL.EmailID, SL.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link "+
                        "has been send to your email id :"+SL.EmailID;
                    Status = true;
                }
                 #endregion
            }
            else
            {
                message = "Invalid Request";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(SL);

        }
        //Verify Account
        
        //Verify Email

        //Verify Email LINK

        //Login
        
        //Login POST
       
        //Logout
      

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (DBLoginSignup_MVCEntities dc = new DBLoginSignup_MVCEntities())
            {
                var v = dc.Student_LoginTB.Where(a => a.EmailID == emailID).FirstOrDefault();
                    return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID,string activationCode)
        {
            var verifyUrl = "/Student_LoginTB/verifyAccount/"+activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("ENTER YOUR EMAIL ID HERE", "YOUR ACCOUNT NAME ENTER HERE");//enter your emailid and your account name
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "******"; //Replace with actual password
            string subject = "Your account is successfully created";

            string body = "<br/><br/>We are excited to tell you that your Dotnet Awesomer account is"+
                "successfully created Please click on the below link to verify your account"+
                "<br/><br/><a href='"+link+"'>"+link+"</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address,fromEmailPassword)
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })

                smtp.Send(message);
        }
     }
    
}