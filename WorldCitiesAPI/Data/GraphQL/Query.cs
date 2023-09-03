using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WorldCitiesAPI.Data.Models;


namespace WorldCitiesAPI.Data.GraphQL
{
  public class Query
  {
    /// <summary>
    /// GEt all Cities
    /// </summary>
    /// <returns></returns>
    [Serial]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<City> GetCities([Service] ApplicationDbContext context) => context.Cities;

    /// <summary>
    /// Get all countries
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [Serial]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Country> GetCountries([Service] ApplicationDbContext context) => context.Countries;
  }

}
