import { Component } from '@angular/core';
import { RouterOutlet } from "@angular/router";

@Component({
  imports: [RouterOutlet],
  selector: 'app-base.layout',
  styleUrl: './base.layout.css',
  templateUrl: './base.layout.html',
})
export class BaseLayout {}
