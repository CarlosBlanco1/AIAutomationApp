import { Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { LoginCardComponent } from './LoginCard/logincard.component';
import { RegisterComponent } from './Register/register.component';
import { DashboardComponent } from './Dashboard/dashboard.component';
import { WorkspacesComponent } from './Workspaces/workspaces.component';
import { DocumentsComponent } from './Documents/documents.component';
import { SingleDocumentComponent } from './SingleDocument/singledocument.component';

export const routes: Routes = [

    {
        path: 'login',
        component: LoginCardComponent,
    },
    {
        path: 'register',
        component: RegisterComponent,
    },
    {
        path: '',
        component: DashboardComponent,
    },
    {
        path: 'workspaces',
        component: WorkspacesComponent,
    },
    {
        path: 'documents',
        component: DocumentsComponent,
    },
    {
        path: 'singledocument',
        component: SingleDocumentComponent,
    },

];
