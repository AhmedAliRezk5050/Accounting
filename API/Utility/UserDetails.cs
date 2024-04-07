namespace API.Utility;

public class UserDetails
{
    public string Id { get; set; } = null!;
    public string UserName { get; set; } = null!;

    public List<ClaimDetails> ClaimDetailsList { get; set; } = new ();
}