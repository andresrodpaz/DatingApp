import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

/**
 * Component for a date picker using ngx-bootstrap.
 * Implements ControlValueAccessor for form integration.
 */
@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.css']
})
export class DatePickerComponent implements ControlValueAccessor {

  /**
   * Input property for the label of the date picker.
   */
  @Input() label = '';

  /**
   * Input property for the maximum selectable date.
   */
  @Input() maxDate: Date | undefined;

  /**
   * Configuration options for the ngx-bootstrap date picker.
   */
  bsConfig: Partial<BsDatepickerConfig> | undefined;

  /**
   * Constructor to inject NgControl and set up the value accessor and date picker configuration.
   * @param ngControl - Control for accessing and managing form control.
   */
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: 'theme-red',
      dateInputFormat: 'DD MMMM YYYY'
    };
  }

  /**
   * Method to write a value to the component.
   * @param obj - Value to be written.
   */
  writeValue(obj: any): void {}

  /**
   * Registers a function to be called when the control's value changes.
   * @param fn - Callback function.
   */
  registerOnChange(fn: any): void {}

  /**
   * Registers a function to be called when the control is touched.
   * @param fn - Callback function.
   */
  registerOnTouched(fn: any): void {}

  /**
   * Sets the disabled state of the control.
   * @param isDisabled - Boolean indicating if the control is disabled.
   */
  setDisabledState?(isDisabled: boolean): void {}

  /**
   * Getter for the FormControl associated with the NgControl.
   * @returns The FormControl instance.
   */
  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }

  /**
   * Validator function to ensure the selected date makes the user at least 18 years old.
   * @param control - FormControl to validate.
   * @returns An object with a validation error key if invalid, otherwise null.
   */
  validateDOB(control: FormControl): { [key: string]: any } | null {
    if (control.value) {
      const selectedDate = new Date(control.value);
      const today = new Date();
      const minDate = new Date(today.getFullYear() - 18, today.getMonth(), today.getDate());

      if (selectedDate > minDate) {
        return { invalidDOB: true };
      }
    }
    return null;
  }
}
