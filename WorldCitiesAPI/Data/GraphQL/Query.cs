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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortOrder"></param>
    /// <param name="filtercolumn"></param>
    /// <param name="filterQuery"></param>
    /// <returns></returns>
    [Serial]
    public async Task<ApiResult<CityDTO>> GetCitiesApiResult(
      [Service] ApplicationDbContext context,
      int pageIndex = 0,
      int pageSize = 10,
      string? sortColumn = null,
      string? sortOrder = null,
      string? filtercolumn = null,
      string? filterQuery = null
     )
    {
      return await ApiResult<CityDTO>.CreateAsync(
          context.Cities.AsNoTracking().Select(c => new CityDTO()
          {
            Id = c.Id,
            Name = c.Name,
            Lat = c.Lat,
            Lon = c.Lon,
            CountryId = c.CountryId,
            CountryName = c.Country!.Name
          }),
          pageIndex, pageSize, sortColumn, sortOrder, filtercolumn, filterQuery);
    }
  }

}
