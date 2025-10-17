using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Queries.Student
{
    public class GetStudentDocumentsQuery : IRequest<IEnumerable<StudentDocumentDto>>
    {
        public int StudentId { get; set; }
    }
}
