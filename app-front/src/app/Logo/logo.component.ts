import { Component } from "@angular/core";
import { SparkleIconComponent } from "../icons/sparkle-icon.component";

@Component({
    selector : 'app-logo',
    template : `
    <div class="flex-1 flex gap-1 items-center text-2xl">
        <app-sparkle-icon></app-sparkle-icon>
        <span class="text-white">Workspace</span>
        <span class="text-[#9333ea]">AI</span>
    </div>`,
    standalone : true,
    imports : [SparkleIconComponent]
})

export class LogoComponent {}