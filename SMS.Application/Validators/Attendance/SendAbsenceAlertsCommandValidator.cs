using FluentValidation;
using SMS.Application.Commands.Attendance;

namespace SMS.Application.Validators.Attendance
{
    public class SendAbsenceAlertsCommandValidator : AbstractValidator<SendAbsenceAlertsCommand>
    {
        public SendAbsenceAlertsCommandValidator()
        {
            RuleFor(x => x.AttendanceDate).NotEmpty();
            RuleFor(x => x.SendEmail).Equal(true).Unless(x => x.SendSms)
                .WithMessage("At least one of SendEmail or SendSms must be true.");
        }
    }
}