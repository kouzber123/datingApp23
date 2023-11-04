import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  //pass parent to child

  /**
   *account service for api
   */
  constructor(private accountService: AccountService) {}
  //child to parent we emit this value
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  register() {
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: (error) => console.log(error),
    });
  }

  //we then call it here and pass the value we want
  cancel() {
    this.cancelRegister.emit(false);
    this.register;
  }
}
