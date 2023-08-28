using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Data
{
  public class JwtHandler
  {
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtHandler(IConfiguration configuration, UserManager<ApplicationUser> userManager)
    { 
      _configuration = configuration;
      _userManager = userManager;
    }
  }
}
