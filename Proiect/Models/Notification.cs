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

        public bool? NewAnswer { get; set; }

        public bool? NewComment { get; set; }

        public bool? NewBestAnswer { get; set; }
    }
}

