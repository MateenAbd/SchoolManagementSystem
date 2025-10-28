using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class CreateClassroomCommandValidator : AbstractValidator<CreateClassroomCommand>
    {
        public CreateClassroomCommandValidator()
        {
            RuleFor(x => x.Room.RoomCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Room.Capacity).GreaterThan(0).When(x => x.Room.Capacity.HasValue);
        }
    }
}