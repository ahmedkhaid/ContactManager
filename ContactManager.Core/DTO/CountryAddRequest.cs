using System;
using System.Collections.Generic;
using Entities;

namespace ServiceContracts.DTO
{
  /// <summary>
  /// DTO class for adding a new country
  /// </summary>
  public class CountryAddRequest
  {
    public string? CountryName { get; set; }

    public Country ToCountry()
    {
      return new Country() { CountryID=Guid.NewGuid(),CountryName = this.CountryName };
    }
  }
}

