using Microsoft.AspNetCore.Identity;

namespace Proiect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Vote>? Votes { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Discussion>? Discussions { get; set; }

        // atribute suplimentare pentru user
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Image { get; set; }
        public string? AboutMe { get; set; }
        public DateTime Date { get; set; }

        public ApplicationUser()
        {
            FirstName = "-";
            LastName = "-";
            Image = "/images/intrebare.png";
            AboutMe = "-";
            Date = DateTime.Now;
        }
    }
}

