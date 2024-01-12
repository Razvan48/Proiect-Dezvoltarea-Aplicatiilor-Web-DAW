using System.ComponentModel.DataAnnotations;

namespace Proiect.Models {
    public class Codespace {
        [Key]
        public int Id { get; set; }
        public int? AnswerId { get; set; }
        public Answer? Answer { get; set; }

        public int? DiscussionId { get; set; }
        public Discussion? Discussion { get; set; }

        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }

        public string Language { get; set; }
    }
}
