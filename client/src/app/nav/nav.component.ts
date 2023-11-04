import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  /**
   * navbar
   */
  model: any = {};

  //constructor is to inject services to ouir component
  constructor(public accountService: AccountService) {}

  ngOnInit(): void {

  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
            console.log(response)
      },
      error: (error) => console.log(error),
    });
  }
  logout() {
    this.accountService.logout();
  }


}
