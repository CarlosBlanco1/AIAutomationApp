import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { SideBarComponent } from './components/sidebar/sidebar.component';
import { AUTH_SERVICE } from './services/auth/auth-service.token';
import { USER_SERVICE } from './services/user/user-service.token';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    HeaderComponent,
    SideBarComponent,
    RouterOutlet
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  authService = inject(AUTH_SERVICE)
  userService = inject(USER_SERVICE)

  constructor() {
    const token = this.authService.getToken();

    if (token) {
      this.userService.fetchCurrentUser().subscribe({
        error: () => {
          this.authService.logout();
        }
      });
    }
  }
}
