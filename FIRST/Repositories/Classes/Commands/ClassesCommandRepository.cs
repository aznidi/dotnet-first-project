using FIRST.Data;
using FIRST.Models.Classes;
using FIRST.DTOs.Classes.Request;
using FIRST.DTOs.Classes.Response;
using FIRST.Services.Classes.Queries;


namespace FIRST.Repositories.Classes.Commands
{
    public class ClassesCommandRepository : IClassesCommandRepository
    {
        private readonly AppDbContext _db;
        private readonly IClassesRepository _rep;

        public ClassesCommandRepository(AppDbContext db, IClassesRepository rep)
        {
            _db = db;
            _rep = rep;
        }
        public async Task<CreateClassResponseDto> CreateClassAsync(CreateClassDto dto)
        {
            Class classe = new Class
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                Capacity = dto.Capacity
            };

            _db.Classes.Add(classe);

            await  _db.SaveChangesAsync(); 

            return new CreateClassResponseDto
            {
                Id = classe.Id,
                Success = true
            };
        }


        public async Task<UpdateClassResponseDto?> UpdateClassAsync(int id, UpdateClassDto dto)
        {
            Class? @class = await _rep.GetClassEntityByIdAsync(id);

            if (@class == null)
            {
                return null;
            }

            @class.Name = dto.Name;
            @class.Description = dto.Description ?? string.Empty;
            @class.Capacity = dto.Capacity;

            _db.Classes.Update(@class);

            await _db.SaveChangesAsync();

            return new UpdateClassResponseDto
            {
                Id = @class.Id,
                Name =  @class.Name,
                Description = @class.Description,
                Capacity = @class.Capacity
            };
        }
        
        
        public async Task<bool?> DeleteClassAsync(int id)
        {
            Class? @class = await _rep.GetClassEntityByIdAsync(id);

            if (@class == null)
            {
                return null;
            }

            _db.Classes.Remove(@class);
            await _db.SaveChangesAsync();

            return true;
        }
    }

   
}