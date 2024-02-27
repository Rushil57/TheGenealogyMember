using FamilyMember.Entities;
using FamilyMember.Models;
using FamilyMember.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace FamilyMember.ApiControllers
{
    public class StaffMemberController : ApiController
    {
        private FamilyMemberEntities db = new FamilyMemberEntities();

        // GET: api/StaffMember
        public async Task<List<UserModels>> GetStaffMembers()
        {

            var result = await db.Users.Where(a => a.RoleId == 3 && a.IsDeleted == false).ToListAsync();
            var staffmember = result.Select(a => new UserModels
            {
                UserId = a.UserId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
               
                IsDeleted = a.IsDeleted,
                CreatedDate = DateTime.Now,
                CreatedBy = a.CreatedBy,
                ModifiedDate = DateTime.Now,
                ModifiedBy = a.ModifiedBy,
                MobileNumber = a.MobileNumber,
                Address = a.Address,
                Country = a.Country,
                State = a.State,
                City = a.City,
                ZipCode = a.ZipCode,

            }).ToList();

            return staffmember;
        }

        // GET: api/StaffMember/5
        [ResponseType(typeof(UserModels))]
        public async Task<IHttpActionResult> GetStaffMember(int id)
        {
            var data = await db.Users.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            var staffmember = new UserModels
            {
                UserId = data.UserId,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                
                IsDeleted = data.IsDeleted,
                CreatedDate = DateTime.Now,
                CreatedBy = data.CreatedBy,
                ModifiedDate = DateTime.Now,
                ModifiedBy = data.ModifiedBy,
                MobileNumber = data.MobileNumber,
                Address = data.Address,
                Country = data.Country,
                State = data.State,
                City = data.City,
                ZipCode = data.ZipCode,

            };

            return Ok(staffmember);
        }

        // DELETE: api/StaffMember/5
        public async Task<IHttpActionResult> DeleteStaffMember(int id)
        {
            var sessionStaffMember = (UserModels)(HttpContext.Current.Session["User"]);

            var staffmember = await db.Users.FindAsync(id);
            staffmember.IsDeleted = true;
            staffmember.ModifiedBy = sessionStaffMember.UserId;
            staffmember.ModifiedDate = DateTime.Now;

            db.Entry(staffmember).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //return new TextResult("hello", Request);
            return Ok();
        }

        // POST: api/StaffMember
        //[System.Web.Http.HttpPost]
        //[System.Web.Http.ActionName("SaveStaffMember")]
        public async Task<HttpResponseMessage> SaveStaffMember()
        {
            var reqfile = HttpContext.Current.Request;
           
            var jsonInput = HttpContext.Current.Request["model"];
            UserModels model = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModels>(jsonInput);
            bool result = true;
            string message = "";

            var sessionStaffMember = (UserModels)(HttpContext.Current.Session["User"]);

            int userId = 0;

            try
            {
                if (model.UserId == 0)
                {
                    var data = await db.Users.Where(a => (a.Email == model.Email || a.MobileNumber == model.MobileNumber) && a.IsDeleted == false).ToListAsync();

                    if (data.Any())
                    {
                        message = "Email or Mobile Number already registered. Please try another one.";
                        result = false;
                        return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
                    }
                    else
                    {
                        var staffmember = new User();
                        staffmember.FirstName = model.FirstName;
                        staffmember.LastName = model.LastName;
                        staffmember.Email = model.Email;

                        staffmember.IsDeleted = false;
                        staffmember.CreatedDate = DateTime.Now;
                        staffmember.CreatedBy = model.CreatedBy;
                        staffmember.ModifiedDate = DateTime.Now;
                        staffmember.ModifiedBy = model.ModifiedBy;
                        staffmember.MobileNumber = model.MobileNumber;
                        staffmember.Address = model.Address;
                        staffmember.Country = model.Country;
                        staffmember.State = model.State;
                        staffmember.City = model.City;
                        staffmember.ZipCode = model.ZipCode;
                        staffmember.RoleId = 3;
                        db.Users.Add(staffmember);
                        message = "Data saved successfully.";

                        await db.SaveChangesAsync();
                        userId = staffmember.UserId;
                     
                    }
                }
                else
                {
                   
                        var staffmember = await db.Users.FindAsync(model.UserId);
                        staffmember.FirstName = model.FirstName;
                        staffmember.LastName = model.LastName;
                        staffmember.Email = model.Email;

                        staffmember.IsDeleted = model.IsDeleted;
                        staffmember.CreatedDate = DateTime.Now;
                        staffmember.CreatedBy = model.CreatedBy;
                        staffmember.ModifiedDate = DateTime.Now;
                        staffmember.ModifiedBy = model.ModifiedBy;
                        staffmember.MobileNumber = model.MobileNumber;
                        staffmember.Address = model.Address;
                        staffmember.Country = model.Country;
                        staffmember.State = model.State;
                        staffmember.City = model.City;
                        staffmember.ZipCode = model.ZipCode;
                        staffmember.RoleId = 3;
                        db.Entry(staffmember).State = EntityState.Modified;
                        message = "Data saved successfully.";

                        await db.SaveChangesAsync();

                        userId = staffmember.UserId;
                    
                }



                if (reqfile.Files.Count > 0)
                {
                    string strMappath = HttpContext.Current.Server.MapPath("~/Policy Verification Documents/User/" + userId + "/");

                    if (!Directory.Exists(strMappath))
                    {
                        DirectoryInfo directory = Directory.CreateDirectory(strMappath);
                    }

                    // Some browsers send file names with full path. We only care about the file name.
                    //var fileName = Path.GetFileName(file.FileName);
                    var fileName = Path.GetFileNameWithoutExtension(reqfile.Files[0].FileName) + Path.GetExtension(reqfile.Files[0].FileName);

                    var destinationPath = Path.Combine(
                    System.Web.HttpContext.Current.Server.MapPath("~/Policy Verification Documents/User/" + userId + "/"), fileName);
                    reqfile.Files[0].SaveAs(destinationPath);

                    var userdetail = await db.Users.FindAsync(userId);
                    userdetail.FilePath = "~/Policy Verification Documents/User/" + userId + "/" + fileName ;
                    db.Entry(userdetail).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                message = "Some error has occured.";
                result = false;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
