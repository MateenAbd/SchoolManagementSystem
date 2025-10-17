using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Dto;

namespace SMS.Application.Commands.Student
{
    public class AddOrUpdateEnrollmentCommand : IRequest<long>
    {
        public StudentEnrollmentDto Enrollment { get; set; } = new();
    }
}
