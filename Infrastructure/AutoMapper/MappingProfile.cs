using AutoMapper;
using Core.Entities;
using Infrastructure.DTOs.Accounts;
using Infrastructure.DTOs.CustomerInvoices;
using Infrastructure.DTOs.Customers;
using Infrastructure.DTOs.Entries;
using Infrastructure.DTOs.EntryDetails;
using Infrastructure.DTOs.SupplierInvoices;
using Infrastructure.DTOs.Suppliers;

namespace Infrastructure.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Account, AddAccountDto>().ReverseMap()
            .ForMember(destination => destination.Currency,
                opt => opt.NullSubstitute("SAR"));

        CreateMap<Account, UpdateAccountDto>().ReverseMap();
        
        CreateMap<Entry, AddEntryDto>().ReverseMap();
        CreateMap<Entry, UpdateEntryDto>().ReverseMap();
        
        CreateMap<EntryDetails, AddEntryDetailsDto>().ReverseMap();
        CreateMap<EntryDetails, UpdateEntryDetailsDto>().ReverseMap()
            .ForMember(dest => dest.UpdatedAt,
                o => o.MapFrom(s => DateTime.UtcNow));
        
        CreateMap<Supplier, AddSupplierDto>().ReverseMap();
        CreateMap<SupplierInvoice, AddSupplierInvoiceDto>().ReverseMap();
        
        CreateMap<Customer, AddCustomerDto>().ReverseMap();
        CreateMap<CustomerInvoice, AddCustomerInvoiceDto>().ReverseMap();
    }
}