import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from './../../environments/environment';

import { City } from './city';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.scss']
})

export class CitiesComponent implements OnInit {
  public cities!: City[]

  constructor(private http: HttpClient) { }

  ngOnInit() {
    const headers = new HttpHeaders().set('Access-Control-Allow-Origin', 'https://localhost:4200/');

    this.http.get<City[]>(environment.baseUrl + '/api/cities')
      .subscribe(result => {
        this.cities = result;
      }, error => console.error(error));
  }
}

