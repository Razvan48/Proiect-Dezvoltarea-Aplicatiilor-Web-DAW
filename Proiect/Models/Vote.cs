namespace Proiect.Models {
    public class Vote {
        public int Id { get; set; }
        
        // userul care va da votul
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // dicutii sau comentarii
        public int? DiscussionId { get; set; }
        public Discussion? Discussion { get; set; }

        public int? AnswerId { get; set; }
        public Answer? Answer { get; set; }

        public int DidVote { get; set; }
    }
}
