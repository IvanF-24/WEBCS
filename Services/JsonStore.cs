using System.Security.Cryptography;
using System.Text.Json;
using EnrollmentApp.Models;
using Microsoft.Extensions.Options;


namespace EnrollmentApp.Services
{
    public class JsonStore : IJsonStore
    {
        private readonly string _usersPath;
        private readonly string _studentsPath;
        private readonly string _programsPath;
        private readonly string _coursesPath;
        private readonly string _schedulesPath;
        private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };
        private readonly SemaphoreSlim _writeLock = new(1,1);

        public JsonStore(IOptions<JsonFilesOptions> options)
        {
            _usersPath = options.Value.UsersFile;
            _studentsPath = options.Value.StudentsFile;
            _programsPath = options.Value.ProgramsFile;
            _coursesPath = options.Value.CoursesFile;
            _schedulesPath = options.Value.SchedulesFile;

            EnsureFile(_usersPath, "[]");
            EnsureFile(_studentsPath, "[]");
            EnsureFile(_programsPath, "[]");
            EnsureFile(_coursesPath, "[]");
            EnsureFile(_schedulesPath, "[]");
        }

        private void EnsureFile(string path, string initial)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!File.Exists(path)) File.WriteAllText(path, initial);
        }

        private async Task<IList<T>> ReadListAsync<T>(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        private async Task WriteListAtomicAsync<T>(string path, IList<T> list)
        {
            await _writeLock.WaitAsync();
            try
            {
                var tmp = Path.GetTempFileName();
                await File.WriteAllTextAsync(tmp, JsonSerializer.Serialize(list, _opts));
                File.Copy(tmp, path, true);
                File.Delete(tmp);
            }
            finally { _writeLock.Release(); }
        }

        // Users
        public Task<IList<User>> GetUsersAsync() => ReadListAsync<User>(_usersPath);
        public async Task SaveUserAsync(User user)
        {
            var list = (await GetUsersAsync()).ToList();
            list.Add(user);
            await WriteListAtomicAsync(_usersPath, list);
        }
        public async Task<User?> FindUserByUsernameAsync(string username)
        {
            var users = await GetUsersAsync();
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        // Students
        public Task<IList<Student>> GetStudentsAsync() => ReadListAsync<Student>(_studentsPath);
        public async Task<Student?> GetStudentAsync(Guid id)
        {
            var list = await GetStudentsAsync();
            return list.FirstOrDefault(s => s.Id == id);
        }
        public async Task SaveStudentAsync(Student student)
        {
            var list = (await GetStudentsAsync()).ToList();
            list.Add(student);
            await WriteListAtomicAsync(_studentsPath, list);
        }

        // Programs
        public Task<IList<AcademicProgram>> GetProgramsAsync() => ReadListAsync<AcademicProgram>(_programsPath);
        public async Task SaveProgramAsync(AcademicProgram program)
        {
            var list = (await GetProgramsAsync()).ToList();
            list.Add(program);
            await WriteListAtomicAsync(_programsPath, list);
        }

        // Courses
        public Task<IList<Course>> GetCoursesAsync() => ReadListAsync<Course>(_coursesPath);
        public async Task SaveCourseAsync(Course course)
        {
            var list = (await GetCoursesAsync()).ToList();
            list.Add(course);
            await WriteListAtomicAsync(_coursesPath, list);
        }

        // Schedules
        public Task<IList<Schedule>> GetSchedulesAsync() => ReadListAsync<Schedule>(_schedulesPath);
        public async Task SaveScheduleAsync(Schedule schedule)
        {
            var list = (await GetSchedulesAsync()).ToList();
            list.Add(schedule);
            await WriteListAtomicAsync(_schedulesPath, list);
        }

        // PBKDF2 helpers
        public static void CreatePasswordHash(string password, out string saltBase64, out string hashBase64)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);
            using var derive = new Rfc2898DeriveBytes(password, salt, 150_000, HashAlgorithmName.SHA256);
            var hash = derive.GetBytes(32);
            saltBase64 = Convert.ToBase64String(salt);
            hashBase64 = Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string saltBase64, string hashBase64)
        {
            var salt = Convert.FromBase64String(saltBase64);
            using var derive = new Rfc2898DeriveBytes(password, salt, 150_000, HashAlgorithmName.SHA256);
            var hash = derive.GetBytes(32);
            return Convert.ToBase64String(hash) == hashBase64;
        }
        // --- ESTE ES EL MÉTODO QUE CONECTA LA MATRÍCULA ---
        public async Task UpdateStudentAsync(Student updatedStudent)
        {
            var list = (await GetStudentsAsync()).ToList();
            var index = list.FindIndex(s => s.Id == updatedStudent.Id);
            
            if (index != -1)
            {
                list[index] = updatedStudent;
                await WriteListAtomicAsync(_studentsPath, list);
            }
        }
    }
    
}