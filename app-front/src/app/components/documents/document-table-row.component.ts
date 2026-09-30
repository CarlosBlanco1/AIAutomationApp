import { afterNextRender, Component, ElementRef, EventEmitter, HostListener, inject, Injector, Input, Output, signal, ViewChild, ViewContainerRef, WritableSignal } from "@angular/core";
import { HorizontalDotsIconComponent } from "../../icons/horizontal-dots-icon.component";
import { InfoIconComponent } from "../../icons/info-icon.component";
import { TrashIconComponent } from "../../icons/trash-icon.component";
import { NgxSmartModalService } from "ngx-smart-modal";
import { DeleteDocumentComponent } from "./delete-document/delete-document.component";
import { RouterLink } from "@angular/router";
import { StatsuPillComponent } from "./state-pill/status-pill.component";
import { UserCircleComponent } from "../../icons/user-circle-icon.component";
import { LockIconComponent } from "../../icons/lock-icon.component";

@Component({
    selector: 'tr[app-document-table-row]',
    templateUrl: './document-table-row.component.html',
    imports: [HorizontalDotsIconComponent, InfoIconComponent, TrashIconComponent, RouterLink, StatsuPillComponent, UserCircleComponent, LockIconComponent]
})
export class DocumentTableRowComponent {
    @Input({ required: true }) documentName!: string;
    @Input({ required: true }) documentId!: string;
    @Input({ required: true }) documentSubtitle!: string;
    @Input({ required: true }) documentCategory!: string;
    @Input({ required: true }) minutesAgoEdited!: number;
    @Input({ required: true }) processingStatus!: string;
    @Input({ required: true }) containerBoundary!: HTMLElement;

    constructor(private ngxSmartModalService: NgxSmartModalService, private vcr: ViewContainerRef) {
    }

    @ViewChild('customDropdown') dropdown!: ElementRef<HTMLElement>;

    openUpwards = signal<boolean>(false);
    private readonly injector = inject(Injector);


    onSelectDropdown() {
        this.menuOpened.emit();

        afterNextRender(() => {
            const rect = this.dropdown.nativeElement.getBoundingClientRect();
            const containerRect = this.containerBoundary.getBoundingClientRect();

            this.openUpwards.set(rect.bottom > containerRect.bottom);
        },
            { injector: this.injector }
        )
    }

    onOpenDelete() {
        const obj = {
            documentId: this.documentId,
            documentName: this.documentName
        }

        var deleteDocumentModal = this.ngxSmartModalService.create('deleteDocument', DeleteDocumentComponent, this.vcr, { customClass: 'bg-(--color-bgcard) !p-0 text-white rounded-lg border border-gray-500' });

        this.ngxSmartModalService.setModalData(
            obj,
            'deleteDocument'
        );

        deleteDocumentModal.open();
    }

    @Input({ required: true }) isMenuVisible!: boolean;
    @Output() menuOpened = new EventEmitter<void>();
}