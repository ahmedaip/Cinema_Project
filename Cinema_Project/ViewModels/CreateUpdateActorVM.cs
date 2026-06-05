namespace Cinema_Project.ViewModels
{
    public class CreateUpdateActorVM
    {
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public decimal Rate { get; set; }
        public string? Img { get; set; }
        public IFormFile ImageFile { get; set; }


    }
}
