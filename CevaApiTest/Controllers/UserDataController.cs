using CevaApiTest.Models;
using CevaApiTest.Services;
using CevaApiTest.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CevaApiTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly IFileService _fileService;
        public UserDataController(IFileService fileService)
        {
            _fileService = fileService;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<UserRecord> records)
        {
            if (records == null || !records.Any())
            {
                return BadRequest(new { Status = "Error", Message = "Records list is null or empty" });
            }
            var validator = new UserRecordValidator();
            var validationErrors = new List<object>();
            foreach (var record in records)
            {
                var result = validator.Validate(record);

                if (!result.IsValid)
                {
                    validationErrors.AddRange(result.Errors.Select(e => new
                    {
                        Field = e.PropertyName,
                        Error = e.ErrorMessage
                    }));
                }
            }
            // send BadRequest is there is validation errors 
            if (validationErrors.Any())
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Validation failed for some or all records.",
                    Errors = validationErrors
                });
            }
            try
                {
                // using aysnc service to save the records to a file
                await _fileService.SaveUserRecordsAsync(records);
                return Ok(new { Status = "Success", Message = "User records successfully saved " });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }
    }
}
