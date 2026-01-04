using FIRST.DTOs;
using FIRST.Services;
using Microsoft.AspNetCore.Mvc;
using FIRST.Shared;
using Microsoft.AspNetCore.Authorization;
using FIRST.DTOs.Demandes;
using FIRST.DTOs.DemandesTypes;
namespace FIRST.Controllers{
    [Authorize]
    [Route("api/[controller]")]
    public class DemandeTypeController : BaseApiController
    {
    private readonly DemandeTypeService _service;
    public DemandeTypeController(DemandeTypeService service)
    {
        _service = service;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var demandeTypes = await _service.GetAllAsync();
        return ApiOk(demandeTypes, "Demande Types retrieved");
    }
    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetDemandeTypeById(int id)
    {
        var demandeType = await _service.GetDemandeTypeById(id);

        if (demandeType == null)
            return ApiNotFound($"Demande Type with id: {id} not found");

        return ApiOk(demandeType, "Demande Type retrieved");
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDemandeTypeDto dto)
    {
        var demandeType = await _service.CreateAsync(dto);
        if (demandeType == null)
            return ApiBadRequest("Failed to create Demande Type");

        return ApiCreated("Demande Type created", demandeType);
    }
    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> UpdateDemandeType(int id, UpdateDemandeTypeDto dto)
    {
        var updatedDemandeType = await _service.UpdateDemandeType(id, dto);

        if (updatedDemandeType == null)
            return ApiNotFound($"Demande Type with id: {id} not found");

        return ApiOk(updatedDemandeType, "Demande Type updated");
    }
    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> DeleteDemandeType(int id)
    {
        var deletedDemandeType = await _service.DeleteDemandeType(id);

        if (!deletedDemandeType)
            return ApiNotFound($"Demande Type with id: {id} not found");

        return ApiOk(deletedDemandeType, "Demande Type deleted");
    }
}

}