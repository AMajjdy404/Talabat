namespace Admin_Dashboard.Helpers
{
	public class DocumentSettings
	{
		public static string UploadFile(IFormFile file,string folderName)
		{
			//1. File Location Path
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images", folderName);

			//2. Get File Name and make it Unique
			var fileName = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";

			//3. Get File Path
			var filePath = Path.Combine(folderPath, fileName);

			//4. Use File Stream to make a copy
			using var fileStream = new FileStream(filePath, FileMode.Create);
			file.CopyTo(fileStream);

			return filePath;

		}

		public static bool DeleteFile(string FileUrl,string folderName)
		{
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", folderName);

			var filePath = Path.Combine(folderPath, FileUrl);

			if (File.Exists(filePath))
			{
				File.Delete(filePath);
				return true;
			}
			return false;


		}
	}
}
