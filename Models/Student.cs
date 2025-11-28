using System.ComponentModel.DataAnnotations;

namespace EnrollmentApp.Models
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, Display(Name = "Nombres")]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Apellidos")]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Date), Display(Name = "Fecha de nacimiento")]
        public DateTime DateOfBirth { get; set; }

        [EmailAddress, Display(Name = "Correo")]
        public string? Email { get; set; }

        [Phone, Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Display(Name = "Programa académico")]
        public Guid? ProgramId { get; set; }

        public List<Guid> EnrolledCourseIds { get; set; } = new();

        [Display(Name = "Notas")]
        public string? Notes { get; set; }

        public string? CreatedBy { get; set; }
    }
}