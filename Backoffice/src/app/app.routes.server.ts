import { RenderMode, ServerRoute } from '@angular/ssr';
import { SignInComponent } from './modules/login/view/sign-in.component/sign-in.component';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'login',
    renderMode: RenderMode.Server
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
