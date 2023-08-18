// Required
import { Component, OnInit } from '@angular/core';
// API
import { HttpClient, HttpParams } from '@angular/common/http';
// Fetch Data Within Template
import { ActivatedRoute, Router } from '@angular/router';
// Form Creation and Validators
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms'
// Async Validators
import { Observable } from 'rxjs'
import { map } from 'rxjs/operators';

// Component Items
import { environment } from './../../environments/environment';
import { Country } from './country';
import { Route } from '@angular/compiler/src/core';
import { env } from 'process';
import { count } from 'console';

@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrls: ['./country-edit.component.scss']
})

export class CountryEditComponent implements OnInit {

  // the view title: Either Create or Edit
  title?: string;

  // the form model
  form!: FormGroup;

  // the country object to edit or create
  country?: Country;

  // the country object id, as fetched from the active route:
  // Its NULL when we're adding a new country,
  // and not NULL when we're editing an existing one.
  id?: number

  // the countries array for select
  countries?: Country[];

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) { }

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required, this.isDupeField("name")],
      iso2: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{2}$/)], this.isDupeField("iso2")],
      iso3: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{3}$/)], this.isDupeField("iso3")]
    })

    this.loadData();
  }

  loadData() {
    //retieve the id from the id parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;

    if (this.id) {
      //Edit Mode

      //Fetch the country from the server
      var url = environment.baseUrl + "/api/countries/" + this.id;
      this.http.get<Country>(url).subscribe(result => {
        this.country = result;
        this.title = "Edit - " + this.country.name;

        // update the form with the country value
        this.form.patchValue(this.country);
      }, error => console.error(error));

    }
    else {
      // Add New Mode
      this.title = "Create a new Country";
    }

  }

  onSubmit() {
    var country = (this.id) ? this.country : <Country>{};

    if (country) {
      country.name = this.form.controls['name'].value;
      country.iso2 = this.form.controls['iso2'].value;
      country.iso3 = this.form.controls['iso3'].value;

      if (this.id) {
        // Edit Mode

        var url = environment.baseUrl + "/api/countries/" + this.id;

        this.http.put<Country>(url, country).subscribe(result => {
          console.log("Country " + result.id + " has been created.");

          // go back to conuntries view
          this.router.navigate(['/countries']);
        }, error => console.error(error));

      }
      else {
        // ADD NEW mode

        var url = environment.baseUrl + "/api/countries/";
        this.http.post<Country>(url, country).subscribe(result => {
          console.log("Country " + result.id + " has been created.");

          //go back to counries view
          this.router.navigate(['/countries']);
        }, error => console.error(error))
      }

    }

  }

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : 0)
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);

      var url = environment.baseUrl + "/api/countries/isdupefield";

      return this.http.post<boolean>(url, null, { params })
        .pipe(map(result => {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }

}
