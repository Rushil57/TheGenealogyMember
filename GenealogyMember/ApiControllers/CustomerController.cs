using FamilyMember.Entities;
using FamilyMember.Models;
using FamilyMember.Utility;
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
    public class CustomerController : ApiController
    {
        private FamilyMemberEntities db = new FamilyMemberEntities();

        // GET: api/Customer
        public async Task<List<UserModels>> GetCustomers()
        {
            
            var result = await db.Users.Where(a => a.RoleId == 4 && a.IsDeleted == false).ToListAsync();
            var customer = result.Select(a => new UserModels
            {
                UserId = a.UserId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                
                IsDeleted =a.IsDeleted,
                CreatedDate = DateTime.Now,
                CreatedBy  = a.CreatedBy,
                ModifiedDate = DateTime.Now,
                ModifiedBy = a.ModifiedBy,
                MobileNumber = a.MobileNumber,
                Address = a.Address,
                Country = a.Country,
                State = a.State,
                City = a.City,
                ZipCode = a.ZipCode,
               
            }).ToList();

            return customer;
        }

        // GET: api/Customer/5
        [ResponseType(typeof(UserModels))]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            var data = await db.Users.FindAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            var customer = new UserModels
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

            return Ok(customer);
        }

        // DELETE: api/Customer/5
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            var sessionCustomer = (UserModels)(HttpContext.Current.Session["User"]);

            var customer = await db.Users.FindAsync(id);
            customer.IsDeleted = true;
            customer.ModifiedBy = sessionCustomer.UserId;
            customer.ModifiedDate = DateTime.Now;
            
            db.Entry(customer).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //return new TextResult("hello", Request);
            return Ok();
        }

        // POST: api/Customer        
        public async Task<HttpResponseMessage> SaveCustomer(UserModels model)
        {
            bool result = true;
            string message = "";

            var sessionCustomer = (UserModels)(HttpContext.Current.Session["User"]);

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
                        var customer = new User();
                        customer.FirstName = model.FirstName;
                        customer.LastName = model.LastName;
                        customer.Email = model.Email;
                        customer.IsDeleted = false;
                        customer.CreatedDate = DateTime.Now;
                        customer.CreatedBy = model.CreatedBy;
                        customer.ModifiedDate = DateTime.Now;
                        customer.ModifiedBy = model.ModifiedBy;
                        customer.MobileNumber = model.MobileNumber;
                        customer.Address = model.Address;
                        customer.Country = model.Country;
                        customer.State = model.State;
                        customer.City = model.City;
                        customer.ZipCode = model.ZipCode;
                        customer.RoleId = 4;
                        db.Users.Add(customer);
                        message = "Data saved successfully.";
                    }
                }
                else
                {
                   
                        var customer = await db.Users.FindAsync(model.UserId);
                        customer.FirstName = model.FirstName;
                        customer.LastName = model.LastName;
                        customer.Email = model.Email;
                        customer.IsDeleted = model.IsDeleted;
                        customer.CreatedDate = DateTime.Now;
                        customer.CreatedBy = model.CreatedBy;
                        customer.ModifiedDate = DateTime.Now;
                        customer.ModifiedBy = model.ModifiedBy;
                        customer.MobileNumber = model.MobileNumber;
                        customer.Address = model.Address;
                        customer.Country = model.Country;
                        customer.State = model.State;
                        customer.City = model.City;
                        customer.ZipCode = model.ZipCode;
                        customer.RoleId = 4;
                        db.Entry(customer).State = EntityState.Modified;
                        message = "Data saved successfully.";
                    
                }

                await db.SaveChangesAsync();

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
