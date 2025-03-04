namespace SmartHiring.APIs.Helpers
{
	public static class DocumentSettings
	{
		public static string UploadFile(IFormFile file, string FolderName)
		{
			string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);

			string FileName = $"{Guid.NewGuid()}{file.FileName}";

			string FilePath = Path.Combine(FolderPath, FileName);

			using var Fs = new FileStream(FilePath, FileMode.Create);
			file.CopyTo(Fs);

			return FileName;
		}

	}
}
