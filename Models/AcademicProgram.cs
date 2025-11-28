using System.ComponentModel.DataAnnotations;

namespace EnrollmentApp.Models
{
    public class AcademicProgram
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El nombre del programa es obligatorio")]
        [Display(Name = "Nombre del Programa")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        // Opcional: Para saber cuántos créditos o semestres tiene
        public int TotalCredits { get; set; }
    }
}