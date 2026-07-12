import { Component, inject, output, Output, signal } from '@angular/core';
import { LogoComponent } from '../../logo/logo.component';
import { HouseIconComponent } from '../../icons/house-icon.component';
import { DocumentIconComponent } from '../../icons/document-icon.component';
import { AnalyticsIconComponent } from '../../icons/analytics-icon.component';
import { UserIconComponent } from '../../icons/user-icon.component';
import { AccountIconComponent } from '../../icons/account-icon.component';
import { DropdownIconComponent } from '../../icons/dropdown-icon.component';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { AUTH_SERVICE } from '../../services/auth/auth-service.token';
import { USER_SERVICE } from '../../services/user/user-service.token';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  standalone: true,
  imports: [
    LogoComponent,
    HouseIconComponent,
    DocumentIconComponent,
    AnalyticsIconComponent,
    UserIconComponent,
    AccountIconComponent,
    DropdownIconComponent,
    RouterLink
  ],
})
export class SideBarComponent {
  protected readonly authService = inject(AUTH_SERVICE);
  protected readonly userService = inject(USER_SERVICE);
  private router = inject(Router);

  navigatedMobile = output<void>();

  constructor() {
    this.router.events.pipe(
      takeUntilDestroyed(),
      filter((event => event instanceof NavigationEnd))
    ).subscribe(() => {
      if (window.innerWidth < 1024) {
        this.navigatedMobile.emit();
      }
    }
    )
  }


}
