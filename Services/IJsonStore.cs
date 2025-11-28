using EnrollmentApp.Models;

namespace EnrollmentApp.Services
{
    public interface IJsonStore
    {
        // Users
        Task<IList<User>> GetUsersAsync();
        Task SaveUserAsync(User user);
        Task<User?> FindUserByUsernameAsync(string username);

        // Students
        Task<IList<Student>> GetStudentsAsync();
        Task<Student?> GetStudentAsync(Guid id);
        Task SaveStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);

        // Programs
        Task<IList<AcademicProgram>> GetProgramsAsync();
        Task SaveProgramAsync(AcademicProgram program);

        // Courses
        Task<IList<Course>> GetCoursesAsync();
        Task SaveCourseAsync(Course course);

        // Schedules
        Task<IList<Schedule>> GetSchedulesAsync();
        Task SaveScheduleAsync(Schedule schedule);
    }
}