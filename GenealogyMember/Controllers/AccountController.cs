using FamilyMember.Entities;
using FamilyMember.Models;
using FamilyMember.Utility;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace FamilyMember.Controllers
{
    public class AccountController : Controller
    {
        Random random = new Random();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserModels model)
        {
            using (var db = new FamilyMemberEntities())
            {
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.MobileNumber))
                {
                    ViewBag.LoginErrorMessage = "Email and mobile number are required.";
                    return View("~/Views/Account/Login.cshtml");
                }
                else
                {
                    var user = db.Users.Where(a => a.Email == model.Email && a.MobileNumber == model.MobileNumber).FirstOrDefault();

                    if (user == null)
                    {
                        var userDetails = new User();
                        userDetails.Email = model.Email;
                        userDetails.MobileNumber = model.MobileNumber;
                        userDetails.AccessCode = random.Next(100000, 999999);
                        userDetails.IsVerified = false;
                        userDetails.RoleId = 4;
                        userDetails.CreatedDate = DateTime.Now;
                        db.Users.Add(userDetails);
                        await db.SaveChangesAsync();

                        string receiverMobileNumber = userDetails.MobileNumber;
                        string message = "Hello, Please use this one time access code:" + userDetails.AccessCode + " to verify your account with The Family Member.";
                        string sURL;
                        StreamReader objReader;
                        sURL = "http://107.167.189.214/API/WebSMS/Http/v1.0a/index.php?username=rushil&password=admin@123&sender=DEMOAC&to=" + receiverMobileNumber + "& message=" + message;
                       
                        WebRequest wrGETURL;
                        wrGETURL = WebRequest.Create(sURL);
                       
                       
                            Stream objStream;
                            objStream = wrGETURL.GetResponse().GetResponseStream();
                            objReader = new StreamReader(objStream);
                            objReader.Close();
                     

                        return Json(new  { isInserted = true, email = model.Email, mobileNumber = model.MobileNumber }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var sessionUser = new UserModels
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            IsDeleted = user.IsDeleted,
                            MobileNumber = user.MobileNumber,
                            Address = user.Address,
                            Country = user.Country,
                            State = user.State,
                            City = user.City,
                            ZipCode = user.ZipCode,
                            RoleId = user.RoleId
                        };
                        Session["User"] = sessionUser;
                        Session["UserId"] = sessionUser.UserId;
                        Session["RoleId"] = sessionUser.RoleId;
                        return Json(new { isInserted = false }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> VerifyAccessCode(string Email, string MobileNumber, int AccessCode, string FirstName, string LastName)
        {
            using (var db = new FamilyMemberEntities())
            {                
                    var user  = await db.Users.Where(a => a.Email == Email && a.MobileNumber == MobileNumber && a.AccessCode == AccessCode).FirstOrDefaultAsync();

                    if (user == null)
                    {                     
                        return Json(new { isVerified = false , message="Access code does not match"}, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                    var userdetail = await db.Users.FindAsync(user.UserId);
                    userdetail.FirstName = FirstName;
                    userdetail.LastName = LastName;
                    userdetail.IsVerified = true;
                    userdetail.IsDeleted = false;
                      db.Entry(userdetail).State = EntityState.Modified;                   
                      await db.SaveChangesAsync();
                     var sessionUser = new UserModels
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            IsDeleted = user.IsDeleted,
                            MobileNumber = user.MobileNumber,
                            Address = user.Address,
                            Country = user.Country,
                            State = user.State,
                            City = user.City,
                            ZipCode = user.ZipCode,
                            RoleId = user.RoleId,
                            IsVerified = user.IsVerified,
                        };
                        Session["User"] = sessionUser;
                        Session["UserId"] = sessionUser.UserId;
                        Session["RoleId"] = sessionUser.RoleId;
                        return Json(new { isVerified = true}, JsonRequestBehavior.AllowGet);
                    }
                
            }
        }
        [HttpGet]
        public ActionResult LoginVerifiedUser()
        {
            return RedirectToAction("Default", "Dashboard");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}