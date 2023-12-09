using Microsoft.AspNetCore.Identity;

namespace Proiect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Answer>? Answers { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        // TODO: atribute suplimentare pentru user

        // TODO: Add
        // public virtual ICollection<Discussion>? Discussions { get; set; }
    }
}

