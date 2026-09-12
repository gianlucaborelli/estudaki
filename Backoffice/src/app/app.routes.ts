import { Routes } from '@angular/router';
import { SignInComponent } from './modules/login/view/sign-in.component/sign-in.component';
import { BaseLayout } from './shared/layouts/base.layout/base.layout';
import { Home } from './modules/home/views/home/home';
import { authGuard, guestGuard } from './core/guards/auth.guard';
import { ExamList } from './modules/exams/views/exam-list/exam-list';

export const routes: Routes = [
  {
    path: 'login',
    component: SignInComponent,
    canActivate: [guestGuard]
  },
  {
    path: '',
    component: BaseLayout,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        component: Home,
        pathMatch: 'full'
      },
      {
        path: 'exams',
        component: ExamList,
        pathMatch: 'full'
      },
    ]
  }
];
