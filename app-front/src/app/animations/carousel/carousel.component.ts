import { Component, effect, viewChild } from '@angular/core';
import { EmblaCarouselDirective } from 'embla-carousel-angular';
import Autoplay from 'embla-carousel-autoplay';
import type { EmblaCarouselType } from 'embla-carousel';

@Component({
  selector: 'app-carousel',
  templateUrl: './carousel.component.html',
  styleUrl : './carousel.component.css',
  imports: [EmblaCarouselDirective],
  standalone: true
})

export class CarouselComponent {
  private emblaRef = viewChild<EmblaCarouselDirective>(EmblaCarouselDirective);

  private emblaApi?: EmblaCarouselType;

  protected options = { loop: true };

  protected plugins = [
    Autoplay({
      delay: 3000,
      stopOnInteraction: false,
    }),
  ];

  constructor() {
    effect(() => {
      this.emblaApi = this.emblaRef()?.emblaApi;
    });
  }
}