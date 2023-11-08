import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css'],
})
export class DatePickerComponent implements ControlValueAccessor {
  @Input() label = 'Date of Birth';
  @Input() maxDate: Date | undefined;

  bsConfig: Partial<BsDatepickerConfig> | undefined;
  /**
   *we extend our ngControl with this ie DatePickerComponent
   */
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: "theme-red",
      dateInputFormat: "DD MMMM YYYY"
    }
  }
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}

  //Tracks the value and validation status of an individual form control.
  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }
}
