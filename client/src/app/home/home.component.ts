import { HttpClient } from '@angular/common/http';
import { Component, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;
  /**
   * home page
    pass parent to child
   */
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.getUsers();
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  getUsers() {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: (response) => (this.users = response),
      error: (error) => console.log(error),
      complete: () => console.log('Request has completet'),
    });
  }
  //in parent html =  (cancelRegister)="cancelRegisterMode($event)", cancelRegister = child output
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }
}
