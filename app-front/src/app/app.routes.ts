import { Routes } from '@angular/router';
import { RegisterComponent } from './components/register/register.component';
import { WorkspacesComponent } from './components/workspaces/workspaces.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DocumentsComponent } from './components/documents/documents.component';
import { LoginCardComponent } from './components/login/logincard.component';
import { AuthGuard } from './guards/auth-guard';
import { HomeComponent } from './components/home/home.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
    },
    {
        path: 'login',
        component: LoginCardComponent,
    },
    {
        path: 'register',
        component: RegisterComponent,
    },
    {
        path: 'dashboard',
        component: DashboardComponent,
        canActivate : [AuthGuard]
    },
    {
        path: 'workspaces',
        component: WorkspacesComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'documents',
        component: DocumentsComponent,
        canActivate: [AuthGuard]
    },
    {
        path: 'singledocument/:documentId',
        loadComponent: () =>
            import('./components/single-document/single-document.component')
                .then(m => m.SingleDocumentComponent),
        // canActivate: [AuthGuard]
    },

];
