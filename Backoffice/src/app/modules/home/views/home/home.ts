import { Component, inject,  } from '@angular/core';
import { AuthService } from '../../../login/auth.service';

@Component({
  imports: [],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {
  private authService = inject(AuthService);

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  currentUser = this.authService.currentUser;
}
