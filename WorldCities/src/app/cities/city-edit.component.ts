import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from './../../environments/environment';
import { City } from './city';
import { Country } from '../countries/country';
import { BaseFormComponent } from '../base-form.component';

import { CityService } from './city.service';
import { ApiResult } from '../base.service'

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.scss']
})

export class CityEditComponent
  extends BaseFormComponent
  implements OnInit {

  // the view title
  title?: string;

  // the city object to edit or create
  city?: City;

  // the city object id, as fetched from the active route:
  // It's NULL when we're adding a new city,
  // and not NULL when we're editing an existing one
  id?: number;

  // the countries array for the select
  countries?: Country[]

  // Actvitiy Log (for debugging purposes)
  activityLog: string = '';

  
  private subscriptions: Subscription = new Subscription()

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private cityService: CityService
  ) {
    super();
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),
      lat: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
      lon: new FormControl('', [Validators.required, Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)]),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());

    // react to form changes
    this.subscriptions.add(
    this.form.valueChanges.subscribe(() => {
      if (!this.form.dirty) {
        this.log("Form Model has been loaded.");
      }
      else {
        this.log("Form was updated by the user.");
      }
    }));

    // react to change in the form.name control
    this.subscriptions.add(
      this.form.get("name")!.valueChanges.subscribe(() => {
        if (!this.form.dirty) {
          this.log("Name has been loaded with initial values.");
        }
        else {
          this.log("Name was updated by the user.");
        }
      }));

    this.loadData();
  }

  log(str: string) {
    this.activityLog += "[" + new Date().toLocaleString() + "] " + str + "<br />";
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {

      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      return this.cityService
        .isDupeCity(city)
        .pipe(map(result => {
        return (result ? { isDupeCity: true } : null);
      }));

    }
  };

  loadData() {
    // load countries
    this.loadCountries();


    // retrieve the ID from the id parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    console.log(idParam);

    this.id = idParam ? +idParam : 0;
    console.log(this.id);

    if (this.id) {
      // Edit Mode :: if this.id contains value
      // fetch the city from the server
      this.cityService
        .get(this.id)
        .subscribe(result => {
        this.city = result;
        this.title = "Edit - " + this.city.name;
        console.log(this.city);

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
      city.countryId = this.form.controls['countryId'].value;

      if (this.id) {
        // Edit mode 
        this.cityService
          .put(city)
          .subscribe(result => {
            console.log("City " + city!.id + " has been updated.");
            // go back to cities view
            this.router.navigate(['/cities']);
          }, error => console.error(error));
      }
      else {
        // Add New mode
        this.cityService
          .post(city)
          .subscribe(result => {
            console.log("City " + result.id + " has been created.");
            // go back to cities view
            this.router.navigate(['/cities']);
          }, error => console.error(error));
      }

    }

  }

  loadCountries() {
    //fetch all the countries from the server
    this.cityService
      .getCountries(0, 9999, "name")
      .subscribe(result => {
      this.countries = result.data;
    }, error => console.error(error));

  }
}

// ng generate component CityEdit --flat --module=app --skip-tests
