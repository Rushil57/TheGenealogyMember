using FamilyMember.Entities;
using FamilyMember.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class RejectedServicesController : ApiController
    {
        FamilyMemberEntities db = new FamilyMemberEntities();

        public async Task<List<RequestServices>> GetRejectedServices()
        {
            var sessionUser = (UserModels)(HttpContext.Current.Session["User"]);
            // var result = sessionUser.RoleId == 3 ? await db.Services.Where(a => a.AssignedTo == sessionUser.UserId).ToListAsync() : await db.Services.Where(a => a.Status == "Rejected").ToListAsync();
            var result = sessionUser.RoleId == 3 ? (await (from s in db.Services
                                                           join sm in db.ServiceMasters on s.ServiceMasterId equals sm.ServiceMasterId
                                                           where s.AssignedTo == sessionUser.UserId
                                                           select new
                                                           {
                                                               ServiceId = s.ServiceId,
                                                               ServiceName = sm.ServiceName,
                                                               StartDate = s.StartDate,
                                                               EndDate = s.EndDate,
                                                               Status = s.Status,
                                                               RequestedBy = db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).FirstName + " " + db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).LastName,
                                                           }).ToListAsync()) : (await (from s in db.Services
                                                                                       join sm in db.ServiceMasters on s.ServiceMasterId equals sm.ServiceMasterId
                                                                                       where s.Status == "Rejected"
                                                                                       select new
                                                                                       {
                                                                                           ServiceId = s.ServiceId,
                                                                                           ServiceName = sm.ServiceName,
                                                                                           StartDate = s.StartDate,
                                                                                           EndDate = s.EndDate,
                                                                                           Status = s.Status,
                                                                                           RequestedBy = db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).FirstName + " " + db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).LastName,
                                                                                       }).ToListAsync());
          
            var services = result.Select(a => new RequestServices
            {
                ServiceId = a.ServiceId,
                ServiceName = a.ServiceName,
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                EndDate = a.EndDate.ToString("dd/MM/yyyy"),
                Status = a.Status,
                RequestedBy = a.RequestedBy
               
            }).ToList();

            return services;
        }

        [ResponseType(typeof(RequestServices))]
        public async Task<IHttpActionResult> GetRejectedService(int ServiceId)
        {
            var data = await db.Services.FindAsync(ServiceId);
            if (data == null)
            {
                return NotFound();
            }
            var result = await db.ServiceMasters.ToListAsync();
            var masterServices = result.Select(a => new ServiceMasterModel
            {
                ServiceMasterId = a.ServiceMasterId,
                ServiceName = a.ServiceName,

            }).ToList();
            var rejectedService = new RequestServices
            {
                ServiceId = data.ServiceId,
               // ServiceType = data.ServiceType,
                StartDate = data.StartDate.ToString("dd/MM/yyyy"),
                EndDate = data.EndDate.ToString("dd/MM/yyyy"),
                StartTime = data.StartDate.ToString("hh:mm tt"),
                EndTime = data.EndDate.ToString("hh:mm tt"),
                Status = data.Status,
                ServiceMasterList = masterServices,
                ServiceMasterIdString = data.ServiceMasterId.ToString()
            };

            return Ok(rejectedService);
        }
        public async Task<HttpResponseMessage> SaveRejectedService(RequestServices model)
        {
            bool result = true;
            string message = "";
            var rejectedService = await db.Services.FindAsync(model.ServiceId);
           // rejectedService.ServiceType = model.ServiceType;
            DateTime dateStart = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", null);
            rejectedService.StartDate = Convert.ToDateTime(dateStart.ToString("MM/dd/yyyy") + " " + model.StartTime);
            DateTime dateEnd = DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", null);
            rejectedService.EndDate = Convert.ToDateTime(dateEnd.ToString("MM/dd/yyyy") + " " + model.EndTime);
            // rejectedService.StartDate = Convert.ToDateTime(model.StartDate + " " + model.StartTime);
            //rejectedService.EndDate = Convert.ToDateTime(model.EndDate + " " + model.EndTime);
            rejectedService.Status = model.Status;
            rejectedService.ServiceMasterId = Convert.ToInt32(model.ServiceMasterIdString);
            db.Entry(rejectedService).State = EntityState.Modified;
            await db.SaveChangesAsync();
            message = "Data saved successfully.";
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }
    }
}