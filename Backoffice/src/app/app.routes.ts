import { Routes } from '@angular/router';
import { SignInComponent } from './modules/login/view/sign-in.component/sign-in.component';
import { BaseLayout } from './shared/layouts/base.layout/base.layout';
import { Home } from './modules/home/views/home/home';

export const routes: Routes = [
  {
    path: 'login',
    component: SignInComponent
  },
  {
    path: '',
    component: BaseLayout,
    children:[
      {
        path: '',
        component: Home,
        pathMatch: 'full'
      },
    ]
  }
];
