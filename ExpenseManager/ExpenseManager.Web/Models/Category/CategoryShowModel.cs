using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManager.Web.Models.Category
{
    public class CategoryShowModel
    {
        public Guid Guid { get; set; }

        /// <summary>
        ///     Name of the category
        /// </summary>
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Glyphicons name for category's icon
        /// </summary>
        [Required]
        [Display(Name = "Icon path")]
        public string Icon { get; set; }

        /// <summary>
        ///     Description of the category
        /// </summary>
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}