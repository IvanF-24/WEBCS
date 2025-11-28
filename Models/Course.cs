using System.ComponentModel.DataAnnotations;

namespace EnrollmentApp.Models
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, Display(Name = "Código")]
        public string Code { get; set; } = string.Empty;

        [Required, Display(Name = "Nombre")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Créditos")]
        public int Credits { get; set; } = 0;

        [Display(Name = "Programa")]
        public Guid? ProgramId { get; set; }
    }
}