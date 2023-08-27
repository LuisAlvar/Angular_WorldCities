import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from '../angular-material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

import { CitiesComponent } from './cities.component';
import { City } from './city';
import { CityService } from './city.service';
import { ApiResult } from '../base.service';


// we can use the describe API to lay out our testing suite

describe('CitiesComponent', () => {
  let component: CitiesComponent;
  let fixture: ComponentFixture<CitiesComponent>;

  beforeEach(async () => {
    //TODO: declare & initialzie requried providers

    //Create a mock CityService object with a mock 'getData' method
    let cityService = jasmine.createSpyObj<CityService>('CityService', ['getData']);

    //Configure the 'getData' spy method
    cityService.getData.and.returnValue(
      of<ApiResult<City>>(<ApiResult<City>>{
        data: [
          <City>{
            name: 'TestCity1',
            id: 1,
            lat: 1,
            lon: 1,
            countryId: 1,
            countryname: "TestCountry1"
          },
          <City>{
            name: 'TestCity2',
            id: 2,
            lat: 1,
            lon: 1,
            countryId: 1,
            countryname: "TestCountry1"
          },
          <City>{
            name: "TestCity3",
            id: 3,
            lat: 1,
            lon: 1,
            countryId: 1,
            countryname: "TestCountry1"
          }
        ],
        totalCount: 3,
        pageIndex: 0,
        pageSize: 10
      }));


    await TestBed.configureTestingModule({
      declarations: [CitiesComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        //TODO: reference requried providers
      ]
    })
      .compileComponents();

  });


  beforeEach(() => {
    fixture = TestBed.createComponent(CitiesComponent);
    component = fixture.componentInstance;

    //TODO: configure fixture/component/children/etc.

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  //TODO: implement some other tests


});
