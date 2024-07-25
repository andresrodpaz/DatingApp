import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

/**
 * Component for a text input field that integrates with Angular forms.
 * Implements ControlValueAccessor for form control integration.
 */
@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {

  /**
   * Input property for the label of the text input.
   */
  @Input() label = '';

  /**
   * Input property for the type of the text input (e.g., 'text', 'password').
   */
  @Input() type = 'text';

  /**
   * Constructor to inject NgControl and set up the value accessor.
   * @param ngControl - Control for accessing and managing form control.
   */
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
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
}
