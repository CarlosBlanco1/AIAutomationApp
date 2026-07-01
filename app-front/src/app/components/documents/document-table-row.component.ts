import { Component, HostListener, inject, Input, ViewContainerRef } from "@angular/core";
import { HorizontalDotsIconComponent } from "../../icons/horizontal-dots-icon.component";
import { InfoIconComponent } from "../../icons/info-icon.component";
import { TrashIconComponent } from "../../icons/trash-icon.component";
import { DownloadIconComponent } from "../../icons/download-icon.component";
import { NgxSmartModalService } from "ngx-smart-modal";
import { DeleteDocumentComponent } from "./delete-document/delete-document.component";
import { RouterLink } from "@angular/router";

@Component({
    selector: 'tr[app-document-table-row]',
    templateUrl: './document-table-row.component.html',
    imports: [HorizontalDotsIconComponent, InfoIconComponent, TrashIconComponent, DownloadIconComponent, RouterLink]
})
export class DocumentTableRowComponent {
    @Input({ required: true }) documentName!: string;
    @Input({ required: true }) documentId!: string;
    @Input({ required: true }) documentSubtitle!: string;
    @Input({ required: true }) documentCategory!: string;
    @Input({ required: true }) minutesAgoEdited!: number;

    constructor(private ngxSmartModalService: NgxSmartModalService, private vcr: ViewContainerRef) {
    }

    isHidden = true;

    onOpenDelete() {
        const obj = {
            documentId : this.documentId,
            documentName : this.documentName
        }

        var deleteDocumentModal = this.ngxSmartModalService.create('deleteDocument', DeleteDocumentComponent, this.vcr, {customClass : 'bg-(--color-bgcard) !p-0 text-white rounded-lg border border-gray-500'});

        this.ngxSmartModalService.setModalData(
            obj ,
            'deleteDocument'
        );

        deleteDocumentModal.open();
    }

    @HostListener('document:click', ['$event'])
    onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;

    if( target.id != "custom-dropdown" && target.id != "three-dots" && this.isHidden == false){
        this.isHidden = true;
    }

    }
}