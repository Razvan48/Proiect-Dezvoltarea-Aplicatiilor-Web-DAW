using System.ComponentModel.DataAnnotations;
using Proiect_DAW.Models;

namespace Proiect_DAW.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }



        [Required(ErrorMessage = "Category name is compulsory")]
        public string CategoryName { get; set; }



        public virtual ICollection<Discussion>? discussions { get; set; }
    }
}
