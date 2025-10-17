using AutoMapper;
using SMS.Application.Dto;
using SMS.Core.Entities;

namespace SMS.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<StudentDocument, StudentDocumentDto>().ReverseMap();
            CreateMap<StudentEnrollment, StudentEnrollmentDto>().ReverseMap();
        }
    }
}
