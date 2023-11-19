import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css'],
})
export class TestErrorComponent {
  baseUrl = environment.apiUrl;
  validationErrors: string[] = [];

  /**
   *
   */
  constructor(private http: HttpClient) {}

  get404Error() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: (resp) => console.log(resp),
      error: (err) => console.log(err),
    });
  }
  get400Error() {
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: (resp) => console.log(resp),
      error: (err) => console.log(err),
    });
  }
  get500Error() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: (resp) => console.log(resp),
      error: (err) => console.log(err),
    });
  }
  get401Error() {
    this.http.get(this.baseUrl + 'buggy/auth').subscribe({
      next: (resp) => console.log(resp),
      error: (err) => console.log(err),
    });
  }
  get400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe({
      next: (resp) => console.log(resp),
      error: (err) => {
        this.validationErrors = err;
        console.log(err);
      },
    });
  }
}
