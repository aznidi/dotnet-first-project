using FIRST.DTOs;
using FIRST.Services;
using Microsoft.AspNetCore.Mvc;
using FIRST.Shared;
using Microsoft.AspNetCore.Authorization;

namespace FIRST.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class StudentsController : BaseApiController
    {
        private readonly StudentService _service;

        public StudentsController(StudentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _service.GetAllAsync();
            return ApiOk(students, "Students retrieved");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
        {
            var created = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = created.Id },
                ApiResponse<object>.Ok(created, "Student created")
            );
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _service.GetStudentById(id);

            if (student == null)
                return ApiNotFound($"Student with id: {id} not found");

            return ApiOk(student, "Student retrieved");
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto dto)
        {
            var student = await _service.UpdateStudent(id, dto);

            if(student == null)
                return ApiNotFound($"Student with id: {id} not found");

            return ApiOk(student, "Student updated");
        }

        [HttpPatch("{id:int:min(1)}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchStudentDto dto)
        {
            var updated = await _service.PatchStudentAsync(id, dto);

            if (updated == null)
                return ApiNotFound($"Student with id: {id} not found");

            return ApiOk(updated, "Student updated (partial)");
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _service.Delete(id);

            if (! deleted)
                return ApiNotFound($"Student with id: {id} not found");

            return NoContent();
        }

    }
}
