using AutoMapper;
using Test.DB.Models;
using Test.DTO.Company;
using Test.DTO.Departament;
using Test.DTO.Employe;
using Test1.DB.Models;

namespace Test.DB.Mappings
{
    public class MapModel : Profile
    {
        public MapModel()
        {
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<CompanyCreateDto, Company>();
            CreateMap<CompanyCreateDto, CompanyDto>();

            CreateMap<Department, DepartamentDto>().ReverseMap();
            CreateMap<Department, DepartamentDtoRD>().ReverseMap();
            CreateMap<DepartamentCreateDto, Department>();
            CreateMap<DepartamentCreateDto, DepartamentDtoRD>();

            CreateMap<Passport, PassportDto>().ReverseMap();


            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.Id))
                .ReverseMap()
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => new Company { Id = src.CompanyId }));

            CreateMap<EmployeeCreateDto, Employee>()
                .ForMember(dest => dest.Passport, opt => opt.MapFrom(src => src.Passport));

            CreateMap<EmployeeUpdateDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PassportUpdateDto, Passport>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


        }
    }
}
