import { Routes } from '@angular/router';
import { RegisterComponent } from './components/register/register.component';
import { WorkspacesComponent } from './components/workspaces/workspaces.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DocumentsComponent } from './components/documents/documents.component';
import { LoginCardComponent } from './components/login/logincard.component';
import { AuthGuard } from './guards/auth-guard';

export const routes: Routes = [
    {
        path: '',
        component: DashboardComponent,
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
        path: 'singledocument',
        loadComponent: () =>
            import('./components/single-document/single-document.component')
                .then(m => m.SingleDocumentComponent),
        canActivate: [AuthGuard]
    },

];
