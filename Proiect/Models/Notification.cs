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
            1. New Answer       -> notificare pt cel care a deschis discutia
            2. New Comment      -> notificare pt cel care a postat raspunsul
            3. New Best Answer  -> TODO
            4. Edit Discussion  -> notificare pt toti cei care au adaugat un comentariu
            5. Edit Answer      -> notificare pt toti cei care au comentat
            6. Edit Answer      -> notificare pt cel care a deschis discutia
            7. Edit Comment     -> notificare pt cel care a postat raspunsul
        */
        public int? Type { get; set; }
    }
}

