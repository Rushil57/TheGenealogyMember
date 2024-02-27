using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FamilyMember.Models
{
    public class PaymentModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Amount { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string ProductInformation { get; set; }
        [Required]
        public string SuccessURL { get; set; }
        [Required]
        public string FailureURL { get; set; }
        [Required]
        public string Email { get; set; }

    }
}