import { Component, Input } from "@angular/core";

@Component({
    selector : 'app-document-metric-block',
    templateUrl : './document-metric-block.component.html',
    standalone : true
})

export class DocumentMetricBlockComponent {
    @Input({required : true}) metricNumber!:number;
    @Input({required : true}) metricName!:string; 
    @Input({required : true}) iconColor!:string;
    @Input({required : true}) iconBackgroundColor!:string;
    @Input({required : true}) cardColor!:string;
}