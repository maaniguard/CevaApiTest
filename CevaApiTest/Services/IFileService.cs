using CevaApiTest.Models;

namespace CevaApiTest.Services
{
    public interface IFileService
    {
        Task SaveUserRecordsAsync(List<UserRecord> records);
    }
}
