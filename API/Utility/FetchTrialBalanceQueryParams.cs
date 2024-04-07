using System.ComponentModel.DataAnnotations;

namespace API.Utility;

public class FetchTrialBalanceQueryParams
{
    [Required] public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}