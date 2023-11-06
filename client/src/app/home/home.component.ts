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
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  //in parent html =  (cancelRegister)="cancelRegisterMode($event)", cancelRegister = child output
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }
}
