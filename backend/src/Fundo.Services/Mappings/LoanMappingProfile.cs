using AutoMapper;
using Fundo.Domain.Entities;
using Fundo.Services.DTOs;

namespace Fundo.Services.Mappings;

public class LoanMappingProfile : Profile
{
    public LoanMappingProfile()
    {
        // Loan -> LoanDto (Status is already string, no conversion needed)
        CreateMap<Loan, LoanDto>();

        // CreateLoanDto -> Loan
        CreateMap<CreateLoanDto, Loan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentBalance, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => LoanStatus.Active));
    }
}
