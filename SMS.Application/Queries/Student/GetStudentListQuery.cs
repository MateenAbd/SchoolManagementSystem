using MediatR;
using SMS.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Queries.Student
{
    public class GetStudentListQuery : IRequest<IEnumerable<StudentDto>>
    {
    }
}
