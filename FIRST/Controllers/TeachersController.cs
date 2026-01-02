using FIRST.DTOs.Teachers;
using FIRST.Services;
using FIRST.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIRST.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TeachersController : BaseApiController
    {
        private readonly TeacherService _service;

        public TeachersController(TeacherService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _service.GetAllAsync();
            return ApiOk(teachers, "Teachers retrieved");
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetTeacherById(int id)
        {
            var teacher = await _service.GetByIdAsync(id);

            if (teacher == null)
                return ApiNotFound($"Teacher with id: {id} not found");

            return ApiOk(teacher, "Teacher retrieved");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto)
        {
            var created = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetTeacherById),
                new { id = created.Id },
                ApiResponse<object>.Ok(created, "Teacher created")
            );
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] UpdateTeacherDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (updated == null)
                return ApiNotFound($"Teacher with id: {id} not found");

            return ApiOk(updated, "Teacher updated");
        }

        [HttpPatch("{id:int:min(1)}")]
        public async Task<IActionResult> PatchTeacher(int id, [FromBody] PatchTeacherDto dto)
        {
            var updated = await _service.PatchAsync(id, dto);

            if (updated == null)
                return ApiNotFound($"Teacher with id: {id} not found");

            return ApiOk(updated, "Teacher updated (partial)");
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return ApiNotFound($"Teacher with id: {id} not found");

            return NoContent();
        }
    }
}
