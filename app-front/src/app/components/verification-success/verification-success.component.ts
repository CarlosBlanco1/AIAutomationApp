import { Component } from "@angular/core";
import { StarIconComponent } from "../../icons/start-icon.component";
import { RightArrowIcon } from "../../icons/right-arrow-icon.component";
import { SparkleIconComponent } from "../../icons/sparkle-icon.component";
import { UsersIconComponent } from "../../icons/users-icon.component";
import { GraphIconComponent } from "../../icons/graph-icon.component";
import { PointerRightIconComponent } from "../../icons/pointer-right-icon.component";

@Component({
    selector : 'app-verification-success',
    templateUrl : './verification-succcess.component.html',
    imports: [StarIconComponent, RightArrowIcon, SparkleIconComponent, UsersIconComponent, GraphIconComponent, PointerRightIconComponent]
})
export class VerificationSuccessComponent{}