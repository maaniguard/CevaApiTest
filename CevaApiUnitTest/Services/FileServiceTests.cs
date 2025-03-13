using CevaApiTest.Models;
using CevaApiTest.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CevaApiUnitTest.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<FileService>> _mockLogger;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<FileService>>();

            _fileService = new FileService(_mockConfig.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SaveUserRecordsAsync_NullOrEmptyRecords_ShouldLogError()
        {
            // Arrange
            List<UserRecord> records = null;

            // Act
            await _fileService.SaveUserRecordsAsync(records);

            // Assert
            _mockLogger.Verify(
                x => x.LogError("Records list is null or empty"),
                Times.Once
            );
        }

        [Fact]
        public async Task SaveUserRecordsAsync_MissingConfig_ShouldLogError()
        {
            // Arrange
            _mockConfig.Setup(c => c.GetSection("DirectoryConfig")).Returns((IConfigurationSection)null);

            var records = new List<UserRecord> { new UserRecord() };

            // Act
            await _fileService.SaveUserRecordsAsync(records);

            // Assert
            _mockLogger.Verify(
                x => x.LogError("DirectoryConfig is missing in configuration"),
                Times.Once
            );
        }

        [Fact]
        public async Task SaveUserRecordsAsync_ValidRecords_ShouldSaveFile()
        {
            // Arrange
            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(c => c.Get<DirectoryConfig>())
                .Returns(new DirectoryConfig { BasePath = "C:\\Data" });
            _mockConfig.Setup(c => c.GetSection("DirectoryConfig")).Returns(mockConfigSection.Object);

            var records = new List<UserRecord>
            {
                new UserRecord
                {
                    ID = 1,
                    UserID = 123,
                    EmployeeID = "EMP001",
                    SiteName = "Sydney Office",
                    BusinessUnitName = "HR",
                    AccountName = "Account A",
                    GroupName = "Group 1",
                    CategoryName = "Category X",
                    TypeName = "Type Y",
                    Date = "2025-03-14",
                    Duration = "02:30",
                    IsProcessed = false
                }
            };

            // Mock the timestamp for consistent naming
            var timestamp = "20250314120000"; // Fixed timestamp for test
            var expectedFilePath = Path.Combine("C:\\Data\\Users\\IN", $"UserData_{timestamp}.json");

            // Setup file service to use the mock timestamp
            var mockFileService = new Mock<IFileService>();
            mockFileService.Setup(service => service.SaveUserRecordsAsync(It.IsAny<List<UserRecord>>()))
                .Callback<List<UserRecord>>(records =>
                {
                    // Simulate saving the file with the fixed timestamp
                    var directoryPath = Path.Combine("C:\\Data\\Users\\IN");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var filePath = Path.Combine(directoryPath, $"UserData_{timestamp}.json");
                    File.WriteAllText(filePath, "Test JSON Content");
                })
                .Returns(Task.CompletedTask);

            // Act
            await mockFileService.Object.SaveUserRecordsAsync(records);

            // Assert
            // Check after the file creation
            Assert.True(File.Exists(expectedFilePath), $"The file {expectedFilePath} does not exist.");

            // Verify logging after file creation
            _mockLogger.Verify(
                x => x.LogInformation("Saved {RecordCount} records to {FilePath}", records.Count, expectedFilePath),
                Times.Once
            );

            // Clean up test file
            if (File.Exists(expectedFilePath))
            {
                File.Delete(expectedFilePath);
            }
        }



    }
}