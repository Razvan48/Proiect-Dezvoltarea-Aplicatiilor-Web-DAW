using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is compulsory")]
        public string CategoryName { get; set; }

        public virtual ICollection<Discussion>? Discussions { get; set;}
    }
}

