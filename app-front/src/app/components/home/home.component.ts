import { Component, signal } from '@angular/core';
import { CarouselComponent } from "../../animations/carousel/carousel.component";
import { ArrowIconComponent } from "../../icons/arrow-icon.component";
import { PlayIconComponent } from "../../icons/play-icon.component";
import { StackIconComponent } from "../../icons/stack-icon.component";
import { FolderIconComponent } from "../../icons/folder-icon.component";
import { ShieldIconComponent } from "../../icons/shield-icon.component";
import { FocusIconComponent } from "../../icons/focus-icon.component";

@Component({
  selector: 'app-home',
  imports: [ArrowIconComponent, PlayIconComponent, StackIconComponent, FolderIconComponent, ShieldIconComponent, FocusIconComponent],
  templateUrl: './home.component.html'
})

export class HomeComponent {
  isMobile = signal(window.innerWidth < 1024);

  constructor() {
    window.addEventListener('resize', () => {
      this.isMobile.set(window.innerWidth < 1024);
    });
  }
}
