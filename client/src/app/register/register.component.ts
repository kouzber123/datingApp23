import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  //child to parent we emit this value
  @Output() cancelRegister = new EventEmitter();
  //max date = year people who can sign up
  maxDate: Date = new Date();
  //this is passed to our component
  registerForm: FormGroup = new FormGroup({});
  validationErrors: String[] | undefined;
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  //on init we calculate current year -18 as 18 is legal adult age ie. 2023 - 18 = 2005
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }
  //this.fb.group = new formGroup
  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', [Validators.required, Validators.minLength(2)]],
      knownAs: ['', [Validators.required, Validators.minLength(2)]],
      dateOfBirth: ['', [Validators.required]],
      city: ['', [Validators.required]],
      country: ['', [Validators.required]],
      password: [
        '',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
      ],
      //password ==  has to match parent
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
    //to check if password changes while confirm pw doesnt confirm password value and validity of its ancestors.
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: (_) =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }
  //check if value matches form given string
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value //get = matching value else err
        ? null
        : { notMatching: true };
    };
  }
  register() {
    //modify date before sending to api
    const dob = this.getDateOnly(
      this.registerForm.controls['dateOfBirth'].value
    );
    //attach registerform new dob value
    const values = { ...this.registerForm.value, dateOfBirth: dob };
    this.accountService.register(values).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
      },
      error: (error) => {
        this.validationErrors = error;
      },
    });
  }

  //we then call it here and pass the value we want
  cancel() {
    this.cancelRegister.emit(false);
    this.register;
  }

  //function to remove time zone from the string as we dont care exact time and time zone varies from location
  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    let theDob = new Date(dob);

    return new Date(
      theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())
    )
      .toISOString()
      .slice(0, 10);
  }
}
