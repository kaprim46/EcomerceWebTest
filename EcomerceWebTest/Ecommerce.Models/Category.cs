using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
	public class Category
	{
		[Key]
        public int CategoryId { get; set; }
		[Required]
		[Display(Name ="Category Name")]
		[MaxLength(30)]
		public string Name { get; set; }

		[DisplayName("Display Order")]
		[Range(1,100, ErrorMessage ="Display order must be between 1 and 100")]
		public int DisplayedOrder { get; set; }
	}
}

