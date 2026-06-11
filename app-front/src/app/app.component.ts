import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Header/header.component';
import { LoginCardComponent } from './LoginCard/logincard.component';
import { RegisterComponent } from './Register/register.component';
import { SideBarComponent } from './SideBar/sidebar.component';
import { DashboardComponent } from './Dashboard/dashboard.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    HeaderComponent,
    LoginCardComponent,
    RegisterComponent,
    SideBarComponent,
    DashboardComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'app-front';
}
