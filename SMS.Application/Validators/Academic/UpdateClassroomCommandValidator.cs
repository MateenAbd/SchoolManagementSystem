using FluentValidation;
using SMS.Application.Commands.Academic;

namespace SMS.Application.Validators.Academic
{
    public class UpdateClassroomCommandValidator : AbstractValidator<UpdateClassroomCommand>
    {
        public UpdateClassroomCommandValidator()
        {
            RuleFor(x => x.Room.RoomId).GreaterThan(0);
            RuleFor(x => x.Room.RoomCode).NotEmpty();
            RuleFor(x => x.Room.Capacity).GreaterThan(0).When(x => x.Room.Capacity.HasValue);
        }
    }
}