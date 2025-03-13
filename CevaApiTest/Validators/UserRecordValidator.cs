using CevaApiTest.Models;
using FluentValidation;

namespace CevaApiTest.Validators
{
    public class UserRecordValidator : AbstractValidator<UserRecord>
    {
        public UserRecordValidator()
        {
            RuleFor(record => record.ID).GreaterThan(0).WithMessage("ID must be greater than 0");
            RuleFor(record => record.UserID).GreaterThan(0).WithMessage("UserID must be greater than 0");
            RuleFor(record => record.EmployeeID).NotEmpty().WithMessage("EmployeeID cannot be empty");
            RuleFor(record => record.SiteName).NotEmpty().WithMessage("SiteName cannot be empty");
            RuleFor(record => record.BusinessUnitName).NotEmpty().WithMessage("BusinessUnitName cannot be empty");
            RuleFor(record => record.AccountName).NotEmpty().WithMessage("AccountName cannot be empty");
            RuleFor(record => record.GroupName).NotEmpty().WithMessage("GroupName cannot be empty");
            RuleFor(record => record.CategoryName).NotEmpty().WithMessage("CategoryName cannot be empty");
            RuleFor(record => record.TypeName).NotEmpty().WithMessage("TypeName cannot be empty");
            RuleFor(record => record.Date).NotEmpty().WithMessage("Date cannot be empty");
            RuleFor(record => record.Duration).NotEmpty().WithMessage("Duration cannot be empty");

        }
    }
}
