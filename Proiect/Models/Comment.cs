using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public int NumberVotes { get; set; }

        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? AnswerId { get; set; }

        public virtual Answer? Answer { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}

