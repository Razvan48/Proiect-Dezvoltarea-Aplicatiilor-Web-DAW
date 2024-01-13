using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Proiect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Vote>? Votes { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Discussion>? Discussions { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }

        // atribute suplimentare pentru user
        [Required(ErrorMessage = "Prenumele este obligatoriu")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Imaginea este obligatorie")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Sectiunea 'Despre Mine' este obligatorie")]
        public string? AboutMe { get; set; }

        public DateTime Date { get; set; }

        public int? UnreadNotifications { get; set; }

        public DateTime CoolDownEnd { get; set; }

        public ApplicationUser()
        {
            FirstName = "-";
            LastName = "-";
            Image = "/images/intrebare.png";
            AboutMe = "-";
            Date = DateTime.Now;
            UnreadNotifications = 0;
            CoolDownEnd = DateTime.MinValue;
        }
    }
}

