import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { environment } from '../../environments/environment.development';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-match-details',
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './match-details.html',
  styleUrl: './match-details.css'
})
export class MatchDetails implements OnInit {
  matchDetails = signal<any | null>(null);
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.errorMessage = 'No match ID provided';
      return;
    }

    this.http.get<any>(`${environment.apiBaseUrl}/api/match/match-details-with-scores/${id}`)
      .subscribe({
        next: data => this.matchDetails.set(data),
        error: err => this.errorMessage = err.error
      });
  }

  getScore() {
    const details = this.matchDetails();
    if (!details?.events) return '';

    let home = 0;
    let away = 0;

    //logic for getscore

    return `(${home}:${away})`;
  }

  getEventClass(action: string) {
    switch (action) {
      case 'Goal': return 'goal';
      case 'YellowCard': return 'yellow';
      case 'RedCard': return 'red';
      default: return '';
    }
  }

  getEventIcon(action: string) {
    switch (action) {
      case 'Goal': return '⚽';
      case 'YellowCard': return '🟨';
      case 'RedCard': return '🟥';
      default: return '•';
    }
  }
}
