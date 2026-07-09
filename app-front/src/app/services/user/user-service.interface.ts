import { Observable } from "rxjs";
import { UserDto } from "../../models/Users/user-dto";
import { WritableSignal } from "@angular/core";

export interface UserService
{
    currentUser : WritableSignal<UserDto | null>;
    fetchCurrentUser() : Observable<void>;
    clearCurrentUser() : void;
}