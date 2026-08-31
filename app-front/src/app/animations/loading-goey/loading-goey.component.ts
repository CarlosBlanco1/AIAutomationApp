import { Component } from "@angular/core";

@Component({
    selector: 'app-loading-goey',
    template: `
    <svg class="stage" 
    viewBox="0 0 90 30">
        <g class="goo" transform="translate(45 15)">
            <circle class="dot dot--a" cx="0" cy="0" r="9" />
            <circle class="dot dot--b" cx="0" cy="0" r="9" />
        </g>
    </svg>`,
    styles: `
      .stage {
        width: 90px;
        height: 30px;
    }
    
    .dot {
        fill: #9333ea;
    }
    
    .dot--a, .dot--b {
        animation-duration: 0.6s;
        animation-timing-function: ease-in-out;
        animation-iteration-count: infinite;
        animation-direction: alternate;
        transform-box: fill-box;
        transform-origin: center;
    }
    
    .dot--a { animation-name: orbit-a; }
    .dot--b { animation-name: orbit-b; }
    
    @keyframes orbit-a {
        0%   { transform: translate(-15.5px, 0) scale(1); }
        100% { transform: translate(15.5px, 0) scale(0.5); }
    }
    
    @keyframes orbit-b {
        0%   { transform: translate(15.5px, 0) scale(0.5); }
        100% { transform: translate(-15.5px, 0) scale(1); }
    }
    
    @media (prefers-reduced-motion: reduce) {
        .dot--a, .dot--b { animation: none; }
    }
    `
})

export class LoadingGoey { }