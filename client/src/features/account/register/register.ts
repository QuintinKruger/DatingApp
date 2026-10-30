import { Component, inject, input, OnInit, output, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';
import { JsonPipe } from '@angular/common';
import { TextInput } from '../../../shared/text-input/text-input';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  // imports: [FormsModule], temaplate driven form
  imports: [ReactiveFormsModule, JsonPipe, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private router = inject(Router);
  private accountService = inject(AccountService);
  private fb = inject(FormBuilder)

  // another way to provide data to child from parent --> using an input signal 
  // membersFromHome = input.required<User[]>(); // .required makes sure parents provide a value for the input variable
  cancelRegister = output<boolean>();

  // {} as RegisterCreds is a type assertion. 
  // It skips that check, because you're telling TypeScript you know what the object will be. The compiler accepts it
  protected creds = {} as RegisterCreds

  // creds: RegisterCreds = {} is a type annotation. Its not the same as the above
  // TypeScript checks the value, sees that {} is missing email, displayName and password, and gives a compile error.
  // protected creds : RegisterCreds = {} // this does not compile

  protected credentialsForm: FormGroup;
  protected profileForm: FormGroup;
  protected currentStep = signal(1);
  protected validationErrors = signal<string[]>([])

  constructor() {
    // this is simplified using form builder below - not by much in my opinion
    // this.credentialsForm = new FormGroup({
    //   email: new FormControl('', [Validators.required, Validators.email]), // prefilled input and validators
    //   displayName: new FormControl('',Validators.required),
    //   password: new FormControl('',[Validators.required, Validators.minLength(4)]),
    //   confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    // })

    this.credentialsForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]], // prefilled input and validators
      displayName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })

    this.profileForm = this.fb.group({
      gender: ["male", Validators.required],
      dateOfBirth: ["", Validators.required],
      city: ["", Validators.required],
      country: ["", Validators.required]
    })
    // if the password is changed to match confirmPassword, we need to trigger a check 
    // when password control is changed to run confirmPassword's validations again
    // reason we cant add this.matchValues('confirmPassword') to password control 
    // is documented in NOTES.md under FAQ - 'Why doesn't a validator that compares two form controls update when the other control changes?'
    this.credentialsForm.controls['password'].valueChanges.subscribe(() => {
      this.credentialsForm.controls['confirmPassword'].updateValueAndValidity();
    })
  }

  // custom validator for reactive form
  matchValues(matchTo: string): ValidatorFn { // field we compare to passed in
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent // for access to other field in this form
      if (!parent) return null; // nothing to compare to

      const matchValue = parent.get(matchTo)?.value;
      return control.value === matchValue ? null : { passwordMismatch: true } // return null meaning valid, otherwise an object of errors
    }
  }

  register() {
    if (this.profileForm.valid && this.credentialsForm.valid) {
      const formData = { ...this.credentialsForm.value, ...this.profileForm.value }
      this.accountService.register(formData).subscribe({
        next: () => {
          this.router.navigateByUrl("/members")
        },
        error: error => { 
          console.log(error)
          this.validationErrors.set(error)
         }
      })
    }

  }

  cancel() {
    console.log('cancelled');
    this.cancelRegister.emit(false);
  }

  prevStep() {
    this.currentStep.update(prevStep => prevStep - 1)
  }

  nextStep() {
    if (this.credentialsForm.valid) {
      this.currentStep.update(prevStep => prevStep + 1)
    }
  }

  getMaxDate() {
    const today = new Date();
    today.setFullYear(today.getFullYear() - 18)
    return today.toISOString().split('T')[0]
  }
}
