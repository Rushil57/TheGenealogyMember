using FamilyMember.Entities;
using FamilyMember.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.Entity;


namespace FamilyMember.ApiControllers
{
    public class RequestServiceController : ApiController
    {
        FamilyMemberEntities db = new FamilyMemberEntities();
               
        // POST: api/User        
        public async Task<HttpResponseMessage> SaveRequestServices(RequestServices model)
        {
            bool result = true;
            string message = "";
            try {
               
                var RequestService = new Service();
               // RequestService.ServiceType = model.ServiceType;
                DateTime dateStart = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", null);
                RequestService.StartDate = Convert.ToDateTime(dateStart.ToString("MM/dd/yyyy") + " " + model.StartTime);
                DateTime dateEnd = DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", null);
                RequestService.EndDate = Convert.ToDateTime(dateEnd.ToString("MM/dd/yyyy") + " " + model.EndTime);
                RequestService.Status = "Pending";
                RequestService.RequestedBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                RequestService.ServiceMasterId = model.ServiceMasterId;
                db.Services.Add(RequestService);
               
                await db.SaveChangesAsync();
                message = "Data saved successfully.";
                
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                message = "Some error has occured.";
                result = false;
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { result = result, message = message });
        }

        //// GET: Service
        public async Task<List<ServiceMasterModel>> GetMasterServices()
        {
           
            var result = await db.ServiceMasters.ToListAsync();
            var masterServices = result.Select(a => new ServiceMasterModel
            {
                ServiceMasterId = a.ServiceMasterId,
                ServiceName = a.ServiceName,
              
            }).ToList();
            return masterServices;
        }
    }
}