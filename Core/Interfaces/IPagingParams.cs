namespace Core.Interfaces;

public interface IPagingParams
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}