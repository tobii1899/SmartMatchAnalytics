import { Component, computed, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { environment } from '../environments/environment.development';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, DecimalPipe],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(private router: Router) {}

  goToMatches() {
    this.router.navigateByUrl('/match-list');
  }

  goToImport() {
    this.router.navigateByUrl('/');
  }
}
