using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace WorldCitiesAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CountriesController: ControllerBase
  {
    private readonly ApplicationDbContext _context;

    private readonly ILogger<CountriesController> _logger;

    public CountriesController(ILogger<CountriesController> logger, ApplicationDbContext context)
    { 
        _context = context;
        _logger = logger;
        _logger.LogInformation("CountriesController initialized");
    }

    // GET: api/countries
    [HttpGet]
    public async Task<ActionResult<ApiResult<Country>>> GetCountries(
        int pageIndex = 0, 
        int pageSize = 10,
        string? sortColumn = null, 
        string? sortOrder = null, 
        string? filterColumn = null, 
        string? filterQuery = null)
    {
      return await ApiResult<Country>.CreateAsync(
          _context.Countries.AsNoTracking()
            .Include(c => c.Cities)
            .Select(c => new Country(){
                Id = c.Id, 
                Name = c.Name,
                ISO2 = c.ISO2,
                ISO3 = c.ISO3, 
                TotCities = c.Cities!.Count
            }),
          pageIndex,
          pageSize,
          sortColumn,
          sortOrder,
          filterColumn,
          filterQuery);
    }

    // GET: api/countries
    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCountry(int id)
    {
        var country = await _context.Countries.FindAsync(id);

        if (country == null) return NotFound();

        return country;
    }

    // PUT: api/Cities/5
    [Authorize(Roles ="RegisteredUser")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, Country country)
    {
      if (id != country.Id)
      { 
        return BadRequest();
      }

      _context.Entry(country).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
           
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!_context.Countries.Any(c => c.Id == id))
        {
            return NotFound(nameof(Country));
        }
        else
        {
            throw;
        }
      }
      return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }


    // POST: api/Countries
    [Authorize(Roles = "RegisteredUser")]
    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(Country country)
    { 
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    //
    [HttpPost]
    [Route("IsDupeField")]
    public bool IsDupeField(
        int countryId, 
        string fieldName,
        string fieldValue
      )
    {
      //switch (fieldName)
      //{
      //  case "name":
      //    return _context.Countries.Any(c => c.Name == fieldValue && c.Id != countryId);
      //  case "iso2":
      //    return _context.Countries.Any(c => c.ISO2 == fieldValue && c.Id != countryId);
      //  case "iso3":
      //    return _context.Countries.Any(c => c.ISO3 == fieldValue && c.Id != countryId);
      //  default:
      //    return false;
      //}
      return (ApiResult<Country>.IsValidProperty(fieldName, true))
            ? _context.Countries.Any(
                string.Format("{0} == @0 && Id != @1", fieldName),
                fieldValue,
                countryId)
            : false;
    }


    // DELETE: api/Countries/5
    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null) return NotFound();

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();

        return Ok();
    }

  }
}
