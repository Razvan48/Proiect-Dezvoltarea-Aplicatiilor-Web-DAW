using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public int ANumberVotes { get; set; }

        public int userVoted {  get; set; }

        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? DiscussionId { get; set; }

        public virtual Discussion? Discussion { get; set; }

        // o discutie are mai multe comentarii => ICollection
        public virtual ICollection<Comment>? Comments { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}

