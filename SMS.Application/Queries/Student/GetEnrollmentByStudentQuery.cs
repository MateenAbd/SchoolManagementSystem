using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Student
{
    public class GetEnrollmentByStudentQuery : IRequest<StudentEnrollmentDto?>
    {
        public int StudentId { get; set; }
    }
}