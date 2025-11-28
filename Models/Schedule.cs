using System.ComponentModel.DataAnnotations;

namespace EnrollmentApp.Models
{
    public class Schedule
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, Display(Name = "Curso")]
        public Guid CourseId { get; set; }

        [Required, Display(Name = "Día")]
        public string Day { get; set; } = string.Empty;

        [Required, Display(Name = "Hora inicio")]
        public TimeSpan Start { get; set; }

        [Required, Display(Name = "Hora fin")]
        public TimeSpan End { get; set; }

        [Display(Name = "Aula")]
        public string? Room { get; set; }

        [Display(Name = "Profesor")]
        public string? Instructor { get; set; }
    }
}