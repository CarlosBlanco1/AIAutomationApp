import { InjectionToken } from "@angular/core";
import { ConfirmationTokenService } from "./confirmation-token-service.interface";

export const CONFIRMATION_TOKEN_SERVICE = new InjectionToken<ConfirmationTokenService>("CONFIRMATION_TOKEN_SERVICE");