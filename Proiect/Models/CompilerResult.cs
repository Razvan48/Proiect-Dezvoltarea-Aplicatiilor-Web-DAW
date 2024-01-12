using System.ComponentModel.DataAnnotations;

namespace Proiect.Models {
    public class CompilerResult {
        [Key]
        public int Id;
        public bool Success { get; set; }
        public List<string>? ErrorMessages { get; set; }

        public string? OutputMessage { get; set; }

        
    }
}
