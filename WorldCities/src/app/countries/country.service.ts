import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { BaseService, ApiResult } from '../base.service';
import { Observable } from 'rxjs';

import { Country } from '../countries/country';


//Adding CountryService as a singleton service to Angular
@Injectable({
  providedIn: 'root',
})
export class CountryService
  extends BaseService<Country> {

  getData(
    pageIndex?: number,
    pageSize?: number,
    sortColumn?: string,
    sortOrder?: string,
    filterColumn?: string | null,
    filterQuery?: string | null): Observable<ApiResult<Country>> {

    var url = this.getUrl("/api/countries");
    var params = new HttpParams()
      .set("pageIndex", pageIndex?.toString() ?? "0")
      .set("pageSize", pageSize?.toString() ?? "10")
      .set("sortColumn", sortColumn ?? "name")
      .set("sortOrder", sortOrder ?? "asc");

    if (filterColumn && filterQuery) {
      params = params
        .set("filterColumn", filterColumn)
        .set("filterQuery", filterQuery);
    }

    return this.http.get<ApiResult<Country>>(url, { params });
  }

  get(id: number): Observable<Country> {
    var url = this.getUrl("/api/countries/" + id);
    return this.http.get<Country>(url);
  }

  put(item: Country): Observable<Country> {
    var url = this.getUrl("/api/countries/" + item.id);
    return this.http.put<Country>(url, item);
  }

  post(item: Country): Observable<Country> {
    var url = this.getUrl("/api/countries");
    return this.http.post<Country>(url, item);
  }

  isDupeField(countryId: number,
    fieldName: string,
    fieldValue: string): Observable<Boolean> {
    var params = new HttpParams()
      .set("countryId", countryId)
      .set("fieldName", fieldName)
      .set("fieldValue", fieldValue);

    var url = this.getUrl("/api/countries/isdupefield");
    return this.http.post<boolean>(url, null, { params });
  }

  constructor(http: HttpClient) { super(http); }
}
