using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public bool? Read { get; set; }

        public string? DateMonth { get; set; }

        public int? DateDay { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public int? DiscussionId { get; set; }

        public virtual Discussion? Discussion { get; set; }

        public int? AnswerId { get; set; }

        public virtual Answer? Answer { get; set; }

        public int? CommentId { get; set; }

        public virtual Comment? Comment { get; set; }

        /*
            Type:
            1. New Answer 
            2. New Comment 
            3. New Best Answer -> TODO
            4. Edit Discussion
            5. Edit Answer
        */
        public int? Type { get; set; }
    }
}

