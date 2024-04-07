namespace Infrastructure.DTOs.Users;

public class LoginDto
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}