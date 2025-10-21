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
        }
    }
}
