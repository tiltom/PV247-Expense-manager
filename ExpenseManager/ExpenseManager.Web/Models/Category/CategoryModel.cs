using System;
using System.ComponentModel.DataAnnotations;
using ExpenseManager.Entity.Enums;
using ExpenseManager.Entity.Users;
using ExpenseManager.Resources.CategoryResources;

namespace ExpenseManager.Web.Models.Category
{
    /// <summary>
    ///     View model for category entity
    /// </summary>
    public class CategoryModel
    {
        public Guid Guid { get; set; }

        /// <summary>
        ///     Description of the category
        /// </summary>
        [Required]
        [Display(ResourceType = typeof (CategoryResource), Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        ///     Name of the category
        /// </summary>
        [Required]
        [Display(ResourceType = typeof (CategoryResource), Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        ///     Glyphicons icon for category
        /// </summary>
        [Required]
        [Display(ResourceType = typeof (CategoryResource), Name = "Icon")]
        public string Icon { get; set; }

        /// <summary>
        ///     Type of the category
        /// </summary>
        [Required]
        [Display(ResourceType = typeof (CategoryResource), Name = "Type")]
        public CategoryType Type { get; set; }

        /// <summary>
        ///     Creator of this category
        /// </summary>
        public UserProfile User { get; set; }

        /// <summary>
        ///     Indicates if it is possible for user to edit this category
        /// </summary>
        public bool EditPossible { get; set; }
    }
}