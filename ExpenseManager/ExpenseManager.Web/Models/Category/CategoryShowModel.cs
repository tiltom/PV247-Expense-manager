using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Enums;

namespace ExpenseManager.Web.Models.Category
{
    /// <summary>
    ///     View model for category entity
    /// </summary>
    public class CategoryShowModel
    {
        public Guid Guid { get; set; }

        /// <summary>
        ///     Description of the category
        /// </summary>
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

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
        ///     Type of the category
        /// </summary>
        [Required]
        [Display(Name = "Type")]
        public CategoryType Type { get; set; }
    }
}