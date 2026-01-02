namespace FIRST.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName  { get; set; } = string.Empty;
        public DateOnly DateNaissance { get; set; }
        public string Cin { get; set; } = string.Empty;
    }
}
