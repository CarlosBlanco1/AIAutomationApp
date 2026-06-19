import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function securePasswordValidator() : ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {

    if(control.value.length < 12)
    {
        return null
    }

    const containsSymbol = /[^a-zA-Z0-9]/.test(control.value);
    
    if(!containsSymbol) {
        return {notContainsSymbol : true }
    }
    
    const containsUpperAndLowerCase = /[A-Z]/.test(control.value) && /[a-z]/.test(control.value);
    
    if(!containsUpperAndLowerCase) {
        return {notContainsUpperAndLowerCase : true }
    }

    const containsNumber = /[0-9]/.test(control.value);
    
    if(!containsNumber) {
        return {notContainsNumber : true }
    }

    return null;
  };
}
