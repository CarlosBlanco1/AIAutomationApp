import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './Header/header.component';
import { LoginCardComponent } from './LoginCard/logincard.component';
import { RegisterComponent } from './Register/register.component';
import { SideBarComponent } from './SideBar/sidebar.component';
import { DashboardComponent } from './Dashboard/dashboard.component';
import { WorkspacesComponent } from './Workspaces/workspaces.component';
import { DocumentsComponent } from './Documents/documents.component';
import { SingleDocumentComponent } from './SingleDocument/singledocument.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    HeaderComponent,
    LoginCardComponent,
    RegisterComponent,
    SideBarComponent,
    DashboardComponent,
    WorkspacesComponent,
    DocumentsComponent,
    SingleDocumentComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'app-front';
}
