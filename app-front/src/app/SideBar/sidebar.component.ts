import { Component } from '@angular/core';
import { LogoComponent } from '../Logo/logo.component';
import { HouseIconComponent } from '../Icons/house-icon.component';
import { DocumentIconComponent } from '../Icons/document-icon.component';
import { AutomationIconComponent } from '../Icons/automation-icon.component';
import { AnalyticsIconComponent } from '../Icons/analytics-icon.component';
import { UserIconComponent } from '../Icons/user-icon.component';
import { AccountIconComponent } from '../Icons/account-icon.component';
import { DropdownIconComponent } from '../Icons/dropdown-icon.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  standalone: true,
  imports: [
    LogoComponent,
    HouseIconComponent,
    DocumentIconComponent,
    AutomationIconComponent,
    AnalyticsIconComponent,
    UserIconComponent,
    AccountIconComponent,
    DropdownIconComponent,
    RouterLink
  ],
})
export class SideBarComponent {}
