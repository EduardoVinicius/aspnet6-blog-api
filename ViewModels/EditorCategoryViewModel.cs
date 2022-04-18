using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class EditorCategoryViewModel
    {
        [Required]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "The name must be at least 3 characters long and have a maximum of 40 characters.")]
        public string Name { get; set; }
        
        [Required]
        public string Slug { get; set; }
    }
}
