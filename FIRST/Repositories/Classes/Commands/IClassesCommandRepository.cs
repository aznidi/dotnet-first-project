using FIRST.DTOs.Classes.Request;
using FIRST.DTOs.Classes.Response;

namespace FIRST.Repositories.Classes.Commands
{
    public interface IClassesCommandRepository
    {

        public Task<CreateClassResponseDto> CreateClassAsync (CreateClassDto dto);

        public Task<UpdateClassResponseDto?> UpdateClassAsync(int id, UpdateClassDto dto);

        public Task<bool?> DeleteClassAsync(int id);
    }
}