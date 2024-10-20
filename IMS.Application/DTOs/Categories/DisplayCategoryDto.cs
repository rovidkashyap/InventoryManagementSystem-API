using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTOs.Categories
{
    public class DisplayCategoryDto
    {
        [Display(Name = "Category ID")]
        public Guid CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
    }
}
