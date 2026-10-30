import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css',
})
export class TextInput implements ControlValueAccessor { // ControlValueAccessor tell angular this component is a form control 
  label = input<string>('');
  type = input<string>('text');
  maxDate = input<string>('');

  // @Self() is a depepdency injection modifier that says to look for the depepdency 
  // we are inejcting on the current element and not to search further up in parent and above
  constructor(@Self() public ngControl: NgControl) {
    // Says TextInput is a type of NgControl and we are assinging it to this component
    this.ngControl.valueAccessor = this;  
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void { 
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl
  }
}
