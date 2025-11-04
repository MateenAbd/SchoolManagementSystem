using AutoMapper;
using SMS.Application.Dto;
using SMS.Core.Entities;


namespace SMS.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //student module
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<StudentDocument, StudentDocumentDto>().ReverseMap();
            CreateMap<StudentEnrollment, StudentEnrollmentDto>().ReverseMap();

            //Identity
            CreateMap<User, UserDto>();
            
            //Admission module
            CreateMap<AdmissionInquiry, AdmissionInquiryDto>().ReverseMap();
            CreateMap<AdmissionApplication, AdmissionApplicationDto>().ReverseMap();
            CreateMap<AdmissionFeePayment, AdmissionFeePaymentDto>().ReverseMap();
            CreateMap<AdmissionFeeSummary, AdmissionFeeSummaryDto>().ReverseMap();
            CreateMap<AdmissionShortlistItem, AdmissionShortlistItemDto>().ReverseMap();
            CreateMap<AdmissionMeritItem, AdmissionMeritItemDto>().ReverseMap();
            CreateMap<AdmissionApplicationDocument, AdmissionApplicationDocumentDto>().ReverseMap();

            //attendance module
            CreateMap<StudentAttendance, StudentAttendanceDto>().ReverseMap();
            CreateMap<StudentLeaveRequest, StudentLeaveRequestDto>().ReverseMap();
            CreateMap<StaffAttendance, StaffAttendanceDto>().ReverseMap();
            CreateMap<StudentAttendanceSummary, StudentAttendanceSummaryDto>().ReverseMap();
            CreateMap<StaffAttendanceSummary, StaffAttendanceSummaryDto>().ReverseMap();
            CreateMap<BiometricDevice, BiometricDeviceDto>().ReverseMap();
            CreateMap<BiometricUserMap, BiometricUserMapDto>().ReverseMap();
            CreateMap<AbsentStudentContact, AbsentStudentContactDto>().ReverseMap();
            CreateMap<NotificationLog, NotificationLogDto>().ReverseMap();

            // Academic
            CreateMap<Subject, SubjectDto>().ReverseMap();
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<CourseSyllabus, CourseSyllabusDto>().ReverseMap();
            CreateMap<Classroom, ClassroomDto>().ReverseMap();
            CreateMap<TimetableEntry, TimetableEntryDto>().ReverseMap();
            CreateMap<LessonPlan, LessonPlanDto>().ReverseMap();
            CreateMap<AcademicCalendarEvent, AcademicCalendarEventDto>().ReverseMap();
            CreateMap<Exam, ExamDto>().ReverseMap();
            CreateMap<ExamPaper, ExamPaperDto>().ReverseMap();

            // Fee Management
            CreateMap<FeeHead, FeeHeadDto>().ReverseMap();
            CreateMap<FeeTerm, FeeTermDto>().ReverseMap();
            CreateMap<FeeStructureHeader, FeeStructureDto>().ReverseMap();
            CreateMap<FeeStructureDetail, FeeStructureDetailDto>().ReverseMap();
            CreateMap<FeeReceipt, FeeReceiptDto>().ReverseMap();
            CreateMap<FeeReceiptItem, FeeReceiptItemDto>().ReverseMap();
            CreateMap<StudentFeeLedger, StudentFeeLedgerDto>().ReverseMap();

            CreateMap<StudentFeeBalance, StudentFeeBalanceDto>() //CreateMap<Source, Destination>
                .ForMember(d => d.Balance, o => o.MapFrom(s => s.TotalDebit - s.TotalCredit));//was not needed as balance was also present in entity //ForMember(destinationProp, options=> options.MapFromo(sourceExpression))
            CreateMap<FeeFineRule, FeeFineRuleDto>().ReverseMap();
            CreateMap<FeeDiscountScheme, FeeDiscountSchemeDto>().ReverseMap();
            CreateMap<StudentScholarship, StudentScholarshipDto>().ReverseMap();
            CreateMap<StudentFeeAdjustment, StudentFeeAdjustmentDto>().ReverseMap();

        }
    }
}
