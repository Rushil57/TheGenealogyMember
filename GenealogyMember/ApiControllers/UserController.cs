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
    public class UserController : ApiController
    {
        private FamilyMemberEntities db = new FamilyMemberEntities();

        // GET: api/User
        public async Task<List<UserModels>> GetUsers()
        {

            var result = await db.Users.Where(a => a.IsDeleted == false).ToListAsync();
            var user = result.Select(a => new UserModels
            {
                UserId = a.UserId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                RoleId = a.RoleId,
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

            return user;
        }

        // GET: api/StaffMember/5
        [ResponseType(typeof(UserModels))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            var data = await db.Users.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            var resultAgency = await db.AgencyMasters.ToListAsync();
            var userAgency = resultAgency.Select(a => new AgencyMasterModel
            {
                AgencyMasterId = a.AgencyMasterId,
                AgencyName = a.AgencyName,

            }).ToList();
            var resultUserCategory = await db.UserCategoryMasters.ToListAsync();
            var userCategory = resultUserCategory.Select(a => new UserCategoryMasterModel
            {
                UserCategoryMasterId = a.UserCategoryMasterId,
                UserCategoryName = a.UserCategoryName,

            }).ToList();
            var user = new UserModels
            {
                UserId = data.UserId,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                RoleId = data.RoleId,
                RoleIds = data.RoleId.ToString(),
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
                UserCategoryMasterList = userCategory,
                UserCategoryMasterIdString = data.UserCategoryMasterId.ToString(),
                AgencyMasterList = userAgency,
                AgencyMasterIdString = data.AgencyMasterId.ToString()
            };

            return Ok(user);
        }

        [System.Web.Http.HttpDelete]
        // DELETE: api/StaffMember/5
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            var sessionUser = (UserModels)(HttpContext.Current.Session["User"]);

            var user = await db.Users.FindAsync(id);
            user.IsDeleted = true;
            user.ModifiedBy = sessionUser.UserId;
            user.ModifiedDate = DateTime.Now;

            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //return new TextResult("hello", Request);
            return Ok();
        }
        public async Task<UserModels> GetMasterAgencyUserCategory()
        {
            var resultAgency = await db.AgencyMasters.ToListAsync();
            var userAgency = resultAgency.Select(a => new AgencyMasterModel
            {
                AgencyMasterId = a.AgencyMasterId,
                AgencyName = a.AgencyName,

            }).ToList();
            var resultUserCategory = await db.UserCategoryMasters.ToListAsync();
            var userCategory = resultUserCategory.Select(a => new UserCategoryMasterModel
            {
                UserCategoryMasterId = a.UserCategoryMasterId,
                UserCategoryName = a.UserCategoryName,

            }).ToList();
            var user = new UserModels
            {
                UserCategoryMasterList = userCategory,
                UserCategoryMasterIdString = "",
                AgencyMasterList = userAgency,
                AgencyMasterIdString = ""
            };
            return user;
        }
        // POST: api/StaffMember
        //[System.Web.Http.HttpPost]
        //[System.Web.Http.ActionName("SaveStaffMember")]
        public async Task<HttpResponseMessage> SaveUser()
        {
            var reqfile = HttpContext.Current.Request;

            var jsonInput = HttpContext.Current.Request["model"];
            UserModels model = Newtonsoft.Json.JsonConvert.DeserializeObject<UserModels>(jsonInput);
            bool result = true;
            string message = "";

            var sessionUser = (UserModels)(HttpContext.Current.Session["User"]);

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
                        var user = new User();
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;

                        user.IsDeleted = false;
                        user.CreatedDate = DateTime.Now;
                        user.CreatedBy = model.CreatedBy;
                        user.ModifiedDate = DateTime.Now;
                        user.ModifiedBy = model.ModifiedBy;
                        user.MobileNumber = model.MobileNumber;
                        user.Address = model.Address;
                        user.Country = model.Country;
                        user.State = model.State;
                        user.City = model.City;
                        user.ZipCode = model.ZipCode;
                        user.RoleId = int.Parse(model.RoleIds);
                        user.AgencyMasterId = model.AgencyMasterIdString != "" ? Convert.ToInt32(model.AgencyMasterIdString) : (int?)null;
                        user.UserCategoryMasterId = Convert.ToInt32(model.UserCategoryMasterIdString);
                        db.Users.Add(user);
                        message = "Data saved successfully.";

                        await db.SaveChangesAsync();
                        userId = user.UserId;
                    }
                }
                else
                {
                   
                        var user = await db.Users.FindAsync(model.UserId);
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;

                        user.IsDeleted = model.IsDeleted;
                        user.CreatedDate = DateTime.Now;
                        user.CreatedBy = model.CreatedBy;
                        user.ModifiedDate = DateTime.Now;
                        user.ModifiedBy = model.ModifiedBy;
                        user.MobileNumber = model.MobileNumber;
                        user.Address = model.Address;
                        user.Country = model.Country;
                        user.State = model.State;
                        user.City = model.City;
                        user.ZipCode = model.ZipCode;
                        user.RoleId =int.Parse(model.RoleIds);
                        user.AgencyMasterId = model.AgencyMasterIdString != "" ? Convert.ToInt32(model.AgencyMasterIdString) : (int?)null;
                        user.UserCategoryMasterId = Convert.ToInt32(model.UserCategoryMasterIdString);
                        db.Entry(user).State = EntityState.Modified;
                        message = "Data saved successfully.";

                        await db.SaveChangesAsync();

                        userId = user.UserId;
                    
                }



                if (reqfile.Files.Count > 0 && reqfile.Files[0].ContentLength > 0)
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
                    userdetail.FilePath = "~/Policy Verification Documents/User/" + userId + "/" + fileName;
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