import { Component } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { environment } from '../../environments/environment.development';

@Component({
  selector: 'app-match-import',
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './match-import.html',
  styleUrls: ['./match-import.css']
})
export class MatchImport {
  folderPath: string = '';
  message: string = '';

  constructor(private http: HttpClient) {}

  importFolder() {
    const body = { folderPath: this.folderPath }; // Wrap folderPath in a JSON object
    this.http.post(`${environment.apiBaseUrl}/api/match/import-folder`, body, { responseType: 'text' })
      .subscribe({
        next: (response) => this.message = response,
        error: (error) => this.message = `Error: ${error.error}`
      });
  }
}
