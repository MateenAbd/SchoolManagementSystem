using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class UpdateInquiryCommandValidator : AbstractValidator<UpdateInquiryCommand>
    {
        public UpdateInquiryCommandValidator()
        {
            RuleFor(x => x.Inquiry.InquiryId).GreaterThan(0);
            RuleFor(x => x.Inquiry.ApplicantName).NotEmpty();
            RuleFor(x => x.Inquiry.InterestedClass).NotEmpty();
            RuleFor(x => x.Inquiry.AcademicYear).NotEmpty();
        }
    }
}