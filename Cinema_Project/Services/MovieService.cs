namespace Cinema_Project.Services
{

    public enum MovieImageType
    {
        MainImg,
        SubImage
    }

    public class MovieService
    {
        public string SaveFile(IFormFile ImageFile, MovieImageType productImageType = MovieImageType.MainImg)
        {
            try
            {
                var filepath = "";
                var filename = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                if (productImageType == MovieImageType.MainImg)
                {
                    filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images\\", filename);
                }

                else if (productImageType == MovieImageType.SubImage)
                {
                    filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images\\movie_sub_images\\", filename);

                }
                using (var stream = System.IO.File.Create(filepath))
                {
                    ImageFile.CopyTo(stream);
                }

                return filename;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors {ex.Message}");
                return null;
            }

        }


        public bool RemoveFile(string filename, MovieImageType productImageType = MovieImageType.MainImg)
        {
            try
            {
                var oldpath = "";
                if (productImageType == MovieImageType.MainImg)
                {

                    oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images\\", filename);
                }
                else if (productImageType == MovieImageType.SubImage)
                {
                    oldpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movie_images\\movie_sub_images\\", filename);
                }

                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
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
