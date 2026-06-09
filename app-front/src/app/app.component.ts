import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Header/header.component';
import { LoginCardComponent } from './LoginCard/logincard.component';
import { RegisterComponent } from './Register/register.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, LoginCardComponent, RegisterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'app-front';
}
