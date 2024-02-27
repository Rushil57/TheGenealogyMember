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
    public class PendingServicesController : ApiController
    {
        FamilyMemberEntities db = new FamilyMemberEntities();
        // GET: Service
        public async Task<List<RequestServices>> GetPendingServices()
        {
           // var result = await db.Services.Where(a => a.Status == "Pending").ToListAsync();
          var result = (await (from s in db.Services
                               join sm in db.ServiceMasters on s.ServiceMasterId equals sm.ServiceMasterId
                               where s.Status == "Pending"
                               select new
                               {
                                   ServiceId = s.ServiceId,
                                   ServiceName = sm.ServiceName,
                                   StartDate = s.StartDate,
                                   EndDate = s.EndDate,
                                   Status = s.Status,
                                   RequestedBy = db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).FirstName + " " + db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).LastName,
                               }).ToListAsync());
            var pendingservices = result.Select(a => new RequestServices
            {
                ServiceId = a.ServiceId,
                ServiceName = a.ServiceName,
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                EndDate = a.EndDate.ToString("dd/MM/yyyy"),
                Status = a.Status,
                RequestedBy = a.RequestedBy
            }).ToList();

            return pendingservices;
        }
        public async Task<List<UserModels>> GetFilteredStaffMembers(int serviceId)
        {
            var serviceRecord = await db.Services.FindAsync(serviceId);
            // var result = await db.Users.Where(a => a.RoleId == 3 && a.IsDeleted == false).ToListAsync();
            var serviceDetail = from s in db.Services
                         join u in db.Users on s.AssignedTo equals u.UserId
                         where ((s.StartDate >= serviceRecord.StartDate && s.StartDate <= serviceRecord.EndDate) || (s.EndDate >= serviceRecord.StartDate && s.EndDate <= serviceRecord.EndDate))
                         && s.ServiceId != serviceId 
                         select u.UserId;
            var result = await db.Users.Where(a => a.RoleId == 3 && a.UserCategoryMasterId == serviceRecord.ServiceMasterId && a.IsDeleted == false && !serviceDetail.Contains(a.UserId)).ToListAsync();
            var staffmember = result.Select(a => new UserModels
            {
                UserId = a.UserId,
                FirstName = a.FirstName,
                LastName = a.LastName,
               

            }).ToList();

            return staffmember;
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.ActionName("assignService")]
        public async Task<HttpResponseMessage> assignService(int ServiceId, int UserId)
        {
            bool result = true;
            string message = "";
            try
            {
                var serviceDetail = await db.Services.FindAsync(ServiceId);
                serviceDetail.Status = "Assigned";
                serviceDetail.AssignedTo = UserId;
                db.Entry(serviceDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                message = "Service Assigned successfully";
            }
            catch (Exception ex)
            {
                message = "Some error has occured.";
                result = false;
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }
        [ResponseType(typeof(RequestServices))]
        public async Task<IHttpActionResult> GetPendingService(int ServiceId)
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
            var pendingService = new RequestServices
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

            return Ok(pendingService);
        }
        public async Task<HttpResponseMessage> SavePendingService(RequestServices model)
        {
            bool result = true;
            string message = "";
            var pendingService = await db.Services.FindAsync(model.ServiceId);
           // pendingService.ServiceType = model.ServiceType;
            DateTime dateStart = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", null);
            pendingService.StartDate = Convert.ToDateTime(dateStart.ToString("MM/dd/yyyy") + " " + model.StartTime);
            DateTime dateEnd = DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", null);
            pendingService.EndDate = Convert.ToDateTime(dateEnd.ToString("MM/dd/yyyy") + " " + model.EndTime);
            // pendingService.StartDate = Convert.ToDateTime(model.StartDate + " " + model.StartTime);
            // pendingService.EndDate = Convert.ToDateTime(model.EndDate + " " + model.EndTime);
            pendingService.Status = model.Status;
            pendingService.ServiceMasterId = Convert.ToInt32(model.ServiceMasterIdString);
            db.Entry(pendingService).State = EntityState.Modified;
            await db.SaveChangesAsync();
            message = "Data saved successfully.";
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }
    }
}