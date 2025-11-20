using System.ComponentModel.DataAnnotations;

namespace studyEFcore.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public int Year { get; set; }

        // FK to Author
        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        // Optional: category relation (om du vill koppla)
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}