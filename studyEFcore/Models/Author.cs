using System.ComponentModel.DataAnnotations;

namespace studyEFcore.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? Country { get; set; }

        // Navigation
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}