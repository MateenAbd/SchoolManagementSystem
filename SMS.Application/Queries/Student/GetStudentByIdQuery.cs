using MediatR;
using SMS.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Queries.Student
{
    public class GetStudentByIdQuery : IRequest<StudentDto>
    {
        public int StudentId { get; set; }
    }
}
