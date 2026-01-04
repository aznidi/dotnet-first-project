using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIRST.Data;
using FIRST.DTOs.Classes;
using FIRST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace FIRST.Repositories.Classes
{
    public class ClassesRepository : IClassesRepository
    {
        private AppDbContext _db;

        public ClassesRepository (AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ClassDto>> GetClassesAsync()
        {
            return await _db.Classes
                .AsNoTracking()
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Capacity = c.Capacity
                })
                .ToListAsync();
        }

        public async Task<ClassDto?> GetClassByIdAsync(int id)
        {
            return await _db.Classes
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Capacity = c.Capacity
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Class ?> GetClassEntityByIdAsync(int id)
        {
            return await _db.Classes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}