import { Component, Input } from '@angular/core';
import {
  FormControl,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-input-validator',
  imports: [ReactiveFormsModule],
  templateUrl: './input-validator.component.html',
})
export class InputValidatorComponent {
  @Input({ required: true }) labelText!: string;
  @Input({ required: true }) placeholderText!: string;
  @Input({ required: true }) controlObject!: FormControl;
  @Input({ required: true }) ruleToMessage!: { validationRule: string; errorMessage: string }[];
  @Input({ required: true }) isPassword!: boolean;
  protected readonly inputId = crypto.randomUUID();
}
