using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Immutable;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<CitiesController> _logger = null!;

        public CitiesController(ILogger<CitiesController> logger, ApplicationDbContext context) {
            _context = context;
            _logger = logger;
            _logger.LogInformation("CitiesController initialized.");
        }

        public CitiesController(ApplicationDbContext context)
        {
          _context = context;
        }

        // GET: api/Cities
        // GET: api/Cities/?pageIndex=0&pageSize=10
        // GET: api/Cities/?pageIndex=0&pageSize=10&sortColumn=name&
        [HttpGet]
        public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(
            int pageIndex = 0, 
            int pageSize = 10, 
            string? sortColumn = null, 
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            return await ApiResult<CityDTO>.CreateAsync(
                _context.Cities.AsNoTracking().Select(c =>  new CityDTO() {
                  Id = c.Id,
                  Name = c.Name,
                  Lat = c.Lat,
                  Lon = c.Lon,
                  CountryId = c.CountryId,
                  CountryName = c.Country!.Name
                }), 
                pageIndex, 
                pageSize, 
                sortColumn, 
                sortOrder,
                filterColumn,
                filterQuery);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // PUT: api/Cities/5
        // How to protect from overpositng attacks
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
               return BadRequest(); 
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cities.Any(c => c.Id == id))
                {
                    return NotFound(nameof(City));
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("IsDupeCity")]
        public bool IsDupeCity(City city)
        {
          return _context.Cities.Any(c => c.Name == city.Name
           && c.Lat == city.Lat
           && c.Lon == city.Lon
           && c.CountryId == city.CountryId
           && c.Id != city.Id);
        }

        // POST: api/Cities
        // To protect from overpositng attacks
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.Id }, city);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);

            if (city == null) { 
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
