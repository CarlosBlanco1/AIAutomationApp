import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function fileValidator() : ValidatorFn {
    return (control: AbstractControl<File>): ValidationErrors | null => {

    const value = control.value;

    if(value === null)
    {
        return null;
    }

    if(value.size > (1 * 1024 * 1024))
    {
        return {exceededSize : true}
    }

    return null;
  };
}
