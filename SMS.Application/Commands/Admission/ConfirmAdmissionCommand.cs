using System;
using MediatR;

namespace SMS.Application.Commands.Admission
{
    public class ConfirmAdmissionCommand : IRequest<int>
    {
        public int ApplicationId { get; set; }
        public string? Section { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    }
}