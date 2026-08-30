import { Component, inject } from '@angular/core';
import { ArrowIconComponent } from "../../icons/arrow-icon.component";
import { PlayIconComponent } from "../../icons/play-icon.component";
import { StackIconComponent } from "../../icons/stack-icon.component";
import { FolderIconComponent } from "../../icons/folder-icon.component";
import { ShieldIconComponent } from "../../icons/shield-icon.component";
import { FocusIconComponent } from "../../icons/focus-icon.component";
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [ArrowIconComponent, RouterLink, PlayIconComponent, StackIconComponent, FolderIconComponent, FocusIconComponent],
  templateUrl: './home.component.html'
})

export class HomeComponent {}
