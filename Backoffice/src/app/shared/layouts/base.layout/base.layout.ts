import { Component, inject, ViewChild } from '@angular/core';
import { RouterOutlet } from "@angular/router";
import { MATERIAL_MODULES } from '../../imports/material.imports';
import { AuthService } from '../../../modules/login/auth.service';
import { MatSidenav } from '@angular/material/sidenav';


@Component({
  imports: [RouterOutlet, ...MATERIAL_MODULES],
  selector: 'app-base.layout',
  styleUrl: './base.layout.css',
  templateUrl: './base.layout.html',
})
export class BaseLayout {
  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;

  private authService = inject(AuthService);
  private router = inject(RouterOutlet)

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }


}
