import { Routes } from '@angular/router';
import { AuthGuard } from './services/auth.guard';
import { NoAuthGuard } from './services/no-auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login').then(m => m.LoginComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register').then(m => m.RegisterComponent),
    canActivate: [NoAuthGuard]
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'tasks',
    loadComponent: () =>
      import('./pages/home/home').then(m => m.HomeComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'tasks/new',
    loadComponent: () =>
      import('./pages/add-task/add-task').then(m => m.AddTaskComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'tasks/edit/:id',
    loadComponent: () =>
      import('./pages/edit-task/edit-task').then(m => m.EditTaskComponent),
    canActivate: [AuthGuard]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
