using System.ComponentModel.DataAnnotations;

namespace Proiect_DAW.Models
{
    public class Discussion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Discussion title is compulsory")]
        [StringLength(100, ErrorMessage = "Discussion title too long")]
        [MinLength(5, ErrorMessage = "Discussion title too short")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Discussion content is compulsory")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        //[Razvan] acest required va fi degeaba, putem sa avem butonul de adauga
        //discutie in interiorul unei categorii de discutie si atunci implicit se stie categoria
        [Required(ErrorMessage = "Discussion category is compulsory")]
        public int? CategoryId { get; set; }
    }
}
