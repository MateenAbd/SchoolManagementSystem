using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SMS.Application.Commands.Student
{
    public class DeleteStudentDocumentByIdCommand : IRequest<long>
    {
        public int DocumentId { get; set; }
    }
}