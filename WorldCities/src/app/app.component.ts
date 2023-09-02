import { Component, OnInit } from '@angular/core';
import { ConnectionService } from 'angular-connection-service';
import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'WorldCities';

  hasNetworkConnectivity: boolean = true;
  hasInternetAccess: boolean = true;

  constructor(private authService: AuthService, private connectionService: ConnectionService) {
    this.connectionService.monitor().subscribe((currentState: any) => {
      this.hasNetworkConnectivity = currentState.hasNetworkConnection;
      this.hasInternetAccess = currentState.hasInternetAccess;
    });

  }

  public isOnline() {
    return this.hasNetworkConnectivity && this.hasInternetAccess;
  }

  ngOnInit(): void {
    this.authService.init();
  }

}
