
using FIRST.DTOs.Classes.Request;
using FIRST.DTOs.Classes.Response;
using FIRST.Models.Classes;
using FIRST.Repositories.Classes.Commands;

namespace FIRST.Services.Classes.Commands;


public class ClassCommandService
{
    private readonly IClassesCommandRepository _r;

    public ClassCommandService(IClassesCommandRepository r)
    {
        _r = r;
    }
    public async Task<CreateClassResponseDto> CreateClassAsync(CreateClassDto r)
    {
        return await _r.CreateClassAsync(r);
    }   

    public async Task<UpdateClassResponseDto?> UpdateClassAsync(int id, UpdateClassDto r)
    {
        return await _r.UpdateClassAsync(id, r);
    }

    public async Task<bool?> DeleteClassAsync(int id)
    {
        return await _r.DeleteClassAsync(id);
    }
}
