using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SMS.Application.Commands.Attendance;
using SMS.Application.Interfaces;
using SMS.Core.Entities;
using SMS.Core.Interfaces;

namespace SMS.Application.Handlers.Attendance
{
    public class SendAbsenceAlertsHandler : IRequestHandler<SendAbsenceAlertsCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;

        public SendAbsenceAlertsHandler(IUnitOfWork uow, IEmailSender emailSender, ISmsSender smsSender)
        {
            _uow = uow;
            _emailSender = emailSender;
            _smsSender = smsSender;
        }

        public async Task<int> Handle(SendAbsenceAlertsCommand request, CancellationToken cancellationToken)
        {
            var contacts = await _uow.NotificationRepository.GetAbsentStudentContactsAsync(
                cancellationToken, request.AttendanceDate.Date, request.ClassName, request.Section);

            int sentCount = 0;

            foreach (var c in contacts)
            {
                if (request.SendEmail && !string.IsNullOrWhiteSpace(c.Email))
                {
                    var subject = $"Absence Alert - {request.AttendanceDate:yyyy-MM-dd} ({c.ClassName}{(string.IsNullOrEmpty(c.Section) ? "" : "-" + c.Section)})";
                    var body = BuildEmailBody(c);
                    var log = new NotificationLog
                    {
                        Type = "Email",
                        Recipient = c.Email!,
                        Subject = subject,
                        Body = body,
                        Status = "Pending",
                        RelatedDate = request.AttendanceDate.Date,
                        ClassName = c.ClassName,
                        Section = c.Section,
                        StudentId = c.StudentId
                    };
                    var id = await _uow.NotificationRepository.InsertNotificationLogAsync(cancellationToken, log);

                    bool ok = false; string? error = null;
                    try { ok = await _emailSender.SendEmailAsync(c.Email!, subject, body, cancellationToken); }
                    catch (Exception ex) { ok = false; error = ex.Message; }

                    await _uow.NotificationRepository.UpdateNotificationLogStatusAsync(
                        cancellationToken, id, ok ? "Sent" : "Failed", ok ? null : error, ok ? DateTime.UtcNow : (DateTime?)null, 1);
                    if (ok) sentCount++;
                }

                if (request.SendSms && !string.IsNullOrWhiteSpace(c.Phone))
                {
                    var text = BuildSmsText(c);
                    var log = new NotificationLog
                    {
                        Type = "SMS",
                        Recipient = c.Phone!,
                        Subject = null,
                        Body = text,
                        Status = "Pending",
                        RelatedDate = request.AttendanceDate.Date,
                        ClassName = c.ClassName,
                        Section = c.Section,
                        StudentId = c.StudentId
                    };
                    var id = await _uow.NotificationRepository.InsertNotificationLogAsync(cancellationToken, log);

                    bool ok = false; string? error = null;
                    try { ok = await _smsSender.SendSmsAsync(c.Phone!, text, cancellationToken); }
                    catch (Exception ex) { ok = false; error = ex.Message; }

                    await _uow.NotificationRepository.UpdateNotificationLogStatusAsync(
                        cancellationToken, id, ok ? "Sent" : "Failed", ok ? null : error, ok ? DateTime.UtcNow : (DateTime?)null, 1);
                    if (ok) sentCount++;
                }
            }

            return sentCount;
        }

        private static string BuildEmailBody(AbsentStudentContact c)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Dear {c.GuardianName ?? "Parent/Guardian"},");
            sb.AppendLine();
            sb.AppendLine($"This is to inform you that {c.StudentName} was marked Absent on {c.AttendanceDate:dddd, MMM dd yyyy} for class {c.ClassName}{(string.IsNullOrEmpty(c.Section) ? "" : "-" + c.Section)}.");
            sb.AppendLine("If this was due to sickness or any valid reason, please submit a leave request in the portal or inform the class teacher.");
            sb.AppendLine();
            sb.AppendLine("Regards,");
            sb.AppendLine("School Administration");
            return sb.ToString();
        }

        private static string BuildSmsText(AbsentStudentContact c)
        {
            return $"Absence Alert: {c.StudentName} absent on {c.AttendanceDate:yyyy-MM-dd} ({c.ClassName}{(string.IsNullOrEmpty(c.Section) ? "" : "-" + c.Section)}). - School";
        }
    }
}