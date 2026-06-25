import { InjectionToken } from "@angular/core";
import { UserService } from "./user-service.interface";

export const USER_SERVICE = new InjectionToken<UserService>("USER_SERVICE")