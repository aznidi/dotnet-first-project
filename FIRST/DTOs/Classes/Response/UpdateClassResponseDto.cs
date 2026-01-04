namespace FIRST.DTOs.Classes.Response
{
    public class UpdateClassResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
    }
}
