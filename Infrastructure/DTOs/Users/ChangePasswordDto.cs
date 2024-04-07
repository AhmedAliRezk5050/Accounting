
namespace Infrastructure.DTOs.Users;

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = null!;
    
    public string NewPassword { get; set; } = null!;
}