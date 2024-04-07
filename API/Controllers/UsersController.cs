using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using Infrastructure.DTOs.Users;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[Authorize]
public class UsersController : ApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [Authorize(Permissions.Users.Add)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.UserName
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            return Ok();
        }

        throw new Exception("Register failed");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _signInManager
            .PasswordSignInAsync(loginDto.UserName,
                loginDto.Password,
                isPersistent: false,
                lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var token = await GenerateJwtToken(loginDto.UserName);

            return Ok(new { Token = token });
        }

        throw new Exception("Login failed");
    }

    private async Task<string> GenerateJwtToken(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    [HttpGet("GetUserPermissions/{userId}")]
    [Authorize(Permissions.UserPermissions.View)]
    public async Task<IActionResult> GetUserPermissions(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var userClaims = (await _userManager.GetClaimsAsync(user)).Select(x => x.Value);

        return Ok(userClaims);
    }


    [HttpPost("SetUserPermissions/{userId}")]
    [Authorize(Permissions.UserPermissions.Edit)]
    public async Task<IActionResult> SetUserPermissions(string userId, [FromBody] List<string> updatedUserPermissions)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var userClaims = await _userManager.GetClaimsAsync(user);


        await _userManager.RemoveClaimsAsync(user, userClaims.Where(c => c.Type == "Permission"));


        var allPermissions = Permissions.GetAllPermissions();

        if (updatedUserPermissions.Distinct().Count() != updatedUserPermissions.Count ||
            !updatedUserPermissions.All(p => allPermissions.Contains(p)))
        {
            throw new Exception("invalid permissions.");
        }

        var userNewClaims = updatedUserPermissions
            .Select(value => new Claim("Permission", value))
            .ToList();

        var addClaimsResult = await _userManager.AddClaimsAsync(user, userNewClaims);

        if (addClaimsResult.Succeeded)
        {
            return Ok();
        }

        throw new Exception("Adding claims failed");
    }

    [HttpGet("GetUsers")]
    [Authorize(Permissions.UserPermissions.View)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager
            .Users
            .Select(u => new UserDetails
            {
                Id = u.Id,
                UserName = u.UserName
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("GetUser/{id}")]
    [Authorize(Permissions.UserPermissions.View)]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager
            .Users
            .Select(x => new UserDetails
            {
                Id = x.Id,
                UserName = x.UserName
            })
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        var claimDetailsList = (await _userManager
                .GetClaimsAsync(new ApplicationUser { Id = id }))
            .Select(x => new ClaimDetails
            {
                Value = x.Value
            }).ToList();

        user.ClaimDetailsList.AddRange(claimDetailsList);

        return Ok(user);
    }

    [HttpGet("GetAllPermissions")]
    [Authorize(Permissions.UserPermissions.View)]
    public IActionResult GetAllPermissions()
    {
        var allPermissions = Permissions
            .GetAllPermissions();
        return Ok(allPermissions);
    }
    
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var applicationUser = await FetchUser(_userManager.GetUserId(User));

        if (applicationUser == null)
        {
            return Unauthorized();
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(
            applicationUser,
            dto.OldPassword,
            dto.NewPassword);

        if (changePasswordResult.Succeeded)
        {
            return Ok();
        }
        
        return BadRequest();
    }

    private async Task<ApplicationUser?> FetchUser(string userId)
    {
        var user = await _userManager
            .Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        return user;
    }
}