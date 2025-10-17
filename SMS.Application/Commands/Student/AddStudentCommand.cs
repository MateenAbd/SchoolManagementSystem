using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Student
{
    public class AddStudentCommand : IRequest<string>
    {
        public StudentDto StudentDto { get; set; }
    }
}
