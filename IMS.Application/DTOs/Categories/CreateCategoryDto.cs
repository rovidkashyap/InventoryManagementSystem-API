using System.ComponentModel.DataAnnotations;

namespace IMS.Application.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Display(Name = "Category Id")]
        public Guid CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
