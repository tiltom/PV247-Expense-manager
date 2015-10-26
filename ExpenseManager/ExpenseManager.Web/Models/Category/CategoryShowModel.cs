using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Category
{
    public class CategoryShowModel
    {
        public Guid Guid { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Icon path")]
        public string Icon { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}