using CevaApiTest.Models;
using CevaApiTest.Validators;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CevaApiTest.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileService> _logger;
        public FileService(IConfiguration configuration,ILogger<FileService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SaveUserRecordsAsync(List<UserRecord> records)
        {
            if (records == null || !records.Any())
            {
                _logger.LogError("Records list is null or empty");
                return;
            }

           
            var config = _configuration.GetSection("DirectoryConfig").Get<DirectoryConfig>();
            if (config == null)
            {
                _logger.LogError("DirectoryConfig is missing in configuration");
                return;// exection will stop here
            }
            try
            {
                // Creating base directory if it doesn't exist
                var baseDir = Path.Combine(config.BasePath, "Users");
                Directory.CreateDirectory(baseDir); // Handles existence check internally

                // Creating IN directory if it doesn't exist
                var inDir = Path.Combine(baseDir, "IN");
                Directory.CreateDirectory(inDir);

                // Creating unique filename based on timestamp could add guid here as well
                var fileName = $"UserData_{DateTime.Now:yyyyMMddHHmmss}.json";
                var filePath = Path.Combine(inDir, fileName);

                // Serializing the JSON file and saving the records
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(records, options);

                await File.WriteAllTextAsync(filePath, jsonString);

                _logger.LogInformation("Saved {RecordCount} records to {FilePath}", records.Count, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving user records.");
            }
        }
    }
}
