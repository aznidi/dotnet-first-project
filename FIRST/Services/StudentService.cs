using FIRST.Data;
using FIRST.DTOs;
using FIRST.Models;
using FIRST.Repositories.Students;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Services
{
    public class StudentService
    {
        private readonly IStudentRepository repository;

        private readonly AppDbContext _db;

        public StudentService(IStudentRepository repository, AppDbContext appDb)
        {
           this.repository = repository;
           _db = appDb;
        }

        public async Task<List<StudentDto>> GetAllAsync()
        {
            return await repository.GetAllStudentsAsync();
        }

        public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
        {
            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateNaissance = dto.DateNaissance,
                Cin = dto.Cin
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            return new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateNaissance = student.DateNaissance,
                Cin = student.Cin
            };
        }
    
        public async Task<StudentDto?> GetStudentById(int Id)
        {
            return await repository.GetStudentDtoById(Id);
                
        }

        public async Task<StudentDto?> UpdateStudent(int id, UpdateStudentDto dto)
        {
            Student ?student = await repository.GetStudentEntityByIdAsync(id);

            if(student == null) return null;

            student.Cin = dto.Cin;
            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.DateNaissance = dto.DateNaissance;

            await _db.SaveChangesAsync();

            return new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateNaissance = student.DateNaissance,
                Cin = student.Cin
            };
        }

        public async Task<StudentDto?> PatchStudentAsync(int id, PatchStudentDto dto)
        {
            Student ?student = await repository.GetStudentEntityByIdAsync(id);
            if (student == null) return null;

            if (dto.FirstName != null) student.FirstName = dto.FirstName;
            if (dto.LastName != null) student.LastName = dto.LastName;
            if (dto.DateNaissance.HasValue) student.DateNaissance = dto.DateNaissance.Value;
            if (dto.Cin != null) student.Cin = dto.Cin;

            await _db.SaveChangesAsync();

            return new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateNaissance = student.DateNaissance,
                Cin = student.Cin
            };
        }

        public async Task<bool> Delete(int id)
        {
            Student ?student = await repository.GetStudentEntityByIdAsync(id);
            if (student == null) return false;

            _db.Students.Remove(student);

            await _db.SaveChangesAsync();

            return true;
        }

    }
}
