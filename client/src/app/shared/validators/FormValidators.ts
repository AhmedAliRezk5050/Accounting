import {AbstractControl, ValidationErrors, ValidatorFn} from "@angular/forms";

export class FormValidators {
  static matchPassword(controlName: string, matchControlName: string): ValidatorFn {
    return (abstractControl: AbstractControl): ValidationErrors | null => {
      const control = abstractControl.get(controlName);
      const matchingControl = abstractControl.get(matchControlName);

      if (!control || !matchingControl) {
        return {controlNotFound: true}
      }

      const error = control.value === matchingControl.value ? null : {noMatch: true}

      matchingControl.setErrors(error);

      return error;
    }
  }
}
