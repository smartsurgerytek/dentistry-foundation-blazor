using AutoMapper;
using Foundation.Dtos;
using Foundation.Entities;

namespace Foundation;

public class FoundationApplicationAutoMapperProfile : Profile
{
    public FoundationApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Organization, OrganizationDto>().ReverseMap();
        CreateMap<Organization, CreateUpdateOrganizationDto>().ReverseMap();

        CreateMap<Department, DepartmentDto>().ReverseMap();
        CreateMap<Department, CreateUpdateDepartmentDto>().ReverseMap();

        CreateMap<Doctor, DoctorDto>().ReverseMap();
        CreateMap<CreateUpdateDoctorDto, Doctor>().ReverseMap();

        CreateMap<Patient, PatientDto>().ReverseMap();
        CreateMap<CreateUpdatePatientDto, Patient>().ReverseMap();


        CreateMap<Record, RecordDto>()
         .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name));

        CreateMap<CreateUpdateRecordDto, Record>().ReverseMap();

        CreateMap<AuditLog, AuditLogDto>().ReverseMap();

    }
}
