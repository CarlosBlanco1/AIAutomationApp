export function getRuleToMessageText(inputName: string, minLength : number, maxlength : number) {
    return [
        {
            validationRule: 'required',
            errorMessage: `${inputName} is required.`,
        },
        {
            validationRule: 'minlength',
            errorMessage: `${inputName} must be at least ${minLength} characters long.`,
        },
        {
            validationRule: 'maxlength',
            errorMessage: `${inputName} must be no longer than ${maxlength} characters long.`,
        },
    ];
}

export function getRuleToMessageEmail() {
    return [
        {
            validationRule: 'required',
            errorMessage: `Email is required.`,
        },
        {
            validationRule: 'email',
            errorMessage: `Invalid email format.`,
        },
    ];
}

export function getRuleToMessageNewPassword() {
    return [
        {
            validationRule: 'required',
            errorMessage: `Password is required.`,
        },
        {
            validationRule: 'minlength',
            errorMessage: `Password must be at least 12 characters long.`,
        },
        {
            validationRule: 'maxlength',
            errorMessage: `Password must be no longer than 128 characters long.`,
        },
        {
            validationRule: 'notContainsSymbol',
            errorMessage: `Password must contain at least 1 symbol.`,
        },
        {
            validationRule: 'notContainsUpperAndLowerCase',
            errorMessage: `Password must contain at least 1 upper case and 1 lower case letter.`,
        },
        {
            validationRule: 'notContainsNumber',
            errorMessage: `Password must contain at least 1 number.`,
        },
    ];
}

export function getRuleToMessageExistingPassword() {
    return [
        {
            validationRule: 'required',
            errorMessage: `Password is required.`,
        }
    ];
}

export function getRuleToMessageFile() {
    return [
        {
            validationRule : 'required',
            errorMessage : 'File is required.',
        },
        {
            validationRule : 'exceededSize',
            errorMessage : 'File can be no more than 5MB.',
        }
    ]
}