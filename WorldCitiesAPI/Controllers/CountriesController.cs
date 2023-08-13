﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        { 
            _context = context;
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
          // first we perform the filtering ...
          var countries = _context.Countries.AsNoTracking();

          if (!string.IsNullOrEmpty(filterColumn)
              && !string.IsNullOrEmpty(filterQuery))
          {
            countries = countries.Where(c => c.Name.StartsWith(filterQuery));
          }

          return await ApiResult<Country>.CreateAsync(
              countries,
              pageIndex,
              pageSize,
              sortColumn,
              sortOrder);
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

            return NoContent();
        }


        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country country)
        { 
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
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
