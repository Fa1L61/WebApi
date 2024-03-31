using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Models
{
    public class Conference
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AuthorId { get; set; }
        [AllowNull]
        public ActivityType ActivityType { get; set; }

        [AllowNull]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(300)]
        [AllowNull]
        public string Description { get; set; }
        [AllowNull]
        [MaxLength(1000)]
        public string Plan { get; set; }
        public bool IsSubmitted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime SubmittedDate { get; set; } = default;
    }
    
    public enum ActivityType
    {
        Report = 1,
        Masterclass = 2,
        Discussion = 3,
    }
}