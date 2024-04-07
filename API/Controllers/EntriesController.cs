using System.ComponentModel.DataAnnotations;
using API.Utility;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.DTOs.Entries;
using Infrastructure.DTOs.EntryDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using AspNetCore.Reporting;
using ClosedXML.Excel;
using Infrastructure.DTOs;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers;

[Authorize]
public class EntriesController : ApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public List<long> RevenuesBlackList { get; set; } = new() { 405 };
    public List<long> ExpensesWhiteList { get; set; } = new() { 502, 503, 504, 506, 507 };

    public EntriesController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    [Authorize(Permissions.Entries.View)]
    public async Task<ActionResult<PagedResultDto<List<Entry>>>> Get([FromQuery] PagingParams pagingParams)
    {
        var entries = await _unitOfWork.EntryRepository.GetAllAsync(
            null,
            s =>
                s.Include(e => e.EntryDetails)
                    .ThenInclude(entryDetail => entryDetail.Account),
            x => x.OrderBy(e => e.IsPosted).ThenByDescending(e => e.EntryDate),
            pagingParams: pagingParams
        );
        
        return Ok(new PagedResultDto<Entry>
        {
         Items   = entries,
         TotalCount = _unitOfWork.EntryRepository.Count()
        });
    }

    [HttpGet("{id}")]
    [Authorize(Permissions.Entries.View)]
    public async Task<ActionResult<Entry?>> GetEntry(int id)
    {
        var entry = await FetchEntry(id);
        return Ok(entry);
    }

    [HttpPost]
    [Authorize(Permissions.Entries.Add)]
    public async Task<IActionResult> Add(AddEntryDto dto)
    {
        if (dto.IsOpening)
        {
            dto.EntryDate = new DateTime(DateTime.Now.Year, 1, 1);
            if (HasAccountsWithOpeningEntry(dto.EntryDetails))
            {
                return BadRequest("Account can't have more than one opening entry");
            }
        }

        var entry = _mapper.Map<Entry>(dto);

        if (HasMainAccounts(entry.EntryDetails))
        {
            return BadRequest("Can't add entry with main account");
        }


        var totalDebit = entry.EntryDetails.Sum(entryDetail => entryDetail.Debit);
        var totalCredit = entry.EntryDetails.Sum(entryDetail => entryDetail.Credit);

        if (!IsBalancedEntry(totalDebit, totalCredit))
        {
            return BadRequest("Unbalanced entry");
        }

        entry.TotalDebit = totalDebit;
        entry.TotalCredit = totalCredit;

        // // ----
        // var salesVatAccount = (await GetAccountByCode(20102))!;
        // var entryHasCustomerAccount = EntryHasCustomersAccounts(entry.EntryDetails);
        //
        // if (entryHasCustomerAccount)
        // {
        //     var pendingAddAmountToSalesVatAccount = await HandleCustomersEntries(entry.EntryDetails);
        //     salesVatAccount.Credit += pendingAddAmountToSalesVatAccount;
        // }
        //
        // // ----

        _unitOfWork.EntryRepository.Add(entry);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("Adding entry failed");
        }

        return Ok();
    }


    [HttpDelete("{id:int}")]
    [Authorize(Permissions.Entries.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _unitOfWork.EntryRepository.GetFirstOrDefaultAsync(e => e.Id == id,
            e => e.Include(x => x.EntryDetails));

        if (entry is null)
        {
            return NotFound();
        }

        if (entry.IsPosted)
        {
            foreach (var entryDetail in entry.EntryDetails)
            {
                var account =
                    await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == entryDetail.AccountId);

                if (account is null)
                {
                    return BadRequest("Account not found");
                }

                account.Debit -= entryDetail.Debit;
                account.Credit -= entryDetail.Credit;
            }
        }


        _unitOfWork.EntryRepository.Remove(entry);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            return BadRequest();
        }

        if (entry.IsPosted && !entry.IsOpening)
        {
            var isProfitOrLossAccountUpdated = await UpdateProfitOrLossAccount(DateTime.MinValue, DateTime.Now);

            if (!isProfitOrLossAccountUpdated)
            {
                throw new Exception("Failed to post entry");
            }
        }

        return Ok();
    }


    [HttpPost("{id:int}/post")]
    [Authorize(Permissions.PostEntry.Add)]
    public async Task<IActionResult> PostEntry(int id)
    {
        var entry = await _unitOfWork.EntryRepository.GetFirstOrDefaultAsync(e => e.Id == id,
            e => e.Include(x => x.EntryDetails));

        if (entry is null)
        {
            return NotFound();
        }

        if (entry.IsPosted)
        {
            return BadRequest("Entry is already posted");
        }

        foreach (var entryDetail in entry.EntryDetails)
        {
            var account =
                await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == entryDetail.AccountId);

            if (account is null)
            {
                return BadRequest("Account not found");
            }

            account.Debit += entryDetail.Debit;
            account.Credit += entryDetail.Credit;
        }


        _unitOfWork.EntryRepository.Post(entry);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("Failed to post entry");
        }
        
        if (!entry.IsOpening)
        {
            var isProfitOrLossAccountUpdated = await UpdateProfitOrLossAccount(DateTime.MinValue, DateTime.Now);

            if (!isProfitOrLossAccountUpdated)
            {
                throw new Exception("Failed to post entry");
            }
        }

        return Ok();
    }

    [HttpPut("{id:int}")]
    [Authorize(Permissions.Entries.Edit)]
    public async Task<IActionResult> Update(int id, UpdateEntryDto dto)
    {
        var entry = await _unitOfWork.EntryRepository.GetFirstOrDefaultAsync(
            e => e.Id == id,
            e => e.Include(entry => entry.EntryDetails));

        if (entry is null)
        {
            return NotFound();
        }

        var isPostedEntry = entry.IsPosted;

        if (HasMainAccounts(entry.EntryDetails))
        {
            return BadRequest("Can't add entry with main account");
        }


        entry.Description = dto.Description!;
        entry.EntryDate = dto.EntryDate!.Value;
        entry.IsOpening = dto.IsOpening!.Value;


        // ---- 1 - remove any entry detail that is not in the dto entry details list
        entry.EntryDetails = entry.EntryDetails.Where(entryDetail =>
        {
            var entryDetailIsInDtoEntryDetails = dto.EntryDetails!.Select(x => x.Id).Contains(entryDetail.Id);
            if (!entryDetailIsInDtoEntryDetails)
            {
                var account = _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == entryDetail.AccountId)
                    .Result;

                if (isPostedEntry)
                {
                    account!.Debit -= entryDetail.Debit;
                    account.Credit -= entryDetail.Credit;
                }
            }

            return entryDetailIsInDtoEntryDetails;
        }).ToList();
        // ----

        // ---- 2 - update entry details
        foreach (var entryDetail in entry.EntryDetails)
        {
            foreach (var dtoEntryDetail in dto.EntryDetails!)
            {
                if (entryDetail.Id == dtoEntryDetail.Id)
                {
                    Account? account = null;

                    if (isPostedEntry)
                    {
                        account =
                            await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a =>
                                a.Id == entryDetail.AccountId);
                        account!.Debit -= entryDetail.Debit;
                        account.Credit -= entryDetail.Credit;
                    }
                    
                    entryDetail.Debit = dtoEntryDetail.Debit!.Value;
                    entryDetail.Credit = dtoEntryDetail.Credit!.Value;
                    entryDetail.Description = dtoEntryDetail.Description;
                    entryDetail.AccountId = dtoEntryDetail.AccountId!.Value;

                    if (isPostedEntry)
                    {
                        account!.Debit += entryDetail.Debit;
                        account.Credit += entryDetail.Credit;
                    }
                }
            }
        }
        // ----

        // ---- 3 - add new dto entry details to entry
        var entryDetailsIds = entry.EntryDetails!.Select(x => x.Id);

        var newPendingAddEntryDetailList = dto.EntryDetails!
            .Where(dtoEntryDetail => !entryDetailsIds.Contains(dtoEntryDetail.Id)).ToList();

        foreach (var detail in newPendingAddEntryDetailList)
        {
            var pendingEntryDetail = _mapper.Map<EntryDetails>(detail);
            pendingEntryDetail.EntryId = entry.Id;
            entry.EntryDetails.Add(pendingEntryDetail);
            if (isPostedEntry)
            {
                var account =
                    await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == detail.AccountId);
                account!.Debit += detail.Debit!.Value;
                account.Credit += detail.Credit!.Value;
            }
        }
        // ----


        var totalDebit = entry.EntryDetails.Sum(entryDetail => entryDetail.Debit);
        var totalCredit = entry.EntryDetails.Sum(entryDetail => entryDetail.Credit);

        if (!IsBalancedEntry(totalDebit, totalCredit))
        {
            return BadRequest("Unbalanced entry");
        }


        entry.TotalDebit = totalDebit;
        entry.TotalCredit = totalCredit;

        _unitOfWork.EntryRepository.Update(entry);

        var isSuccess = await _unitOfWork.SaveAsync();

        if (!isSuccess)
        {
            throw new Exception("Updating entry failed");
        }

        if (entry.IsPosted)
        {
            var isProfitOrLossAccountUpdated = await UpdateProfitOrLossAccount(DateTime.MinValue, DateTime.Now);

            if (!isProfitOrLossAccountUpdated)
            {
                throw new Exception("Failed to post entry");
            }
        }

        return Ok();
    }


    [HttpGet("GetGeneralLedger/{accountId}")]
    [Authorize(Permissions.GeneralLedger.View)]
    public async Task<ActionResult<GetGeneralLedgerByAccountCodeResponse>> GetGeneralLedger(
        int accountId,
        [FromQuery] GetGeneralLedgerByAccountCodeQueryParams queryParams)
    {
        var generalLedger = await FetchGeneralLedger(accountId, queryParams);

        if (generalLedger == null)
        {
            return BadRequest("No results found");
        }

        return Ok(generalLedger);
    }

    [HttpGet("GetIncomeStatement")]
    [Authorize(Permissions.IncomeStatement.View)]
    public async Task<ActionResult<IncomeStatementResponseModel>> GetIncomeStatement(
        [FromQuery] FetchTrialBalanceQueryParams queryParams)
    {
        var incomeStatementResponseModel = await FetchIncomeStatement(queryParams);

        if (incomeStatementResponseModel is null)
        {
            return BadRequest("No results found");
        }

        return Ok(incomeStatementResponseModel);
    }

    private async Task<IncomeStatementResponseModel?> FetchIncomeStatement(FetchTrialBalanceQueryParams queryParams)
    {
        var actualToDate = queryParams.ToDate ?? DateTime.UtcNow.Date;

        var revenuesAmount = await CalculateRevenues(queryParams.FromDate.Value, actualToDate);
        var revenueReturnsAmount = await CalculateRevenueReturns(queryParams.FromDate.Value, actualToDate);
        var capitalRevenuesRevenuesAmount = await CalculateCapitalRevenues(queryParams.FromDate.Value, actualToDate);
        var accidentCompensationRevenuesAmount = await CalculateAccidentCompensationRevenues(queryParams.FromDate.Value, actualToDate);

        var operatingExpensesAmount = await CalculateOperatingExpenses(queryParams.FromDate.Value, actualToDate);
        var administrativeExpenses = await CalculateAdministrativeExpenses(queryParams.FromDate.Value, actualToDate);
        var generalExpenses = await CalculateGeneralExpenses(queryParams.FromDate.Value, actualToDate);
        var sellingExpenses = await CalculateSellingExpenses(queryParams.FromDate.Value, actualToDate);
        var financingExpenses = await CalculateFinancingExpenses(queryParams.FromDate.Value, actualToDate);


        var totalRevenuesAmount = revenuesAmount + revenueReturnsAmount + capitalRevenuesRevenuesAmount + accidentCompensationRevenuesAmount;
        var totalProfitOrLoss = totalRevenuesAmount + operatingExpensesAmount;
        var netIncome = totalProfitOrLoss +
                        administrativeExpenses +
                        generalExpenses +
                        sellingExpenses +
                        financingExpenses;

        return new IncomeStatementResponseModel
        {
            FromDate = queryParams.FromDate.Value.ToString("yyyy-MM-dd"),
            ToDate = actualToDate.ToString("yyyy-MM-dd"),
            RevenuesAmount = revenuesAmount,
            RevenueReturnsAmount = revenueReturnsAmount,
            OperatingExpensesAmount = operatingExpensesAmount,
            AdministrativeExpensesAmount = administrativeExpenses,
            GeneralExpensesAmount = generalExpenses,
            SellingExpensesAmount = sellingExpenses,
            FinancingExpensesAmount = financingExpenses,
            CapitalRevenuesAmount = capitalRevenuesRevenuesAmount,
            AccidentCompensationRevenuesAmount = accidentCompensationRevenuesAmount,
            NetIncome = netIncome,
        };
    }

    [HttpGet("GetGeneralLedgerPdf/{accountId}")]
    [Authorize(Permissions.GeneralLedger.View)]
    public async Task<IActionResult> GetGeneralLedgerPdf(
        int accountId,
        [FromQuery] GetGeneralLedgerByAccountCodeQueryParams queryParams)
    {
        var generalLedger = await FetchGeneralLedger(accountId, queryParams);

        if (generalLedger == null)
        {
            return BadRequest("No results found");
        }

        var generalLedgerInfoDataTable = new DataTable();


        generalLedgerInfoDataTable.Columns.Add("from");
        generalLedgerInfoDataTable.Columns.Add("to");
        generalLedgerInfoDataTable.Columns.Add("accountCode");
        generalLedgerInfoDataTable.Columns.Add("accountName");
        generalLedgerInfoDataTable.Columns.Add("totalBalance");
        generalLedgerInfoDataTable.Columns.Add("totalDebit");
        generalLedgerInfoDataTable.Columns.Add("totalCredit");

        // here
        var generalLedgerInfoRow = generalLedgerInfoDataTable.NewRow();
        generalLedgerInfoRow["from"] = generalLedger.FromDate;
        generalLedgerInfoRow["to"] = generalLedger.ToDate;
        generalLedgerInfoRow["accountCode"] = generalLedger.AccountCode;
        generalLedgerInfoRow["accountName"] = generalLedger.AccountArabicName;
        generalLedgerInfoRow["totalBalance"] = generalLedger.TotalBalance;
        generalLedgerInfoRow["totalDebit"] = generalLedger.TotalDebit;
        generalLedgerInfoRow["totalCredit"] = generalLedger.TotalCredit;


        generalLedgerInfoDataTable.Rows.Add(generalLedgerInfoRow);

        var generalLedgerListDataTable = new DataTable();

        generalLedgerListDataTable.Columns.Add("description");
        generalLedgerListDataTable.Columns.Add("debit");
        generalLedgerListDataTable.Columns.Add("credit");
        generalLedgerListDataTable.Columns.Add("balance");
        generalLedgerListDataTable.Columns.Add("date");


        foreach (var item in generalLedger.Data)
        {
            var generalLedgerListRow = generalLedgerListDataTable.NewRow();
            generalLedgerListRow["description"] = item.Description;
            generalLedgerListRow["debit"] = SetNegativeToParentheses(item.Debit) ;
            generalLedgerListRow["credit"] = SetNegativeToParentheses(item.Credit);
            generalLedgerListRow["balance"] = SetNegativeToParentheses(item.Balance);
            generalLedgerListRow["date"] = item.Date.ToString("yyyy-MM-dd");

            generalLedgerListDataTable.Rows.Add(generalLedgerListRow);
        }

        string mimetype = String.Empty;
        int extension = 1;

        var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\generalLedger.rdlc";
        var pdfReportName = $"{Guid.NewGuid()}.pdf";

        var localReport = new LocalReport(reportPath);
        localReport.AddDataSource("generalLedgerInfo", generalLedgerInfoDataTable);
        localReport.AddDataSource("generalLedgerList", generalLedgerListDataTable);
        var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);


        return File(result.MainStream, "application/pdf", pdfReportName);
    }

    [HttpGet("GetGeneralLedgerExcel/{accountId}")]
    [Authorize(Permissions.GeneralLedger.View)]
    public async Task<IActionResult> GetGeneralLedgerExcel(
        int accountId,
        [FromQuery] GetGeneralLedgerByAccountCodeQueryParams queryParams)
    {
        var generalLedger = await FetchGeneralLedger(accountId, queryParams);

        if (generalLedger == null)
        {
            return BadRequest("No results found");
        }

        var dt = new DataTable();

        dt.Columns.Add("AccountName");
        dt.Columns.Add("AccountCode");
        dt.Columns.Add("Date");
        dt.Columns.Add("Description");
        dt.Columns.Add("Debit");
        dt.Columns.Add("Credit");
        dt.Columns.Add("Balance");


        foreach (var item in generalLedger.Data)
        {
            //DataRow row = dt.NewRow();
            //// row["AccountName"] = generalLedger.Data[0].AccountName;
            //row["AccountCode"] = generalLedger.Data[0];
            //row["Date"] = DateOnly.FromDateTime(item.Date).ToString();
            //row["Description"] = item.Description;
            //row["Debit"] = item.Debit;
            //row["Credit"] = item.Credit;
            //row["Balance"] = item.Balance;
            //dt.Rows.Add(row);
        }

        var excelFileName = $"{Guid.NewGuid()}.xlsx";


        using var wb = new XLWorkbook();
        wb.Worksheets.Add(dt, "Sheet1");

        using var stream = new MemoryStream();

        wb.SaveAs(stream);


        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            excelFileName);
    }


    [HttpGet("GetTrialBalance")]
    [Authorize(Permissions.TrialBalance.View)]
    public async Task<ActionResult<GetTrialBalanceResponse>> GetTrialBalance(
        [FromQuery] FetchTrialBalanceQueryParams queryParams)
    {
        var response = await FetchTrialBalance(queryParams);

        if (response is null)
        {
            return BadRequest("No results found");
        }

        return Ok(response);
    }

    [HttpGet("GetTrialBalancePdf")]
    [Authorize(Permissions.TrialBalance.View)]
    public async Task<IActionResult> GetTrialBalancePdf([FromQuery] FetchTrialBalanceQueryParams queryParams)
    {
        var response = await FetchTrialBalance(queryParams);

        if (response is null)
        {
            return BadRequest("No results found");
        }

        var trialBalanceDataTable = new DataTable();

        trialBalanceDataTable.Columns.Add("accountCode");
        trialBalanceDataTable.Columns.Add("accountName");
        trialBalanceDataTable.Columns.Add("openingDebit");
        trialBalanceDataTable.Columns.Add("openingCredit");
        trialBalanceDataTable.Columns.Add("debit");
        trialBalanceDataTable.Columns.Add("credit");
        trialBalanceDataTable.Columns.Add("endingDebit");
        trialBalanceDataTable.Columns.Add("endingCredit");

        foreach (var trialBalanceQueryResponse in response.TrialBalanceQueryResponseList)
        {
            var trialBalanceRow = trialBalanceDataTable.NewRow();

            trialBalanceRow["accountCode"] = trialBalanceQueryResponse.AccountCode;
            trialBalanceRow["accountName"] = trialBalanceQueryResponse.AccountArabicName;
            trialBalanceRow["openingDebit"] = trialBalanceQueryResponse.OpeningDebit;
            trialBalanceRow["openingCredit"] = trialBalanceQueryResponse.OpeningCredit;
            trialBalanceRow["debit"] = trialBalanceQueryResponse.Debit;
            trialBalanceRow["credit"] = trialBalanceQueryResponse.Credit;
            trialBalanceRow["endingDebit"] = trialBalanceQueryResponse.EndingDebit;
            trialBalanceRow["endingCredit"] = trialBalanceQueryResponse.EndingCredit;
            trialBalanceDataTable.Rows.Add(trialBalanceRow);
        }

        var trialBalanceInfoDataTable = new DataTable();

        trialBalanceInfoDataTable.Columns.Add("fromDate");
        trialBalanceInfoDataTable.Columns.Add("foDate");
        trialBalanceInfoDataTable.Columns.Add("totalOpeningDebit");
        trialBalanceInfoDataTable.Columns.Add("totalOpeningCredit");
        trialBalanceInfoDataTable.Columns.Add("totalDebit");
        trialBalanceInfoDataTable.Columns.Add("totalCredit");
        trialBalanceInfoDataTable.Columns.Add("totalEndingDebit");
        trialBalanceInfoDataTable.Columns.Add("totalEndingCredit");


        var trialBalanceInfoRow = trialBalanceInfoDataTable.NewRow();


        trialBalanceInfoRow["fromDate"] = response.FromDate;
        trialBalanceInfoRow["foDate"] = response.ToDate;
        trialBalanceInfoRow["totalOpeningDebit"] = response.TotalOpeningDebit;
        trialBalanceInfoRow["totalOpeningCredit"] = response.TotalOpeningCredit;
        trialBalanceInfoRow["totalDebit"] = response.TotalDebit;
        trialBalanceInfoRow["totalCredit"] = response.TotalCredit;
        trialBalanceInfoRow["totalEndingDebit"] = response.TotalEndingDebit;
        trialBalanceInfoRow["totalEndingCredit"] = response.TotalEndingCredit;

        trialBalanceInfoDataTable.Rows.Add(trialBalanceInfoRow);


        string mimetype = String.Empty;
        int extension = 1;

        var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\trial-balance.rdlc";
        var pdfReportName = $"{Guid.NewGuid()}.pdf";

        var localReport = new LocalReport(reportPath);
        localReport.AddDataSource("trialBalance", trialBalanceDataTable);
        localReport.AddDataSource("trialBalanceInfo", trialBalanceInfoDataTable);
        var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);


        return File(result.MainStream, "application/pdf", pdfReportName);
    }

    [HttpGet("GetTrialBalanceExcel")]
    [Authorize(Permissions.TrialBalance.View)]
    public async Task<IActionResult> GetTrialBalanceExcel([FromQuery] FetchTrialBalanceQueryParams queryParams)
    {
        var response = await FetchTrialBalance(queryParams);
        
        if (response is null)
        {
            return BadRequest("No results found");
        }
        
         var trialBalanceDataTable = new DataTable();

        trialBalanceDataTable.Columns.Add("accountCode");
        trialBalanceDataTable.Columns.Add("accountName");
        trialBalanceDataTable.Columns.Add("openingDebit");
        trialBalanceDataTable.Columns.Add("openingCredit");
        trialBalanceDataTable.Columns.Add("debit");
        trialBalanceDataTable.Columns.Add("credit");
        trialBalanceDataTable.Columns.Add("endingDebit");
        trialBalanceDataTable.Columns.Add("endingCredit");

        foreach (var trialBalanceQueryResponse in response.TrialBalanceQueryResponseList)
        {
            var trialBalanceRow = trialBalanceDataTable.NewRow();

            trialBalanceRow["accountCode"] = trialBalanceQueryResponse.AccountCode;
            trialBalanceRow["accountName"] = trialBalanceQueryResponse.AccountArabicName;
            trialBalanceRow["openingDebit"] = trialBalanceQueryResponse.OpeningDebit;
            trialBalanceRow["openingCredit"] = trialBalanceQueryResponse.OpeningCredit;
            trialBalanceRow["debit"] = trialBalanceQueryResponse.Debit;
            trialBalanceRow["credit"] = trialBalanceQueryResponse.Credit;
            trialBalanceRow["endingDebit"] = trialBalanceQueryResponse.EndingDebit;
            trialBalanceRow["endingCredit"] = trialBalanceQueryResponse.EndingCredit;
            trialBalanceDataTable.Rows.Add(trialBalanceRow);
        }

        var trialBalanceInfoDataTable = new DataTable();

        trialBalanceInfoDataTable.Columns.Add("fromDate");
        trialBalanceInfoDataTable.Columns.Add("foDate");
        trialBalanceInfoDataTable.Columns.Add("totalOpeningDebit");
        trialBalanceInfoDataTable.Columns.Add("totalOpeningCredit");
        trialBalanceInfoDataTable.Columns.Add("totalDebit");
        trialBalanceInfoDataTable.Columns.Add("totalCredit");
        trialBalanceInfoDataTable.Columns.Add("totalEndingDebit");
        trialBalanceInfoDataTable.Columns.Add("totalEndingCredit");


        var trialBalanceInfoRow = trialBalanceInfoDataTable.NewRow();


        trialBalanceInfoRow["fromDate"] = response.FromDate;
        trialBalanceInfoRow["foDate"] = response.ToDate;
        trialBalanceInfoRow["totalOpeningDebit"] = response.TotalOpeningDebit;
        trialBalanceInfoRow["totalOpeningCredit"] = response.TotalOpeningCredit;
        trialBalanceInfoRow["totalDebit"] = response.TotalDebit;
        trialBalanceInfoRow["totalCredit"] = response.TotalCredit;
        trialBalanceInfoRow["totalEndingDebit"] = response.TotalEndingDebit;
        trialBalanceInfoRow["totalEndingCredit"] = response.TotalEndingCredit;

        trialBalanceInfoDataTable.Rows.Add(trialBalanceInfoRow);


        var mimetype = String.Empty;
        int extension = 1;
        
        
        var excelFileName = $"{Guid.NewGuid()}.xlsx";


        using var wb = new XLWorkbook();
        wb.Worksheets.Add(trialBalanceDataTable, "Sheet1");
        wb.Worksheets.Add(trialBalanceInfoDataTable, "Sheet2");

        using var stream = new MemoryStream();

        wb.SaveAs(stream);


        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            excelFileName);
    }
    
    
    [HttpGet("GetBalanceSheetPdf")]
    [Authorize(Permissions.BalanceSheet.View)]
    public async Task<IActionResult> GetBalanceSheetPdf([FromQuery] FetchTrialBalanceQueryParams queryParams)
    {
        var response = await FetchBalanceSheet(queryParams);

        if (response is null)
        {
            return BadRequest("No results found");
        }

        var balanceSheetAccountsDataTable = new DataTable();
        balanceSheetAccountsDataTable.Columns.Add("name");
        balanceSheetAccountsDataTable.Columns.Add("balance");
        var balanceSheetList = new List<AccountInfo>();
        balanceSheetList.AddRange(response.AssetsAccountsInfoList);
        balanceSheetList.AddRange(response.LiabilitiesAccountsInfoList);
        balanceSheetList.AddRange(response.EquityAccountsInfoList);
        balanceSheetList = balanceSheetList.Where(b => b.Level < 4 && b.Balance != 0).ToList();

        foreach (var item in balanceSheetList)
        {
            var balanceSheetAccountsRow = balanceSheetAccountsDataTable.NewRow();
            balanceSheetAccountsRow["name"] = $"{item.ArabicName} - {item.Code}";
            balanceSheetAccountsRow["balance"] = SetNegativeToParentheses(item.Balance);
            balanceSheetAccountsDataTable.Rows.Add(balanceSheetAccountsRow);
        }


        var balanceSheetInfoDataTable = new DataTable();
        balanceSheetInfoDataTable.Columns.Add("fromDate");
        balanceSheetInfoDataTable.Columns.Add("toDate");
        balanceSheetInfoDataTable.Columns.Add("liability");
        balanceSheetInfoDataTable.Columns.Add("equityLiabilitySum");
        balanceSheetInfoDataTable.Columns.Add("netIncome");
        balanceSheetInfoDataTable.Columns.Add("equityLiabilityNetIncomeSum");

        var balanceSheetInfoRow = balanceSheetInfoDataTable.NewRow();
        balanceSheetInfoRow["fromDate"] = response.FromDate;
        balanceSheetInfoRow["toDate"] = response.ToDate;
        balanceSheetInfoRow["liability"] = SetNegativeToParentheses(response.LiabilitiesAmount);
        balanceSheetInfoRow["equityLiabilitySum"] =
            SetNegativeToParentheses(response.LiabilitiesAmount + response.EquityAmount);
        balanceSheetInfoRow["netIncome"] = SetNegativeToParentheses(response.NetIncome);
        balanceSheetInfoRow["equityLiabilityNetIncomeSum"] =
            SetNegativeToParentheses(response.LiabilitiesAmount + response.EquityAmount + response.NetIncome);
        balanceSheetInfoDataTable.Rows.Add(balanceSheetInfoRow);


        string mimetype = String.Empty;
        int extension = 1;

        var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\balance-sheet.rdlc";
        var pdfReportName = $"{Guid.NewGuid()}.pdf";

        var localReport = new LocalReport(reportPath);
        localReport.AddDataSource("balanceSheetAccounts", balanceSheetAccountsDataTable);
        localReport.AddDataSource("balanceSheetInfo", balanceSheetInfoDataTable);
        var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);

        return File(result.MainStream, "application/pdf", pdfReportName);
    }

    [HttpGet("GetEntryPdf/{id}")]
    [Authorize(Permissions.Entries.View)]
    public async Task<IActionResult> GetEntryPdf(int id)
    {
        var response = await FetchEntry(id);

        if (response is null)
        {
            return BadRequest("No results found");
        }
        
        var entryDataTable = new DataTable();
        
        entryDataTable.Columns.Add("date");
        entryDataTable.Columns.Add("description");
        entryDataTable.Columns.Add("id");
        
        var entryRow = entryDataTable.NewRow();
        
        entryRow["date"] = response.EntryDate.ToString("yyyy-MM-dd");
        entryRow["description"] = response.Description;
        entryRow["id"] = response.Id;
        entryDataTable.Rows.Add(entryRow);
        
        
        var entryDetailsDataTable = new DataTable();
        entryDetailsDataTable.Columns.Add("accountCode");
        entryDetailsDataTable.Columns.Add("accountName");
        entryDetailsDataTable.Columns.Add("credit");
        entryDetailsDataTable.Columns.Add("debit");


        foreach (var entryDetail in response.EntryDetails)
        {
            var entryDetailsRow = entryDetailsDataTable.NewRow();
            entryDetailsRow["accountCode"] = entryDetail.Account.Code;
            entryDetailsRow["accountName"] = entryDetail.Account.ArabicName;
            entryDetailsRow["credit"] = entryDetail.Credit;
            entryDetailsRow["debit"] = entryDetail.Debit;
            entryDetailsDataTable.Rows.Add(entryDetailsRow);
        }
        
        
        var mimetype = string.Empty;
        const int extension = 1;

        var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\entry.rdlc";
        var pdfReportName = $"{Guid.NewGuid()}.pdf";

        var localReport = new LocalReport(reportPath);
        localReport.AddDataSource("entry", entryDataTable);
        localReport.AddDataSource("entryDetails", entryDetailsDataTable);
        var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);

        return File(result.MainStream, "application/pdf", pdfReportName);
    }
    
    [HttpGet("GetIncomeStatementPdf")]
    [Authorize(Permissions.IncomeStatement.View)]
    public async Task<IActionResult> GetIncomeStatementPdf([FromQuery] FetchTrialBalanceQueryParams queryParams)
    {
        var response = await FetchIncomeStatement(queryParams);

        if (response is null)
        {
            return BadRequest("No results found");
        }

        var incomeStatementDataTable = new DataTable();
        incomeStatementDataTable.Columns.Add("fromDate");
        incomeStatementDataTable.Columns.Add("toDate");
        incomeStatementDataTable.Columns.Add("revenuesAmount");
        incomeStatementDataTable.Columns.Add("revenueReturnsAmount");
        incomeStatementDataTable.Columns.Add("operatingExpensesAmount");
        incomeStatementDataTable.Columns.Add("totalProfitOrLoss");
        incomeStatementDataTable.Columns.Add("administrativeAndGeneralExpensesAmount");
        incomeStatementDataTable.Columns.Add("sellingExpensesAmount");
        incomeStatementDataTable.Columns.Add("financingExpensesAmount");
        incomeStatementDataTable.Columns.Add("capitalRevenuesAmount");
        incomeStatementDataTable.Columns.Add("accidentCompensationRevenuesAmount");
        incomeStatementDataTable.Columns.Add("netIncome");


        var incomeStatementRow = incomeStatementDataTable.NewRow();
        incomeStatementRow["fromDate"] = response.FromDate;
        incomeStatementRow["toDate"] = response.ToDate;
        incomeStatementRow["revenuesAmount"] = SetNegativeToParentheses(response.RevenuesAmount);
        incomeStatementRow["revenueReturnsAmount"] = SetNegativeToParentheses(response.RevenueReturnsAmount);
        incomeStatementRow["operatingExpensesAmount"] = SetNegativeToParentheses(response.OperatingExpensesAmount);
        incomeStatementRow["totalProfitOrLoss"] = SetNegativeToParentheses(
            response.RevenuesAmount + response.RevenueReturnsAmount +
            response.OperatingExpensesAmount);
        incomeStatementRow["administrativeAndGeneralExpensesAmount"] =
            SetNegativeToParentheses(response.AdministrativeExpensesAmount + response.GeneralExpensesAmount);
        incomeStatementRow["sellingExpensesAmount"] = SetNegativeToParentheses(response.SellingExpensesAmount);
        incomeStatementRow["financingExpensesAmount"] = SetNegativeToParentheses(response.FinancingExpensesAmount);
        incomeStatementRow["capitalRevenuesAmount"] = SetNegativeToParentheses(response.CapitalRevenuesAmount);
        incomeStatementRow["accidentCompensationRevenuesAmount"] = SetNegativeToParentheses(response.AccidentCompensationRevenuesAmount);
        incomeStatementRow["netIncome"] = SetNegativeToParentheses(response.NetIncome);
        incomeStatementDataTable.Rows.Add(incomeStatementRow);


        string mimetype = String.Empty;
        int extension = 1;

        var reportPath = $"{_webHostEnvironment.WebRootPath}\\reports\\incomeStatement.rdlc";
        var pdfReportName = $"{Guid.NewGuid()}.pdf";

        var localReport = new LocalReport(reportPath);
        localReport.AddDataSource("incomeStatement", incomeStatementDataTable);
        var result = localReport.Execute(RenderType.Pdf, extension, null, mimetype);

        return File(result.MainStream, "application/pdf", pdfReportName);
    }

    [HttpGet("GetBalanceSheet")]
    [Authorize(Permissions.BalanceSheet.View)]
    public async Task<ActionResult<BalanceSheetModel>> GetBalanceSheet(
        [FromQuery][Required] DateTime fromDate, [FromQuery] DateTime? toDate)
    {
        var actualToDate = toDate ?? DateTime.UtcNow.Date;

        var balanceSheet = await FetchBalanceSheet(new FetchTrialBalanceQueryParams()
        {
            FromDate = fromDate,
            ToDate = actualToDate
        });

        if (balanceSheet is null)
        {
            return NotFound();
        }
        
        // todo

        // balanceSheet.LiabilitiesAmount += (await GetAccountByCode(20102));

        return balanceSheet;
    }

    private bool HasMainAccounts(IEnumerable<EntryDetails> entryDetails)
    {
        var accountIds = entryDetails.Select(e => e.AccountId);

        var accounts = _unitOfWork.AccountRepository.All;

        var result = (
            from account in accounts
            where accountIds.Contains(account.Id) && account.IsMain
            select new { }
        ).Any();
        return result;
    }

    private async Task<bool> IsSubAccount(int accountId)
    {
        var account = await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == accountId);
        return account is { IsMain: false };
    }

    private static bool IsBalancedEntry(decimal totalDebit, decimal totalCredit) => totalDebit == totalCredit;

    private bool HasAccountsWithOpeningEntry(IEnumerable<AddEntryDetailsDto> entryDetails)
    {
        var accountsIds = entryDetails.Select(e => e.AccountId);

        var accounts = _unitOfWork.AccountRepository.All;
        var dBEntryDetails = _unitOfWork.EntryDetailsRepository.All;
        var result = (
            from account in accounts
            where accountsIds.Contains(account.Id)
            join dBEntryDetail in dBEntryDetails on account.Id equals dBEntryDetail.AccountId
            where dBEntryDetail.Entry.IsOpening
            select new { }
        ).Any();

        return result;
    }

    private async Task<GetGeneralLedgerByAccountCodeResponse?> FetchGeneralLedger(
        int accountId,
        GetGeneralLedgerByAccountCodeQueryParams queryParams)
    {
        var actualToDate = queryParams.ToDate ?? DateTime.Now;

        var account = await _unitOfWork.AccountRepository.GetFirstOrDefaultAsync(a => a.Id == accountId);

        if (account == null)
        {
            throw new Exception("No Data found");
        }

        var entries = _unitOfWork.EntryRepository.All;
        var entryDetails = _unitOfWork.EntryDetailsRepository.All;
        var accounts = _unitOfWork.AccountRepository.All;


        var query = (
            from entriesTbl in entries
            join entryDetailsTbl in entryDetails
                on entriesTbl.Id equals entryDetailsTbl.EntryId
            join accountsTbl in accounts on entryDetailsTbl.AccountId equals accountsTbl.Id
            where
                entriesTbl.IsPosted
                &&
                entryDetailsTbl.AccountId == accountId
                && entriesTbl.EntryDate.Date >= queryParams.FromDate && entriesTbl.EntryDate.Date <= actualToDate.Date
            orderby entriesTbl.EntryDate
            select
                new GeneralLedgerQueryResponse()
                {
                    Date = entriesTbl.EntryDate,
                    Debit = entryDetailsTbl.Debit,
                    Credit = entryDetailsTbl.Credit,
                    Balance = entryDetailsTbl.Debit - entryDetailsTbl.Credit,
                    Description = entriesTbl.Description,
                    EntryId = entriesTbl.Id
                });

        GeneralLedgerQueryResponse.BalanceAccumulator = 0;
        var data = await query.ToListAsync();

        if (!data.Any())
        {
            throw new Exception("No Data found");
        }

        return new GetGeneralLedgerByAccountCodeResponse()
        {
            Data = data,
            TotalDebit = query.Sum(x => x.Debit),
            TotalCredit = query.Sum(x => x.Credit),
            TotalBalance = data.Last().Balance,
            FromDate = queryParams.FromDate.Value.ToString("yyyy-MM-dd"),
            ToDate = actualToDate.ToString("yyyy-MM-dd"),
            AccountArabicName = account.ArabicName,
            AccountEnglishName = account.EnglishName,
            AccountCode = account.Code,
            AccountId = account.Id
           
        };
    }

    private async Task<GetTrialBalanceResponse?> FetchTrialBalance(FetchTrialBalanceQueryParams queryParams)
    {
        var actualToDate = queryParams.ToDate ?? DateTime.Now;

        var entries = _unitOfWork.EntryRepository.All;
        var entryDetails = _unitOfWork.EntryDetailsRepository.All;
        var accounts = _unitOfWork.AccountRepository.All;

        var query = (
            from entriesTbl in entries
            join entryDetailsTbl in entryDetails on entriesTbl.Id equals entryDetailsTbl.EntryId
            join accountsTbl in accounts on entryDetailsTbl.AccountId equals accountsTbl.Id
            where entriesTbl.EntryDate.Date >= queryParams.FromDate.Value.Date &&
                  entriesTbl.EntryDate.Date <= actualToDate.Date && entriesTbl.IsPosted
            group entryDetailsTbl by new
            { accountsTbl.Code, accountsTbl.EnglishName, accountsTbl.ArabicName, accountsTbl.Id }
            into grouped
            select new TrialBalanceQueryResponse()
            {
                AccountCode = grouped.Key.Code,
                AccountEnglishName = grouped.Key.EnglishName,
                AccountArabicName = grouped.Key.ArabicName,
                Debit = grouped.Sum(e => e.Entry.IsOpening ? 0 : e.Debit),
                Credit = grouped.Sum(e => e.Entry.IsOpening ? 0 : e.Credit),
                OpeningDebit = grouped.Sum(e => e.Entry.IsOpening ? e.Debit : 0),
                OpeningCredit = grouped.Sum(e => e.Entry.IsOpening ? e.Credit : 0)
            }
        );

        query = query
            .OrderBy(x => x.AccountCode.ToString().Substring(0, 1))
            .ThenBy(x => x.AccountCode);

        var trialBalanceQueryResponseList = await query.ToListAsync();


        if (!trialBalanceQueryResponseList.Any())
        {
            return null;
        }

        var totalOpeningDebit = trialBalanceQueryResponseList.Sum(x => x.OpeningDebit);
        var totalOpeningCredit = trialBalanceQueryResponseList.Sum(x => x.OpeningCredit);
        var totalDebit = trialBalanceQueryResponseList.Sum(x => x.Debit);
        var totalCredit = trialBalanceQueryResponseList.Sum(x => x.Credit);
        var totalEndingDebit = trialBalanceQueryResponseList.Sum(x => x.EndingDebit);
        var totalEndingCredit = trialBalanceQueryResponseList.Sum(x => x.EndingCredit);

        var response = new GetTrialBalanceResponse
        {
            TrialBalanceQueryResponseList = trialBalanceQueryResponseList,
            TotalOpeningDebit = totalOpeningDebit,
            TotalOpeningCredit = totalOpeningCredit,
            TotalDebit = totalDebit,
            TotalCredit = totalCredit,
            TotalEndingDebit = totalEndingDebit,
            TotalEndingCredit = totalEndingCredit,
            FromDate = queryParams.FromDate!.Value.ToString("yyyy-MM-dd"),
            ToDate = actualToDate.ToString("yyyy-MM-dd")
        };
        return response;
    }

    private async Task<decimal> CalculateRevenues(DateTime fromDate, DateTime toDate)
    {
        // here
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                // ed.Account.AccountTypeId == (int)AccountTypeEnum.Revenue &&
                // ed.Account.AccountCategoryId != (int)AccountCategoryEnum.RevenueReturns &&
                ed.Account.Code.ToString().StartsWith($"{(int)AccountTypeEnum.Revenue}") &&
                !RevenuesBlackList.Contains(ed.Account.Code) &&
                ed.Account.Code != (long)AccountCategoryEnum.RevenueReturns &&
                ed.Account.Code != (long)AccountCategoryEnum.CapitalRevenue &&
                ed.Account.Code != (long)AccountCategoryEnum.AccidentCompensationRevenue &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateRevenueReturns(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                ed.Account.Code == (long)AccountCategoryEnum.RevenueReturns &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateCapitalRevenues(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                ed.Account.Code == (long)AccountCategoryEnum.CapitalRevenue &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateAccidentCompensationRevenues(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                ed.Account.Code == (long)AccountCategoryEnum.AccidentCompensationRevenue &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateOperatingExpenses(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                ed.Account.Code.ToString().StartsWith($"{(int)AccountCategoryEnum.OperatingExpenses}") &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateAdministrativeExpenses(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                // ed.Account.AccountTypeId == (int)AccountTypeEnum.Expense &&
                // ed.Account.AccountCategoryId == (int)AccountCategoryEnum.OperatingExpenses &&
                ed.Account.Code.ToString().StartsWith($"{(int)AccountCategoryEnum.AdministrativeExpenses}") &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateGeneralExpenses(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                // ed.Account.AccountTypeId == (int)AccountTypeEnum.Expense &&
                // ed.Account.AccountCategoryId == (int)AccountCategoryEnum.OperatingExpenses &&
                ed.Account.Code.ToString().StartsWith($"{(int)AccountCategoryEnum.GeneralExpenses}") &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateSellingExpenses(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                // ed.Account.AccountTypeId == (int)AccountTypeEnum.Expense &&
                // ed.Account.AccountCategoryId == (int)AccountCategoryEnum.OperatingExpenses &&
                ed.Account.Code.ToString().StartsWith($"{(int)AccountCategoryEnum.SellingExpenses}") &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateFinancingExpenses(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                // ed.Account.AccountTypeId == (int)AccountTypeEnum.Expense &&
                // ed.Account.AccountCategoryId == (int)AccountCategoryEnum.OperatingExpenses &&
                ed.Account.Code.ToString().StartsWith($"{(int)AccountCategoryEnum.FinancingExpenses}") &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<BalanceSheetModel?> FetchBalanceSheet(
        FetchTrialBalanceQueryParams queryParams)
    {
        var actualToDate = queryParams.ToDate ?? DateTime.UtcNow.Date;

        var equityAmount = await CalculateEquity(queryParams.FromDate.Value, actualToDate);

        var assetsAccountsInfoList = await FetchAssetsAccountsInfoList(queryParams.FromDate.Value, actualToDate);
        var liabilitiesAccountsInfoList =
            await FetchLiabilitiesAccountsInfoList(queryParams.FromDate.Value, actualToDate);
        var equityAccountsInfoList = await FetchEquityAccountsInfoList(queryParams.FromDate.Value, actualToDate);

        if (assetsAccountsInfoList == null || liabilitiesAccountsInfoList == null || equityAccountsInfoList == null)
        {
            return null;
        }

        return new BalanceSheetModel
        {
            AssetsAmount = assetsAccountsInfoList.Single(x => x.Code == 1).Balance,
            LiabilitiesAmount = liabilitiesAccountsInfoList.Single(x => x.Code == 2).Balance,
            EquityAmount = equityAmount,
            AssetsAccountsInfoList = assetsAccountsInfoList,
            LiabilitiesAccountsInfoList = liabilitiesAccountsInfoList,
            EquityAccountsInfoList = equityAccountsInfoList,
            FromDate = queryParams.FromDate.Value.ToString("yyyy-MM-dd"),
            ToDate = actualToDate.ToString("yyyy-MM-dd"),
            NetIncome = await CalculateNetIncome(queryParams.FromDate.Value, actualToDate)
        };
    }

    private async Task<List<AccountInfo>?> FetchAssetsAccountsInfoList(DateTime fromDate,
        DateTime toDate)
    {
        var accounts = _unitOfWork
            .AccountRepository
            .All
            // .Where(x => 
            //     x.AccountTypeId == (int)AccountTypeEnum.Asset
            //     );
            .Where(a => a.Code.ToString().StartsWith($"{(int)AccountTypeEnum.Asset}"));


        var dBEntryDetails = _unitOfWork
            .EntryDetailsRepository.All.Where(x =>
                x.Entry.IsPosted &&
                x.Entry.EntryDate.Date >= fromDate.Date &&
                x.Entry.EntryDate.Date <= toDate.Date);

        var query = from account in accounts
                    join dBEntryDetail in dBEntryDetails
                        on account.Id equals dBEntryDetail.AccountId
                        into aGroup
                    from d in aGroup.DefaultIfEmpty()
                    select new AccountInfo
                    {
                        Code = account.Code,
                        EnglishName = account.EnglishName,
                        ArabicName = account.ArabicName,
                        Balance = (d != null) ? (d.Debit - d.Credit) : 0,
                        Level = account.AccountLevel,
                        IsMain = account.IsMain
                    };

        var groupedQuery = from account in query
                           group account by new
                           {
                               account.EnglishName,
                               account.ArabicName,
                               account.Code,
                               account.Level,
                               account.IsMain
                           }
            into grouped
                           select new AccountInfo
                           {
                               Code = grouped.Key.Code,
                               EnglishName = grouped.Key.EnglishName,
                               ArabicName = grouped.Key.ArabicName,
                               Balance = grouped.Sum(a => a.Balance),
                               Level = grouped.Key.Level,
                               IsMain = grouped.Key.IsMain
                           };

        var groupedAccountInfos = groupedQuery.ToList();


        foreach (var groupedAccountInfo in groupedAccountInfos)
        {
            if (groupedAccountInfo.Level == 1)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x => x.Balance);
            }
            else if (groupedAccountInfo.Level == 2 && groupedAccountInfo.IsMain)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x =>
                        x.Code.ToString().StartsWith(groupedAccountInfo.Code.ToString())
                            ? x.Balance
                            : 0);
            }
            else if (groupedAccountInfo.Level == 3 && groupedAccountInfo.IsMain)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x =>
                        x.Code.ToString().StartsWith(groupedAccountInfo.Code.ToString())
                            ? x.Balance
                            : 0);
            }
        }

        return groupedAccountInfos;
    }

    private async Task<List<AccountInfo>?> FetchLiabilitiesAccountsInfoList(DateTime fromDate,
        DateTime toDate)
    {
        var accounts = _unitOfWork
            .AccountRepository
            .All
            // .Where(x => x.AccountTypeId == (int)AccountTypeEnum.Liability);
            .Where(a => a.Code.ToString().StartsWith($"{(int)AccountTypeEnum.Liability}"));

        var dBEntryDetails = _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(x =>
                x.Entry.IsPosted &&
                x.Entry.EntryDate.Date >= fromDate.Date &&
                x.Entry.EntryDate.Date <= toDate.Date);

        var query = from account in accounts
                    join dBEntryDetail in dBEntryDetails
                        on account.Id equals dBEntryDetail.AccountId
                        into aGroup
                    from d in aGroup.DefaultIfEmpty()
                    select new AccountInfo
                    {
                        Code = account.Code,
                        EnglishName = account.EnglishName,
                        ArabicName = account.ArabicName,
                        Balance = (d != null) ? (d.Debit - d.Credit) : 0,
                        Level = account.AccountLevel,
                        IsMain = account.IsMain
                    };

        var groupedQuery = from account in query
                           group account by new
                           {
                               account.EnglishName,
                               account.ArabicName,
                               account.Code,
                               account.Level,
                               account.IsMain
                           }
            into grouped
                           select new AccountInfo
                           {
                               Code = grouped.Key.Code,
                               EnglishName = grouped.Key.EnglishName,
                               ArabicName = grouped.Key.ArabicName,
                               Balance = grouped.Sum(a => a.Balance),
                               Level = grouped.Key.Level,
                               IsMain = grouped.Key.IsMain
                           };

        var groupedAccountInfos = groupedQuery.ToList();

        foreach (var groupedAccountInfo in groupedAccountInfos)
        {
            if (groupedAccountInfo.Level == 1)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x => x.Balance);
            }
            else if (groupedAccountInfo.Level == 2 && groupedAccountInfo.IsMain)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x =>
                        x.Code.ToString().StartsWith(groupedAccountInfo.Code.ToString())
                            ? x.Balance
                            : 0);
            }
            else if (groupedAccountInfo.Level == 3 && groupedAccountInfo.IsMain)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x =>
                        x.Code.ToString().StartsWith(groupedAccountInfo.Code.ToString())
                            ? x.Balance
                            : 0);
            }
        }

        return groupedAccountInfos;
    }

    private async Task<List<AccountInfo>?> FetchEquityAccountsInfoList(DateTime fromDate,
        DateTime toDate)
    {
        var accounts = _unitOfWork
            .AccountRepository
            .All
            // .Where(x => x.AccountTypeId == (int)AccountTypeEnum.Equity);
            .Where(a => a.Code.ToString().StartsWith($"{(int)AccountTypeEnum.Equity}"));

        var dBEntryDetails = _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(x =>
                x.Entry.IsPosted &&
                x.Entry.EntryDate.Date >= fromDate.Date &&
                x.Entry.EntryDate.Date <= toDate.Date);

        var query = from account in accounts
                    join dBEntryDetail in dBEntryDetails
                        on account.Id equals dBEntryDetail.AccountId
                        into aGroup
                    from d in aGroup.DefaultIfEmpty()
                    select new AccountInfo
                    {
                        Code = account.Code,
                        EnglishName = account.EnglishName,
                        ArabicName = account.ArabicName,
                        Balance = (d != null) ? (d.Debit - d.Credit) : 0,
                        Level = account.AccountLevel,
                        IsMain = account.IsMain,
                    };

        var groupedQuery = from account in query
                           group account by new
                           {
                               account.EnglishName,
                               account.ArabicName,
                               account.Code,
                               account.Level,
                               account.IsMain,
                           }
            into grouped
                           select new AccountInfo
                           {
                               Code = grouped.Key.Code,
                               EnglishName = grouped.Key.EnglishName,
                               ArabicName = grouped.Key.ArabicName,
                               Balance = grouped.Sum(a => a.Balance),
                               Level = grouped.Key.Level,
                               IsMain = grouped.Key.IsMain,
                           };

        var groupedAccountInfos = groupedQuery.ToList();

        var currentYearProfitOrLoss =
            await _unitOfWork.AccountRepository
                .All.FirstAsync(x => x.Code == (long)AccountCategoryEnum.CurrentYearProfitOrLoss);
        // .SingleOrDefaultAsync(x => x.AccountCategoryId
        //                            == (int)AccountCategoryEnum.CurrentYearProfitOrLoss);


        var currentYearProfitOrLossAccountInfo = groupedAccountInfos.First(x =>
            x.Code == (int)AccountCategoryEnum.CurrentYearProfitOrLoss);

        var currentYearProfitOrLossAccountInfoIndex = groupedAccountInfos.IndexOf(currentYearProfitOrLossAccountInfo);


        var netIncome = await CalculateNetIncome(fromDate, toDate);

        groupedAccountInfos[currentYearProfitOrLossAccountInfoIndex] = new AccountInfo
        {
            IsMain = true,
            EnglishName = currentYearProfitOrLoss.EnglishName,
            ArabicName = currentYearProfitOrLoss.ArabicName,
            Balance = netIncome,
            Code = currentYearProfitOrLoss.Code,
            Level = currentYearProfitOrLoss.AccountLevel
        };

        groupedAccountInfos = groupedAccountInfos.OrderBy(x => x.Code).ToList();

        foreach (var groupedAccountInfo in groupedAccountInfos)
        {
            if (groupedAccountInfo.Level == 1)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x => x.Balance);
            }
            else if (groupedAccountInfo.Level == 2 && groupedAccountInfo.IsMain)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x =>
                        x.Code.ToString().StartsWith(groupedAccountInfo.Code.ToString())
                            ? x.Balance
                            : 0);
            }
            else if (groupedAccountInfo.Level == 3 && groupedAccountInfo.IsMain)
            {
                groupedAccountInfo.Balance =
                    groupedAccountInfos.Sum(x =>
                        x.Code.ToString().StartsWith(groupedAccountInfo.Code.ToString())
                            ? x.Balance
                            : 0);
            }
        }

        return groupedAccountInfos;
    }

    private async Task<decimal> CalculateNetIncome(DateTime fromDate,
        DateTime toDate)
    {
        var revenuesAmount = await CalculateRevenues(fromDate, toDate);
        var revenueReturnsAmount = await CalculateRevenueReturns(fromDate, toDate);
        var capitalRevenuesRevenuesAmount = await CalculateCapitalRevenues(fromDate, toDate);
        var accidentCompensationRevenuesAmount = await CalculateAccidentCompensationRevenues(fromDate, toDate);

        var operatingExpensesAmount = await CalculateOperatingExpenses(fromDate, toDate);
        var administrativeExpenses = await CalculateAdministrativeExpenses(fromDate, toDate);
        var generalExpenses = await CalculateGeneralExpenses(fromDate, toDate);
        var sellingExpenses = await CalculateSellingExpenses(fromDate, toDate);
        var financingExpenses = await CalculateFinancingExpenses(fromDate, toDate);


        var totalRevenuesAmount = revenuesAmount +
                                  revenueReturnsAmount +
                                  capitalRevenuesRevenuesAmount +
                                  accidentCompensationRevenuesAmount;
        var totalProfitOrLoss = totalRevenuesAmount + operatingExpensesAmount;
        var netIncome = totalProfitOrLoss +
                        administrativeExpenses +
                        generalExpenses +
                        sellingExpenses +
                        financingExpenses;

        return netIncome;
    }

    private async Task<decimal> CalculateAssets(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                // ed.Account.AccountTypeId == (int)AccountTypeEnum.Asset &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateLiabilities(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                // ed.Account.AccountTypeId == (int)AccountTypeEnum.Liability &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<decimal> CalculateEquity(DateTime fromDate, DateTime toDate)
    {
        var result = await _unitOfWork
            .EntryDetailsRepository
            .All
            .Where(ed =>
                ed.Account.Code.ToString().StartsWith($"{(int)AccountTypeEnum.Equity}") &&
                ed.Entry.IsPosted &&
                ed.Entry.EntryDate.Date >= fromDate.Date &&
                ed.Entry.EntryDate.Date <= toDate.Date)
            .Select(ed => new
            {
                ed.Debit,
                ed.Credit
            })
            .ToListAsync();

        var totalDebit = result.Sum(r => r.Debit);
        var totalCredit = result.Sum(r => r.Credit);


        return totalDebit - totalCredit;
    }

    private async Task<bool> UpdateProfitOrLossAccount(DateTime fromDate, DateTime toDate)
    {
        var currentYearProfitOrLoss =
            await _unitOfWork.AccountRepository.All
                .SingleOrDefaultAsync(x => x.Code == (long)AccountCategoryEnum.CurrentYearProfitOrLoss);
        var netIncome = await CalculateNetIncome(fromDate, toDate);

        currentYearProfitOrLoss.Debit = netIncome;
        currentYearProfitOrLoss.Credit = 0;


        _unitOfWork.AccountRepository.Update(currentYearProfitOrLoss);

        return await _unitOfWork.SaveAsync();
    }

    private string SetNegativeToParentheses(decimal number)
    {
        return number >= 0 ? number.ToString() : $"({Math.Abs(number)})";
    }

    private async Task<Entry?> FetchEntry(int id)
    {
        var entry = await _unitOfWork.EntryRepository.GetFirstOrDefaultAsync(
            e => e.Id == id,
            s => s.Include(e => e.EntryDetails)
                .ThenInclude(entryDetail => entryDetail.Account));

        return entry;
    }

    [HttpPost("ClosePeriod")]
    [Authorize(Permissions.Entries.Add)]
    public async Task<IActionResult> ClosePeriod(DateTime from, DateTime to)
    {
        var netIncome = await CalculateNetIncome(from, to);
        
        // Create closing entries for revenue and expense accounts, moving them to the Profit and Loss account.
        var profitAndLossAccount = await GetAccountByCode((long)AccountCategoryEnum.CurrentYearProfitOrLoss); // Replace with your actual logic to get this account.
        
        var revenueAccounts = await _unitOfWork.AccountRepository.All.Where(a => a.Code.ToString().StartsWith("4") && !a.IsMain).ToListAsync();
        var expenseAccounts = await _unitOfWork.AccountRepository.All.Where(a => a.Code.ToString().StartsWith("5") && !a.IsMain).ToListAsync();
        
        var closingEntries = new List<Entry>();
        
        closingEntries.AddRange(CreateClosingEntriesForAccounts(revenueAccounts, true, profitAndLossAccount.Id));
        closingEntries.AddRange(CreateClosingEntriesForAccounts(expenseAccounts, false, profitAndLossAccount.Id));
        
        // Step 3: Close the Profit and Loss account to Retained Earnings.
        var retainedEarningsAccount = await GetAccountByCode((long)AccountCategoryEnum.RetainedEarningsAndLosses);
        
        var closingEntryForProfitAndLoss = new Entry
        {
            Description = "Close Profit and Loss to Retained Earnings",
            EntryDate = DateTime.UtcNow,
            IsPosted = true,
            EntryDetails = new List<EntryDetails>
            {
                new EntryDetails
                {
                    AccountId = profitAndLossAccount.Id,
                    Debit = netIncome < 0 ? -netIncome : 0, // If net loss, debit the profit and loss account
                    Credit = netIncome > 0 ? netIncome : 0, // If net profit, credit the profit and loss account
                    Description = "Closing Profit and Loss"
                },
                new EntryDetails
                {
                    AccountId = retainedEarningsAccount.Id,
                    Debit = netIncome > 0 ? netIncome : 0, // If net profit, debit the retained earnings account
                    Credit = netIncome < 0 ? -netIncome : 0, // If net loss, credit the retained earnings account
                    Description = "Closing to Retained Earnings"
                }
            }
        };
        
        closingEntries.Add(closingEntryForProfitAndLoss);
        
        foreach (var entry in closingEntries)
        {
            _unitOfWork.EntryRepository.Add(entry);
        }

        var isSuccess = await _unitOfWork.SaveAsync();
    
        if (!isSuccess)
        {
            throw new Exception("Closing period failed");
        }

        return Ok();
    }

    private async Task<Account?> GetAccountByCode(long accountCode)
    {
        return await _unitOfWork
            .AccountRepository
            .GetFirstOrDefaultAsync(x => x.Code == accountCode,
                include: s => s.Include(a => a.SubAccounts));
    }
    
    
    private List<Entry> CreateClosingEntriesForAccounts(List<Account> accounts, bool isRevenue, int profitAndLossAccountId)
    {
        var entries = new List<Entry>();
        foreach (var account in accounts)
        {
            var amountToClose = isRevenue ? account.Credit - account.Debit : account.Debit - account.Credit;
            var entry = new Entry
            {
                Description = $"Close {account.EnglishName}",
                EntryDate = DateTime.UtcNow,
                IsPosted = true,
                EntryDetails = new List<EntryDetails>
                {
                    new EntryDetails
                    {
                        AccountId = account.Id,
                        Debit = isRevenue ? 0 : amountToClose, // Debit expenses or credit revenues to zero out
                        Credit = isRevenue ? amountToClose : 0, // Credit revenues or debit expenses to zero out
                        Description = "Closing Entry"
                    },
                    new EntryDetails
                    {
                        AccountId = profitAndLossAccountId,
                        Debit = isRevenue ? amountToClose : 0, // Debit the profit and loss account by the revenue amount
                        Credit = isRevenue ? 0 : amountToClose, // Credit the profit and loss account by the expense amount
                        Description = "To Profit and Loss"
                    }
                }
            };
            entries.Add(entry);
        }
        return entries;
    }
    
    // private bool EntryHasCustomersAccounts(IEnumerable<EntryDetails> entryDetails)
    // {
    //     var accountIds = entryDetails.Select(e => e.AccountId);
    //
    //     var accounts = _unitOfWork.AccountRepository.All;
    //
    //     var hasCustomersAccounts = (
    //         from account in accounts
    //         where accountIds.Contains(account.Id) && account.Code.ToString().StartsWith("10101")
    //         select new { }
    //     ).Any();
    //
    //     return hasCustomersAccounts;
    // }
    //
    // private  async Task<decimal> HandleCustomersEntries(IEnumerable<EntryDetails> entryDetails)
    // {
    //     var accountIds = entryDetails.Select(e => e.AccountId);
    //
    //     var accounts = _unitOfWork.AccountRepository.All;
    //
    //     var pendingAddAmountToSalesVatAccount = 0m;
    //     
    //     var customersAccountIds = await (
    //         from account in accounts
    //         where accountIds.Contains(account.Id) && account.Code.ToString().StartsWith("10101")
    //         select  account.Id
    //     ).ToListAsync();
    //
    //     foreach (var entryDetail in entryDetails)
    //     {
    //         if (customersAccountIds.Contains(entryDetail.AccountId))
    //         {
    //             var creditAmount = entryDetail.Credit;
    //             var creditAmountWithoutTax = creditAmount / 1.15m;
    //             var creditTaxAmount = creditAmount - creditAmountWithoutTax;
    //
    //             entryDetail.Credit = creditAmountWithoutTax;
    //
    //             pendingAddAmountToSalesVatAccount += creditTaxAmount;
    //         }
    //     }
    //
    //     return pendingAddAmountToSalesVatAccount;
    // }
    //
    // private decimal CalculateSalesVatFromEntries(DateTime fromDate, DateTime toDate)
    // {
    //     var entries = _unitOfWork.EntryRepository.All;
    //     var entryDetails = _unitOfWork.EntryDetailsRepository.All;
    //     var accounts = _unitOfWork.AccountRepository.All.Where(x => x.Code.ToString().StartsWith("10101"));
    //     
    //     var sum = (
    //         from entriesTbl in entries
    //         join entryDetailsTbl in entryDetails
    //             on entriesTbl.Id equals entryDetailsTbl.EntryId
    //         join accountsTbl in accounts on entryDetailsTbl.AccountId equals accountsTbl.Id
    //         where
    //             entriesTbl.IsPosted
    //             && entriesTbl.EntryDate.Date >= fromDate.Date && entriesTbl.EntryDate.Date <= toDate.Date
    //         orderby entriesTbl.EntryDate
    //         select entryDetailsTbl.Credit).Sum();
    //
    //     return sum;
    // }
}