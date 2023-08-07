﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Runtime;
using System.Security;
using System.Collections.Concurrent;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        { 
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            //prevent non-development environments from running this seed method
            if (!_env.IsDevelopment()) {
                throw new SecurityException("Import Functionality not allowed");
            }

            string path = Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");
            Console.WriteLine("What is ContextRootPath: " + _env.ContentRootPath);

            using FileStream stream = System.IO.File.OpenRead(path);
            using ExcelPackage excelPackage = new ExcelPackage(stream);

            //get the first worksheet
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];

            //define how many rows we want to process
            int nEndRow = worksheet.Dimension.End.Row;

            //initialize the record counters
            int numberOfCountriesAdded = 0;
            int numberOfCitiesAdded = 0;

            // create a lookup dictionary 
            // containing all the countires alredy existing 
            // into the Database (it will be empty on first run).
            var countriesByName = _context.Countries.AsNoTracking().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
    
            //iterates through all rows, skipping the first one
            for (int nRow = 2; nRow < nEndRow; nRow++)
            {
                ExcelRange row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

                string countryName = row[nRow, 5].GetValue<string>();
                string iso2 = row[nRow, 6].GetValue<string>();
                string iso3 = row[nRow, 7].GetValue<string>();


                // skip this country if it already exists in the database
                if (countriesByName.ContainsKey(countryName))
                {
                    continue;
                }


                // create the Country entity and fill it with xlsx data
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };

                // add the new coutnry to the DB context 
                await _context.Countries.AddAsync(country);

                // store the country in our lookup to retrieve its Id later on 
                countriesByName.Add(countryName, country);
                numberOfCountriesAdded++;
            }

            // save all the countries into the Database
            if (numberOfCountriesAdded > 0)
            {
                await _context.SaveChangesAsync();  
            }


            // create a lookup dictionary 
            // containing all the cities already existing 
            // into the Database (it will be empty on first run).
            var cities = _context.Cities.AsNoTracking().ToDictionary(x => 
                new CityKey {
                    Name = x.Name,
                    Lat = x.Lat,
                    Lon = x.Lon,
                    CountryId = x.CountryId
                }
            );

            // iterates through all rows, skipping the first one
            for (int nRow = 2; nRow < nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                // retrieve country id by countryName 
                var countryId = countriesByName[countryName].Id;

                // skip this city if it already exists in the database 
                if (cities.ContainsKey(new CityKey { Name = name, Lat = lat, Lon = lon, CountryId = countryId }))
                {
                    continue;
                }

                // create the City entity and fill it with xlsx data
                var city = new City
                {
                    Name = name,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId
                };

                // add the new city to the db context 
                _context.Cities.Add(city);

                numberOfCitiesAdded++;

            }

            if (numberOfCitiesAdded > 0)
            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
                await _context.SaveChangesAsync();app
            }

            return new JsonResult(new {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            });
        }
    }


    public class CityKey
    {
        public string Name { get; set; } = null!;
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public int CountryId { get; set; }
    }


}
