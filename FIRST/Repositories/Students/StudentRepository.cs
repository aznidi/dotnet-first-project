using FIRST.Data;
using FIRST.DTOs;
using FIRST.Models;
using Microsoft.EntityFrameworkCore;


namespace FIRST.Repositories.Students
{
    public class StudentRepository : IStudentRepository
    {
        protected readonly AppDbContext _db;

        public StudentRepository(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        public async Task<List<StudentDto>> GetAllStudentsAsync()
        {

            return await _db.Students.Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Cin = s.Cin,
            })
            .ToListAsync();
        }

        public async Task<StudentDto?> GetStudentDtoById(int StudentId)
        {
            return await _db.Students
                    .Where(s => s.Id == StudentId)
                    .Select(s => new StudentDto
                    {
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        DateNaissance = s.DateNaissance,
                        Cin = s.Cin
                    })
                    .FirstOrDefaultAsync();
        }

        public async  Task<Student?> GetStudentEntityByIdAsync(int StudentId)
        {
            return await _db.Students.FirstOrDefaultAsync(s => s.Id == StudentId);
        }

    }
}