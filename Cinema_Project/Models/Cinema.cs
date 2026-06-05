namespace Cinema_Project.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Img { get; set; }

    }
}
