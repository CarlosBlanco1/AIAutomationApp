import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function securePasswordValidator() : ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {

    const value = control.value;

    if(value === null)
    {
        return null;
    }

    if(value.length < 12 || value.length > 128)
    {
        return null
    }

    const containsSymbol = /[^a-zA-Z0-9]/.test(value);
    
    if(!containsSymbol) {
        return {notContainsSymbol : true }
    }
    
    const containsUpperAndLowerCase = /[A-Z]/.test(value) && /[a-z]/.test(value);
    
    if(!containsUpperAndLowerCase) {
        return {notContainsUpperAndLowerCase : true }
    }

    const containsNumber = /[0-9]/.test(value);
    
    if(!containsNumber) {
        return {notContainsNumber : true }
    }

    return null;
  };
}
