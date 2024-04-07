namespace API.Utility;

public static class Helpers
{
    public static string? GetRequestHeader(HttpRequest request, string key)
    {
        request.Headers.TryGetValue(key, out var headerValue);
        return headerValue.FirstOrDefault();
    }
}