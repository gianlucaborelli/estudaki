import { Component } from '@angular/core';
import { AuthService } from '../../auth.service';
import { Router } from '@angular/router';
import { MATERIAL_MODULES } from '../../../../shared/imports/material.imports';


@Component({
  imports: [
    ...MATERIAL_MODULES
  ],
  selector: 'app-sign-in.component',
  styleUrl: './sign-in.component.css',
  templateUrl: './sign-in.component.html',
})
export class SignInComponent {

  constructor(private authService: AuthService, private router: Router) {
    
   }

  login(email: string, password: string): void {
    this.authService.login(email, password).subscribe({
      next: () => this.router.navigateByUrl('/'),
      error: (err) => console.error('Login failed', err),
    });
  }
}
