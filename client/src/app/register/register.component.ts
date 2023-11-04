import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

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
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}
  //child to parent we emit this value
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  register() {
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: (error) => {
        console.log(error);
        this.toastr.error(error.error);
      },
    });
  }

  //we then call it here and pass the value we want
  cancel() {
    this.cancelRegister.emit(false);
    this.register;
  }
}
