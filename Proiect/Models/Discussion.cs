using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Discussion
    {
        [Key]
        public int Id { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        
        public bool? didAward { get; set; }
        public int NumberVotes { get; set; }

        [Required(ErrorMessage = "Discussion title is compulsory")]
        [StringLength(100, ErrorMessage = "Discussion title too long")]
        [MinLength(5, ErrorMessage = "Discussion title too short")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Discussion content is compulsory")]
        public string Content { get; set; }

        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Discussion category id is compulsory")]
        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        // o discutie are mai multe raspunsuri => ICollection
        public virtual ICollection<Answer>? Answers { get; set; }

        // o discutie este postata de un user => FK
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}

