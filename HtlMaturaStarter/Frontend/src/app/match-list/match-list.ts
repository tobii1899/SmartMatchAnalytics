import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { filter } from 'rxjs';

@Component({
  selector: 'app-match-list',
  imports: [CommonModule],
  templateUrl: './match-list.html',
  styleUrl: './match-list.css'
})
export class MatchList implements OnInit{
  matches = signal<any[]>([]);

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadMatches();
  }

  loadMatches() {
    console.log('Loading matches...');

    this.http.get<any[]>(`${environment.apiBaseUrl}/api/match/matches`)
      .subscribe(data => {
        console.log('Matches received:', data);
        this.matches.set(data);
      });
  }

  openMatch(id: number) {
    this.router.navigate(['/match-details', id]);
  }
}
