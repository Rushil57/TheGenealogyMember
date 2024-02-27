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

namespace FamilyMember.ApiControllers
{
    public class CompletedServicesController : ApiController
    {
        FamilyMemberEntities db = new FamilyMemberEntities();

        public async Task<List<RequestServices>> GetCompletedServices()
        {
            var sessionUser = (UserModels)(HttpContext.Current.Session["User"]);
            // var result = sessionUser.RoleId == 3 ? await db.Services.Where(a => a.AssignedTo == sessionUser.UserId).ToListAsync() : await db.Services.Where(a => a.Status == "Completed").ToListAsync();
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
                                                                                       where s.Status == "Completed"
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
        public async Task<IHttpActionResult> GetCompletedService(int ServiceId)
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
            var completedService = new RequestServices
            {
                ServiceId = data.ServiceId,
                //ServiceType = data.ServiceType,
                StartDate = data.StartDate.ToString("dd/MM/yyyy"),
                EndDate = data.EndDate.ToString("dd/MM/yyyy"),
                StartTime = data.StartDate.ToString("hh:mm tt"),
                EndTime = data.EndDate.ToString("hh:mm tt"),
                Status = data.Status,
                ServiceMasterList = masterServices,
                ServiceMasterIdString = data.ServiceMasterId.ToString()
            };

            return Ok(completedService);
        }
        public async Task<HttpResponseMessage> SaveCompletedService(RequestServices model)
        {
            bool result = true;
            string message = "";
            var completedService = await db.Services.FindAsync(model.ServiceId);
           // completedService.ServiceType = model.ServiceType;
            DateTime dateStart = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", null);
            completedService.StartDate = Convert.ToDateTime(dateStart.ToString("MM/dd/yyyy") + " " + model.StartTime);
            DateTime dateEnd = DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", null);
            completedService.EndDate = Convert.ToDateTime(dateEnd.ToString("MM/dd/yyyy") + " " + model.EndTime);
            //completedService.StartDate = Convert.ToDateTime(model.StartDate + " " + model.StartTime);
            //completedService.EndDate = Convert.ToDateTime(model.EndDate + " " + model.EndTime);
            completedService.Status = model.Status;
            completedService.ServiceMasterId = Convert.ToInt32(model.ServiceMasterIdString);
            db.Entry(completedService).State = EntityState.Modified;
            await db.SaveChangesAsync();
            message = "Data saved successfully.";
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }
    }
}