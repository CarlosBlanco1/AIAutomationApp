import { Component } from '@angular/core';
import { LogoComponent } from '../../logo/logo.component';
import { HouseIconComponent } from '../../icons/house-icon.component';
import { DocumentIconComponent } from '../../icons/document-icon.component';
import { AnalyticsIconComponent } from '../../icons/analytics-icon.component';
import { UserIconComponent } from '../../icons/user-icon.component';
import { AccountIconComponent } from '../../icons/account-icon.component';
import { DropdownIconComponent } from '../../icons/dropdown-icon.component';
import { RouterLink } from '@angular/router';

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
export class SideBarComponent {}
