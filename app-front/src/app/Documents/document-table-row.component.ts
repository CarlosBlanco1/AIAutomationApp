import { Component, Input } from "@angular/core";
import { HorizontalDotsIconComponent } from "../Icons/horizontal-dots-icon.component";

@Component({
    selector : 'tr[app-document-table-row]',
    templateUrl : './document-table-row.component.html',
    imports : [HorizontalDotsIconComponent]
})
export class DocumentTableRowComponent {
    @Input({required : true}) documentName!:string;
    @Input({required : true}) documentSubtitle!:string;
    @Input({required : true}) documentCategory!:string;
    @Input({required : true}) minutesAgoEdited!:number;
}