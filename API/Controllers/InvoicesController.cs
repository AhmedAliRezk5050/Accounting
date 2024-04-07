using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.DTOs;
using Infrastructure.DTOs.CustomerInvoices;
using Infrastructure.DTOs.Customers;
using Infrastructure.DTOs.SupplierInvoices;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class InvoicesController : ApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public InvoicesController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("GetCustomersInvoices")]
    public async Task<ActionResult<PagedResultDto<List<CustomerInvoice>>>> GetCustomersInvoices(
        [FromQuery] PagingParams pagingParams)
    {
        var customerInvoices = await _unitOfWork.CustomerInvoiceRepository.GetAllAsync(
            null,
            s =>
                s.Include(e => e.Customer)
                    .ThenInclude(c => c.Account),
            x => x.OrderByDescending(e => e.Date),
            pagingParams: pagingParams
        );

        return Ok(new PagedResultDto<CustomerInvoice>
        {
            Items = customerInvoices,
            TotalCount = _unitOfWork.CustomerInvoiceRepository.Count()
        });
    }

    [HttpGet("GetSuppliersInvoices")]
    public async Task<ActionResult<PagedResultDto<List<SupplierInvoice>>>> GetSuppliersInvoices(
        [FromQuery] PagingParams pagingParams)
    {
        var supplierInvoices = await _unitOfWork.SupplierInvoiceRepository.GetAllAsync(
            null,
            s =>
                s.Include(e => e.Supplier)
                    .ThenInclude(c => c.Account),
            x => x.OrderByDescending(e => e.Date),
            pagingParams: pagingParams
        );

        return Ok(new PagedResultDto<SupplierInvoice>
        {
            Items = supplierInvoices,
            TotalCount = _unitOfWork.SupplierInvoiceRepository.Count()
        });
    }


    [HttpPost]
    [HttpPost("AddCustomerInvoice")]
    public async Task<ActionResult<List<Account>>> AddCustomerInvoice(AddCustomerInvoiceDto addCustomerInvoiceDto)
    {
        var customerInvoice = _mapper.Map<CustomerInvoice>(addCustomerInvoiceDto);

        _unitOfWork.CustomerInvoiceRepository.Add(customerInvoice);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("customerInvoice creation failed. try again later");
        }

        return Ok();
    }


    [HttpPost]
    [HttpPost("AddSupplierInvoice")]
    public async Task<ActionResult<List<Account>>> AddSupplierInvoice(AddSupplierInvoiceDto addSupplierInvoiceDto)
    {
        var supplierInvoice = _mapper.Map<SupplierInvoice>(addSupplierInvoiceDto);

        _unitOfWork.SupplierInvoiceRepository.Add(supplierInvoice);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("supplierInvoice creation failed. try again later");
        }

        return Ok();
    }
}