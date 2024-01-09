namespace Proiect.Models {
    public class Award {
        public int Id { get; set; }
        public int? DiscussionId { get; set; }
        public Discussion? Discussion { get; set; }

        public int? AnswerId { get; set; }
        public Answer? Answer { get; set; }

        public string? UserId {get; set;}   

        public bool DidAward { get; set; }

    }
}
