using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Application.Commands.Student
{
    public class DeleteStudentByIdCommand : IRequest<string>
    {
        public int StudentId { get; set; }
    }
}
