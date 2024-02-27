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
    public class ServiceController : ApiController
    {
        FamilyMemberEntities db = new FamilyMemberEntities();
        // GET: Service
        public async Task<List<RequestServices>> GetAssignedServices()
        {
            var sessionUser = (UserModels)(HttpContext.Current.Session["User"]);
            // var result = sessionUser.RoleId == 3 ? await db.Services.Where(a => a.AssignedTo == sessionUser.UserId).ToListAsync() : await db.Services.Where(a => a.Status == "Assigned").ToListAsync();
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
                                                            where s.Status == "Assigned"
                                                            select new 
                                                            {
                                                                ServiceId = s.ServiceId,
                                                                ServiceName = sm.ServiceName,
                                                                StartDate = s.StartDate,
                                                                EndDate = s.EndDate,
                                                                Status = s.Status,
                                                                RequestedBy = db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).FirstName + " " + db.Users.FirstOrDefault(x => x.UserId == s.RequestedBy).LastName,
                                                            }).ToListAsync());


            var assignedServices = result.Select(a => new RequestServices
            {
                ServiceId = a.ServiceId,
               ServiceName = a.ServiceName,
                StartDate = a.StartDate.ToString("dd/MM/yyyy"),
                EndDate = a.EndDate.ToString("dd/MM/yyyy"),
                Status = a.Status,  
                RequestedBy = a.RequestedBy
            }).ToList();
            return assignedServices;
        }
      
        [ResponseType(typeof(RequestServices))]
        public async Task<IHttpActionResult> GetAssignedService(int ServiceId)
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
            var serviceDetail = from s in db.Services
                                join u in db.Users on s.AssignedTo equals u.UserId
                                where ((s.StartDate >= data.StartDate && s.StartDate <= data.EndDate) || (s.EndDate >= data.StartDate && s.EndDate <= data.EndDate))
                                && s.ServiceId != ServiceId
                                select u.UserId;
            var resultUser = await db.Users.Where(a => a.RoleId == 3 && a.UserCategoryMasterId == data.ServiceMasterId && !serviceDetail.Contains(a.UserId) && a.IsDeleted == false).ToListAsync();
            var masterUserList = resultUser.Select(a => new UserModels
            {
                UserId = a.UserId,
                FullName = a.FirstName + " " + a.LastName,        

            }).ToList();
            var assignedService = new RequestServices
            {
                ServiceId = data.ServiceId,
               // ServiceType = data.ServiceType,
                StartDate = data.StartDate.ToString("dd/MM/yyyy"),
                EndDate = data.EndDate.ToString("dd/MM/yyyy"), 
                StartTime = data.StartDate.ToString("hh:mm tt"),  
                EndTime = data.EndDate.ToString("hh:mm tt"),     
                Status = data.Status,
                ServiceMasterList = masterServices,
                ServiceMasterIdString = data.ServiceMasterId.ToString(),
                AssignedTo = (int)data.AssignedTo,
                UserMasterList = masterUserList,
                AssignedToString = data.AssignedTo.ToString(),
            };

            return Ok(assignedService);
        }
        public async Task<HttpResponseMessage> SaveAssignedServices(RequestServices model)
        {
            bool result = true;
            string message = "";
            var assignedService = await db.Services.FindAsync(model.ServiceId);          
            //assignedService.ServiceType = model.ServiceType;
            DateTime dateStart = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", null);
            assignedService.StartDate = Convert.ToDateTime(dateStart.ToString("MM/dd/yyyy") + " " + model.StartTime);
            DateTime dateEnd = DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", null);
            assignedService.EndDate = Convert.ToDateTime(dateEnd.ToString("MM/dd/yyyy") + " " + model.EndTime);
            //assignedService.StartDate = Convert.ToDateTime(model.StartDate + " " + model.StartTime);
            // assignedService.EndDate = Convert.ToDateTime(model.EndDate + " " + model.EndTime);
            assignedService.Status = model.Status;
            assignedService.ServiceMasterId = Convert.ToInt32(model.ServiceMasterIdString);
            db.Entry(assignedService).State = EntityState.Modified;
            await db.SaveChangesAsync();          
            message = "Data saved successfully.";
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }
     
        public async Task<HttpResponseMessage> GetNewServiceCount()
        {
            var sessionUser = (UserModels)(HttpContext.Current.Session["User"]);
            if(sessionUser.RoleId!=2)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { number = 0 });
            }
            var result = await db.Services.Where(a => a.IsApplicationUserNotified == false).ToListAsync();
            int countservice = result.Count();
           foreach(var service in result)
            {
                service.IsApplicationUserNotified = true;
                db.Entry(service).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { number = countservice});
        }
        public async Task<HttpResponseMessage> UpdateServiceDuration(int ServiceId, string ServiceDuration)
        {
            bool result = true;
            string message = "";
            try
            {
                var serviceDetails = await db.Services.FindAsync(ServiceId);
                serviceDetails.ServiceDuration = ServiceDuration;
                serviceDetails.Status = "Completed";

                db.Entry(serviceDetails).State = EntityState.Modified;
                message = "Data saved successfully.";


                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                message = "Some error has occured.";
                result = false;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }
    }
}