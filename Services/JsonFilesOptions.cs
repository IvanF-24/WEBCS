namespace EnrollmentApp.Services
{
    public class JsonFilesOptions
    {
        public string UsersFile { get; set; } = "Data/users.json";
        public string StudentsFile { get; set; } = "Data/students.json";
        public string ProgramsFile { get; set; } = "Data/programs.json";
        public string CoursesFile { get; set; } = "Data/courses.json";
        public string SchedulesFile { get; set; } = "Data/schedules.json";
    }
}