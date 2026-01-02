using FIRST.DTOs;
using FIRST.Services;
using Microsoft.AspNetCore.Mvc;
using FIRST.Shared;
using Microsoft.AspNetCore.Authorization;
using FIRST.DTOs.Demandes;
namespace FIRST.Controllers{
    [Authorize]
    [Route("api/[controller]")]


    public class DemandeController : BaseApiController
    {

    private readonly DemandeService _service;

    public DemandeController(DemandeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var demandes = await _service.GetAllAsync();
        return ApiOk(demandes, "Demandes retrieved");
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetDemandeById(int id)
    {
        var demande = await _service.GetDemandeById(id);

        if (demande == null)
            return ApiNotFound($"Demande with id: {id} not found");

        return ApiOk(demande, "Demande retrieved");
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDemandeDto dto)
    {
        var created = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetDemandeById),
            new { id = created.DemandeId },
            ApiResponse<object>.Ok(created, "Demande created")
        );
    }

    [HttpPut("{id:int:min(1)}")]

    public async Task<IActionResult> UpdateDemande(int id, [FromBody] UpdateDemandeDto dto)
    {
        var demande = await _service.UpdateDemande(id, dto);

        if(demande == null)
            return ApiNotFound($"Demande with id: {id} not found");

        return ApiOk(demande, "Demande updated");
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<IActionResult> DeleteDemande(int id)
    {
        var deleted = await _service.DeleteDemande(id);

        if (!deleted)
            return ApiNotFound($"Demande with id: {id} not found");

        return ApiOk(deleted, "Demande deleted");
    }


    }
}
