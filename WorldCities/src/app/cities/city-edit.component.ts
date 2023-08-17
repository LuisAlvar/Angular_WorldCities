import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl } from '@angular/forms';

import { environment } from './../../environments/environment';
import { City } from './city';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})
export class CityEditComponent implements OnInit {

  // the view title
  title?: string;

  // the form model
  form!: FormGroup;

  // the city object to edit or create
  city?: City;

  // the city object id, as fetched from the active route:
  // It's NULL when we're adding a new city,
  // and not NULL when we're editing an existing one
  id?: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) { }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      lat: new FormControl(''),
      lon: new FormControl('')
    });
    this.loadData();
  }

  loadData() {
    // retrieve the ID from the id parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    console.log(idParam);

    this.id = idParam ? +idParam : 0;
    console.log(this.id);

    if (this.id) {
      // Edit Mode :: if this.id contains value

      // fetch the city from the server
      var url = environment.baseUrl + '/api/cities/' + this.id;
      this.http.get<City>(url).subscribe(result => {
        this.city = result;
        this.title = "Edit - " + this.city.name;

        // update the form with the city value
        this.form.patchValue(this.city);

      }, error => console.error(error));
    }
    else {
      // Add new mode :: if this.id is null

      this.title = "Create a new City";
    }
  }

  onSubmit() {
    var city = (this.id) ? this.city : <City>{};

    console.log(city);

    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;

      if (this.id) {
        // Edit mode 
        var url = environment.baseUrl + '/api/cities/' + city.id;

        this.http
          .put<City>(url, city)
          .subscribe(result => {
            console.log("City " + city!.id + " has been updated.");

            // go back to cities view
            this.router.navigate(['/cities']);
          }, error => console.error(error));

      }
      else {
        // Add New mode
        var url = environment.baseUrl + '/api/cities';

        this.http
          .post<City>(url, city)
          .subscribe(result => {
            console.log("City " + result.id + " has been created.");

            // go back to cities view
            this.router.navigate(['/cities']);
          }, error => console.error(error));
      }
    }
  }

}

// ng generate component CityEdit --flat --module=app --skip-tests
