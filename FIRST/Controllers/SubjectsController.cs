using FIRST.DTOs.Subjects;
using FIRST.Services;
using FIRST.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIRST.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SubjectsController : BaseApiController
    {
        private readonly SubjectService _service;

        public SubjectsController(SubjectService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subjects = await _service.GetAllAsync();
            return ApiOk(subjects, "Subjects retrieved");
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            var subject = await _service.GetByIdAsync(id);
            if (subject == null)
                return ApiNotFound($"Subject with id: {id} not found");

            return ApiOk(subject, "Subject retrieved");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubjectDto dto)
        {
            var created = await _service.CreateAsync(dto);
            if (created == null)
                return ApiBadRequest("Subject code already used");

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                ApiResponse<object>.Ok(created, "Subject created")
            );
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
                return ApiBadRequest("Subject not found or code already used");

            return ApiOk(updated, "Subject updated");
        }

        [HttpPatch("{id:int:min(1)}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchSubjectDto dto)
        {
            var updated = await _service.PatchAsync(id, dto);
            if (updated == null)
                return ApiBadRequest("Subject not found or code already used");

            return ApiOk(updated, "Subject updated (partial)");
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return ApiNotFound($"Subject with id: {id} not found");

            return NoContent();
        }
    }
}
