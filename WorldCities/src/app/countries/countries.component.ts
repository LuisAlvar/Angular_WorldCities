import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from './../../environments/environment';

import { Country } from './country';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';


@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.scss']
})
export class CountriesComponent implements OnInit {
  public displayedColumns: string[] = ['id', 'name', 'iso2', 'iso3'];
  public countries!: MatTableDataSource<Country>

  //paging
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  //sorting
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";
  //filtering
  defaultFilterColumn: string = "name";
  defaultQuer?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    var url = environment.baseUrl + "/api/Countries";

    this.http.get<any>(url)
      .subscribe(result => {
          this.countries = new MatTableDataSource<Country>(result.data)
      }, error => console.error(error))
  }

}
