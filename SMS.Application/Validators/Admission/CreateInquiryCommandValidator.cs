using FluentValidation;
using SMS.Application.Commands.Admission;

namespace SMS.Application.Validators.Admission
{
    public class CreateInquiryCommandValidator : AbstractValidator<CreateInquiryCommand>
    {
        public CreateInquiryCommandValidator()
        {
            RuleFor(x => x.Inquiry.ApplicantName).NotEmpty();
            RuleFor(x => x.Inquiry.InterestedClass).NotEmpty();
            RuleFor(x => x.Inquiry.AcademicYear).NotEmpty();
        }
    }
}