using System;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
	public class Company
	{
		[Key]
		public int CompanyId { get; set; }
		[Required]
        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [Display(Name = "Street Address")]
        public string StreetAdress { get; set; }

		public string City { get; set; }
		public string State { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
	}
}

