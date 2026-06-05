namespace Cinema_Project.Services
{
    public class ActorService
    {
        public string SaveFile(IFormFile ImageFile)
        {
            try
            {
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\actor_images\\", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors {ex.Message}");
                return null;
            }

        }
        public bool RemoveFile(string fileName)
        {
            try
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\actor_images\\", fileName);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors {ex.Message}");
                return false;
            }

        }
    }
}
