using System.Data;
using System.Linq.Expressions;
using API.Utility;
using AspNetCore.Reporting;
using AutoMapper;
using ClosedXML.Excel;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Accounts;
using Infrastructure.DTOs.Customers;
using Infrastructure.DTOs.Suppliers;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class AccountsController : ApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly List<Account> orderedAccounts = new();

    public AccountsController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }


    [HttpGet("GetAccountsTree")]
    [Authorize(Permissions.Accounts.View)]
    public async Task<ActionResult<List<Account>>> GetAccountsTree()
    {
        return await FetchAccountsTree();
    }

    [HttpGet("GetAccountsPdf")]
    [Authorize(Permissions.Accounts.View)]
    public async Task<IActionResult> GetAccountsPdf()
    {
        var accounts = await FetchAccountsTree();

        var dt = new DataTable();

        dt.Columns.Add("Code");
        dt.Columns.Add("ArabicName");
        dt.Columns.Add("EnglishName");
        dt.Columns.Add("Balance");

        orderedAccounts.Clear();

        foreach (var account in accounts)
        {
            AddAccountToOrderedAccounts(account);
        }

        foreach (var account in orderedAccounts)
        {
            DataRow row = dt.NewRow();
            row["Code"] = account.Code;
            row["ArabicName"] = account.ArabicName;
            row["EnglishName"] = account.EnglishName;
            row["Balance"] = account.Balance;
            dt.Rows.Add(row);
        }

        string mimetype = String.Empty;
        int extension = 1;

        var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\accounts.rdlc";
        var pdfReportName = $"{Guid.NewGuid()}.pdf";

        var localReport = new LocalReport(reportPath);
        localReport.AddDataSource("accounts", dt);
        var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);


        return File(result.MainStream, "application/pdf", pdfReportName);
    }

    [HttpGet("GetAccountsExcel")]
    [Authorize(Permissions.Accounts.View)]
    public async Task<ActionResult<List<Account>>> GetAccountsExcel()
    {
        var accounts = await FetchAccountsTree();


        var dt = new DataTable();

        dt.Columns.Add("AccountName");
        dt.Columns.Add("AccountCode");
        dt.Columns.Add("Balance");

        orderedAccounts.Clear();

        foreach (var account in accounts)
        {
            AddAccountToOrderedAccounts(account);
        }

        foreach (var account in orderedAccounts)
        {
            DataRow row = dt.NewRow();
            row["AccountName"] = account.ArabicName;
            row["AccountCode"] = account.Code;
            row["Balance"] = account.Balance;
            dt.Rows.Add(row);
        }

        var excelFileName = $"{Guid.NewGuid()}.xlsx";


        using var wb = new XLWorkbook();
        wb.Worksheets.Add(dt, "Sheet1");

        using var stream = new MemoryStream();

        wb.SaveAs(stream);


        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            excelFileName);
    }

    private void AddAccountToOrderedAccounts(Account account)
    {
        orderedAccounts.Add(account);
        if (!account.IsMain)
        {
            return;
        }

        foreach (var subAccount in account.SubAccounts)
        {
            AddAccountToOrderedAccounts(subAccount);
        }
    }

    [HttpGet("GetAccountsByLevel")]
    [Authorize(Permissions.Accounts.View)]
    public async Task<ActionResult<List<Account>>> GetAccountsByLevel(int accountLevel)
    {
        var accounts = await _unitOfWork.AccountRepository.GetAllAsync(new List<Expression<Func<Account, bool>>>()
        {
            a => a.AccountLevel == accountLevel
        });


        foreach (var account in accounts)
        {
            if (account.IsMain)
            {
                account.Balance = GetSumOfBalance(account.Code.ToString());
            }
        }

        return Ok(accounts);
    }

    [HttpGet("GetAccountsByParent")]
    [Authorize(Permissions.Accounts.View)]
    public async Task<ActionResult> GetAccountsByParent(int id)
    {
        var account =
            await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == id,
                s => s.Include(a => a.SubAccounts));
        if (account is not null)
        {
            foreach (var a in account.SubAccounts)
            {
                if (a.IsMain)
                {
                    a.Balance = GetSumOfBalance(a.Code.ToString());
                }

                ;
            }

            return Ok(new
            {
                parentLevel = account.AccountLevel,
                accounts = account.SubAccounts
            });
        }

        return NotFound();
    }

    [HttpGet("GetAccount/{id:int}")]
    [Authorize(Permissions.Accounts.View)]
    public async Task<ActionResult<Account>> GetAccount(int id)
    {
        var account = await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(e => e.Id == id);

        if (account is null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [Authorize(Permissions.Accounts.Add)]
    [HttpPost("AddAccount")]
    public async Task<ActionResult<List<Account>>> AddAccount(AddAccountDto addAccountDto)
    {
        var account = _mapper.Map<Account>(addAccountDto);

        if (account.ParentId is null)
        {
            var lastRootAccount =
                await _unitOfWork.AccountRepository.All.Where(a => a.AccountLevel == 1).OrderByDescending(a => a.Code)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            if (lastRootAccount is not null)
            {
                if (lastRootAccount.Code == 5)
                {
                    throw new Exception("Max root accounts number exceeded");
                }

                account.Code = lastRootAccount.Code + 1;
            }
            else
            {
                account.Code = 1;
            }

            account.AccountLevel = 1;

            _unitOfWork.AccountRepository.Add(account);
        }
        else
        {
            var parentAccount =
                await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(
                    a => a.Id == account.ParentId,
                    include: s => s.Include(a => a.SubAccounts));

            var parentAccountHasEntries =
                _unitOfWork.EntryDetailsRepository.All.Any(e => e.AccountId == parentAccount!.Id);

            if (parentAccountHasEntries)
            {
                throw new Exception("Can't add children to an account that has entries");
            }

            if (!parentAccount!.IsMain)
            {
                parentAccount!.IsMain = true;
            }

            long lastChildCode = 0;
            var parentCode = parentAccount!.Code;
            var parentLevel = parentAccount.AccountLevel;
            var lastChild = parentAccount.SubAccounts.MaxBy(a => a.Code);

            if (lastChild is not null)
            {
                lastChildCode = lastChild.Code;
            }

            switch (parentLevel)
            {
                case 1:
                    account.AccountLevel = 2;
                    if (lastChildCode is 0)
                    {
                        var x = $"{parentCode}01";
                        account.Code = Int32.Parse(x);
                    }
                    else
                    {
                        if (IsChildrenCountExceeded(lastChildCode.ToString(), 2))
                        {
                            throw new Exception("Can't add more children");
                        }

                        account.Code = ++lastChildCode;
                    }

                    break;
                case 2:
                    account.AccountLevel = 3;
                    if (lastChildCode is 0)
                    {
                        var x = $"{parentCode}01";
                        account.Code = Int32.Parse(x);
                    }
                    else
                    {
                        if (IsChildrenCountExceeded(lastChildCode.ToString(), 3))
                        {
                            throw new Exception("Can't add more children");
                        }

                        account.Code = ++lastChildCode;
                    }

                    break;
                case 3:
                    account.AccountLevel = 4;
                    if (lastChildCode is 0)
                    {
                        var x = $"{parentCode}01";
                        account.Code = Int32.Parse(x);
                    }
                    else
                    {
                        if (IsChildrenCountExceeded(lastChildCode.ToString(), 4))
                        {
                            throw new Exception("Can't add more children");
                        }

                        account.Code = ++lastChildCode;
                    }

                    break;
                case 4:
                    account.AccountLevel = 5;
                    if (lastChildCode is 0)
                    {
                        var x = $"{parentCode}01";
                        account.Code = Int32.Parse(x);
                    }
                    else
                    {
                        if (IsChildrenCountExceeded(lastChildCode.ToString(), 5))
                        {
                            throw new Exception("Can't add more children");
                        }

                        account.Code = ++lastChildCode;
                    }

                    break;
                case 5:
                    account.AccountLevel = 6;
                    if (lastChildCode is 0)
                    {
                        var x = $"{parentCode}0001";
                        account.Code = Int64.Parse(x);
                    }
                    else
                    {
                        if (IsChildrenCountExceeded(lastChildCode.ToString(), 6))
                        {
                            throw new Exception("Can't add more children");
                        }

                        account.Code = ++lastChildCode;
                    }

                    break;
            }

            _unitOfWork.AccountRepository.Add(account);
        }

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("Account creation failed. try again later");
        }

        return Ok();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Permissions.Accounts.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var accountToDelete = await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == id);

        if (accountToDelete is null)
        {
            return NotFound();
        }

        // dodo
        var accountLevel = accountToDelete.AccountLevel;

        if (accountLevel == 1)
        {
            throw new Exception("Can't delete root account");
        }

        if (accountToDelete.IsMain)
        {
            throw new Exception("Can't delete main account. Remove it's children first");
        }

        var parent = await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(
            a => a.Id == accountToDelete.ParentId,
            s => s.Include(a => a.SubAccounts));

        if (parent is null)
        {
            throw new Exception("Parent not found");
        }

        var parentSubAccountsCount = parent.SubAccounts.Count;

        if (parentSubAccountsCount == 0)
        {
            throw new Exception("No accounts to delete");
        }

        if (parentSubAccountsCount == 1)
        {
            parent.IsMain = false;
        }

        _unitOfWork.AccountRepository.Remove(accountToDelete);

        await UpdateHierarchicalTreeCode(parent, accountToDelete);

        // dodo


        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpPut("{id:int}")]
    [Authorize(Permissions.Accounts.Edit)]
    public async Task<IActionResult> Update(int id, UpdateAccountDto dto)
    {
        var account = await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == id);

        if (account is null)
        {
            return NotFound();
        }

        var accountToUpdate = _mapper.Map(dto, account);


        _unitOfWork.AccountRepository.Update(accountToUpdate);
        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            return BadRequest();
        }

        return Ok();
    }


    [HttpGet]
    [Authorize(Permissions.Accounts.View)]
    public async Task<ActionResult<List<Account>>> Search([FromQuery] AccountSearchQueryParams queryParams)
    {
        var lang = Helpers.GetRequestHeader(Request, "Accept-Language");

        if (lang is null)
        {
            return BadRequest();
        }

        List<Expression<Func<Account, bool>>> filters = new();

        if (queryParams.Term is string term)
        {
            if (lang is "en")
            {
                filters.Add(a => a.EnglishName.Contains(term)
                                 ||
                                 a.Code.ToString().Contains(term));
            }
            else
            {
                filters.Add(a =>
                    a.ArabicName.Contains(term)
                    ||
                    a.Code.ToString().Contains(term));
            }
        }

        if (queryParams.SubOnly is true)
        {
            filters.Add(a => !a.IsMain);
        }

        var accounts = await _unitOfWork.AccountRepository
            .GetAllAsync(filters, 
                include: e => 
                    e.Include(x => x.Customer)
                        .Include(r => r.Supplier)
                    );

        return Ok(accounts);
    }

    [HttpGet("GetBankAccounts")]
    [Authorize(Permissions.Banks.View)]
    public async Task<ActionResult<List<Account>>> GetBankAccounts()
    {
        var banksAccount = await _unitOfWork
            .AccountRepository
            .GetFirstOrDefaultAsync(a => a.Code == 1011402,
                s => s.Include(a => a.SubAccounts));

        var accounts = new List<Account>() { };
        var subAccounts = banksAccount.SubAccounts;
        banksAccount.SubAccounts.Clear();
        accounts.Add(banksAccount);
        accounts.AddRange(subAccounts);

        foreach (var account in accounts)
        {
            await FillAccountsTree(account);
        }

        accounts.ForEach(a =>
        {
            if (a.IsMain)
            {
                a.Balance = GetSumOfBalance(a.Code.ToString());
            }
        });

        return accounts;
    }

    [HttpGet("GetCustomerAccounts")]
    [Authorize(Permissions.Customers.View)]
    public async Task<ActionResult<List<Account>>> GetCustomerAccounts()
    {
        var customersAccount = await _unitOfWork
            .AccountRepository
            .GetFirstOrDefaultAsync(a => a.Code == 10101,
                s => s.Include(a => a.SubAccounts));

        var accounts = new List<Account>() { };
        var subAccounts = customersAccount.SubAccounts;
        customersAccount.SubAccounts.Clear();
        accounts.Add(customersAccount);
        accounts.AddRange(subAccounts);

        foreach (var account in accounts)
        {
            await FillAccountsTree(account);
        }

        accounts.ForEach(a =>
        {
            if (a.IsMain)
            {
                a.Balance = GetSumOfBalance(a.Code.ToString());
            }
        });

        return accounts;
    }

    [HttpGet("GetSupplierAccounts")]
    [Authorize(Permissions.Suppliers.View)]
    public async Task<ActionResult<List<Account>>> GetSupplierAccounts()
    {
        var suppliersAccount = await _unitOfWork
            .AccountRepository
            .GetFirstOrDefaultAsync(a => a.Code == 20101,
                s => s.Include(a => a.SubAccounts));

        var accounts = new List<Account>() { };
        var subAccounts = suppliersAccount.SubAccounts;
        suppliersAccount.SubAccounts.Clear();
        accounts.Add(suppliersAccount);
        accounts.AddRange(subAccounts);

        foreach (var account in accounts)
        {
            await FillAccountsTree(account);
        }

        accounts.ForEach(a =>
        {
            if (a.IsMain)
            {
                a.Balance = GetSumOfBalance(a.Code.ToString());
            }
        });

        return accounts;
    }

    private decimal GetSumOfBalance(string code)
    {
        var sum = _unitOfWork.AccountRepository.All
            .Where(e => e.Code.ToString().StartsWith(code))
            .SelectMany(e => e.SubAccounts)
            .Sum(e => e.Balance);

        return sum;
    }

    private bool IsChildrenCountExceeded(string accountCodeAsString, int level)
    {
        var maximumCode = level switch
        {
            2 => string.Concat(accountCodeAsString.AsSpan(0, 1), "99"),
            3 => string.Concat(accountCodeAsString.AsSpan(0, 3), "99"),
            4 => string.Concat(accountCodeAsString.AsSpan(0, 5), "99"),
            5 => string.Concat(accountCodeAsString.AsSpan(0, 7), "99"),
            6 => string.Concat(accountCodeAsString.AsSpan(0, 9), "9999"),
            _ => "0"
        };
        return long.Parse(accountCodeAsString) >= long.Parse(maximumCode);
    }

    private async Task UpdateHierarchicalTreeCode(Account startingNode, Account accountToDelete)
    {
        foreach (var account in
                 startingNode.SubAccounts.Where(subAccount => subAccount.Code > accountToDelete.Code))
        {
            if (account.IsMain)
            {
                account.Code--;
                // change child codes
                // split code and change parent code only
                await Iterate(account);
            }
            else
            {
                account.Code--;
            }
        }
    }

    private async Task Iterate(Account account)
    {
        if (!account.IsMain)
        {
            return;
        }

        var acc = await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(
            a => a.Id == account.Id,
            s => s.Include(a => a.SubAccounts));

        foreach (var subAcc in acc!.SubAccounts)
        {
            subAcc.Code = UpdateAccountCode(subAcc.Code, acc.Code);
            await Iterate(subAcc);
        }
    }

    private long UpdateAccountCode(long oldCode, long parentCode)
    {
        var count = parentCode.ToString().Length;
        var oldCodeAsString = oldCode.ToString();
        var updatedCodeAsString = oldCodeAsString.Remove(0, count).Insert(0, parentCode.ToString());
        return Int64.Parse(updatedCodeAsString);
    }

    private async Task FillAccountsTree(Account account)
    {
        var subAccounts = await _unitOfWork.AccountRepository.GetAllAsync(new List<Expression<Func<Account, bool>>>()
        {
            a => a.ParentId == account.Id
        });

        subAccounts.ForEach(a =>
        {
            if (a.IsMain)
            {
                a.Balance = GetSumOfBalance(a.Code.ToString());
            }
        });

        if (!subAccounts.Any())
        {
            return;
        }

        account.SubAccounts = subAccounts;

        foreach (var subAccount in subAccounts)
        {
            await FillAccountsTree(subAccount);
        }
    }

    private async Task<List<Account>> FetchAccountsTree()
    {
        var accounts = await _unitOfWork.AccountRepository.GetAllAsync(new List<Expression<Func<Account, bool>>>()
        {
            a => a.AccountLevel == 1
        });

        foreach (var account in accounts)
        {
            await FillAccountsTree(account);
        }

        accounts.ForEach(a =>
        {
            if (a.IsMain)
            {
                a.Balance = GetSumOfBalance(a.Code.ToString());
            }
        });

        return accounts;
    }

    [HttpPost]
    [HttpPost("AddCustomer")]
    [Authorize(Permissions.Customers.Add)]
    public async Task<ActionResult<List<Account>>> AddCustomer(AddCustomerDto addCustomerDto)
    {
        var customer = _mapper.Map<Customer>(addCustomerDto);

        var account = new Account()
        {
            ArabicName = customer.ArabicName,
            EnglishName = customer.EnglishName,
            Currency = "SAR"
        };

        var customersParentAccount = await GetAccountByCode(10101);

        var customersParentAccountHasEntries = _unitOfWork.EntryDetailsRepository
            .All.Any(e => e.AccountId == customersParentAccount!.Id);

        if (customersParentAccountHasEntries)
        {
            throw new Exception("Can't add children to an account that has entries");
        }

        if (!customersParentAccount!.IsMain)
        {
            customersParentAccount!.IsMain = true;
        }

        long lastChildCode = 0;
        var parentCode = customersParentAccount!.Code;
        var parentLevel = customersParentAccount.AccountLevel;
        var lastChild = customersParentAccount.SubAccounts.MaxBy(a => a.Code);

        if (lastChild is not null)
        {
            lastChildCode = lastChild.Code;
        }

        switch (parentLevel)
        {
            case 1:
                account.AccountLevel = 2;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 2))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 2:
                account.AccountLevel = 3;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 3))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 3:
                account.AccountLevel = 4;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 4))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 4:
                account.AccountLevel = 5;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 5))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 5:
                account.AccountLevel = 6;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}0001";
                    account.Code = Int64.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 6))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
        }

        account.ParentId = customersParentAccount.Id;
        
        customer.Account = account;
        
        // _unitOfWork.AccountRepository.Add(account);
        _unitOfWork.CustomerRepository.Add(customer);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("Account creation failed. try again later");
        }

        return Ok();
    }

    [HttpPost]
    [HttpPost("AddSupplier")]
    [Authorize(Permissions.Suppliers.Add)]
    public async Task<ActionResult<List<Account>>> AddSupplier(AddSupplierDto addSupplierDto)
    {
        var supplier = _mapper.Map<Supplier>(addSupplierDto);

        var account = new Account()
        {
            ArabicName = supplier.ArabicName,
            EnglishName = supplier.EnglishName,
            Currency = "SAR"
        };

        var suppliersParentAccount = await GetAccountByCode(20101);

        var suppliersParentAccountHasEntries = _unitOfWork.EntryDetailsRepository
            .All.Any(e => e.AccountId == suppliersParentAccount!.Id);

        if (suppliersParentAccountHasEntries)
        {
            throw new Exception("Can't add children to an account that has entries");
        }

        if (!suppliersParentAccount!.IsMain)
        {
            suppliersParentAccount!.IsMain = true;
        }

        long lastChildCode = 0;
        var parentCode = suppliersParentAccount!.Code;
        var parentLevel = suppliersParentAccount.AccountLevel;
        var lastChild = suppliersParentAccount.SubAccounts.MaxBy(a => a.Code);

        if (lastChild is not null)
        {
            lastChildCode = lastChild.Code;
        }

        switch (parentLevel)
        {
            case 1:
                account.AccountLevel = 2;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 2))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 2:
                account.AccountLevel = 3;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 3))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 3:
                account.AccountLevel = 4;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 4))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 4:
                account.AccountLevel = 5;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}01";
                    account.Code = Int32.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 5))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
            case 5:
                account.AccountLevel = 6;
                if (lastChildCode is 0)
                {
                    var x = $"{parentCode}0001";
                    account.Code = Int64.Parse(x);
                }
                else
                {
                    if (IsChildrenCountExceeded(lastChildCode.ToString(), 6))
                    {
                        throw new Exception("Can't add more children");
                    }

                    account.Code = ++lastChildCode;
                }

                break;
        }

        account.ParentId = suppliersParentAccount.Id;
        
        supplier.Account = account;
        
        // _unitOfWork.AccountRepository.Add(account);
        _unitOfWork.SupplierRepository.Add(supplier);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("Account creation failed. try again later");
        }

        return Ok();
    }

    private async Task<string> AddFoo(long accountCode)
    {
        return "";
    }

    private async Task<Account?> GetAccountByCode(long accountCode)
    {
        return await _unitOfWork
            .AccountRepository
            .GetFirstOrDefaultAsync(x => x.Code == accountCode,
                include: s => s.Include(a => a.SubAccounts));
    }
}