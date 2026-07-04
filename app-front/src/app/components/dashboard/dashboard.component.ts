import { Component, computed, inject, signal } from "@angular/core";
import { DocumentIconComponent } from "../../icons/document-icon.component";
import { EyeIconComponent } from "../../icons/eye-icon.component";
import { PointerRightIconComponent } from "../../icons/pointer-right-icon.component";
import { VerticalDotsIconComponent } from "../../icons/vertical-dots-icon.component";
import { USER_SERVICE } from "../../services/user/user-service.token";
import { DOCUMENT_SERVICE } from "../../services/document/document-service.token";
import { DocumentDto } from "../../models/Documents/document-dto";
import { UserDto } from "../../models/Users/user-dto";
import { ChartConfiguration, ChartType } from 'chart.js/auto';
import { BaseChartDirective } from 'ng2-charts';


@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    standalone: true,
    imports: [DocumentIconComponent, EyeIconComponent, VerticalDotsIconComponent, PointerRightIconComponent, BaseChartDirective]
})

export class DashboardComponent {

    // userService = inject(USER_SERVICE)
    // documentService = inject(DOCUMENT_SERVICE)

    constructor() {
        // this.documentService.getUserDocuments().subscribe();

    }

    user: UserDto = {
        id: "7d4d8d0e-4f5d-4d5d-9c7a-5f3b9d4e8c21",
        firstName: "Carlos",
        lastName: "Blanco",
        email: "carlos.blanco@example.com",
        createdAt: "2026-05-14T13:42:18Z"
    };

    documents: DocumentDto[] = [
        {
            documentId: "550e8400-e29b-41d4-a716-446655440001",
            workspaceName: "Research",
            fileName: "lion-habitat.pdf",
            blobKey: "docs/lion-habitat.pdf",
            fileSizeBytes: 254812,
            description: "Research on lion habitats.",
            fileText: "Lions inhabit grasslands and savannahs...",
            summary: "Overview of lion habitats in Africa.",
            createdAt: "2026-07-01T09:15:00Z"
        },
        {
            documentId: "550e8400-e29b-41d4-a716-446655440002",
            workspaceName: "Research",
            fileName: "climate-report.pdf",
            blobKey: "docs/climate-report.pdf",
            fileSizeBytes: 842193,
            description: "Annual climate report.",
            fileText: "Global temperatures have risen...",
            summary: "Summary of recent climate trends.",
            createdAt: "2026-07-02T08:45:00Z"
        },
        {
            documentId: "550e8400-e29b-41d4-a716-446655440003",
            workspaceName: "Research",
            fileName: "marine-life.pdf",
            blobKey: "docs/marine-life.pdf",
            fileSizeBytes: 493201,
            description: "Marine biodiversity.",
            fileText: "The oceans contain millions of species...",
            summary: "Introduction to marine ecosystems.",
            createdAt: "2026-07-02T16:20:00Z"
        },
        {
            documentId: "550e8400-e29b-41d4-a716-446655440004",
            workspaceName: "Finance",
            fileName: "budget-2026.xlsx",
            blobKey: "docs/budget-2026.xlsx",
            fileSizeBytes: 126740,
            description: "Budget planning.",
            fileText: "Projected expenses and revenue...",
            summary: "2026 financial planning document.",
            createdAt: "2026-07-03T10:10:00Z"
        },
        {
            documentId: "550e8400-e29b-41d4-a716-446655440005",
            workspaceName: "Finance",
            fileName: "tax-notes.pdf",
            blobKey: "docs/tax-notes.pdf",
            fileSizeBytes: 312988,
            description: "Tax preparation notes.",
            fileText: "Important deductions include...",
            summary: "Notes for filing taxes.",
            createdAt: "2026-07-03T15:40:00Z"
        },
        {
            documentId: "550e8400-e29b-41d4-a716-446655440006",
            workspaceName: "Personal",
            fileName: "travel-itinerary.pdf",
            blobKey: "docs/travel-itinerary.pdf",
            fileSizeBytes: 215472,
            description: "Vacation itinerary.",
            fileText: "Flight departs at 8:30 AM...",
            summary: "Trip schedule and reservations.",
            createdAt: "2026-07-04T11:05:00Z"
        },
        {
            documentId: "550e8400-e29b-41d4-a716-446655440007",
            workspaceName: "Personal",
            fileName: "book-notes.md",
            blobKey: "docs/book-notes.md",
            fileSizeBytes: 18342,
            description: "Notes from a recent book.",
            fileText: "Chapter 1 discusses...",
            summary: "Personal reading notes.",
            createdAt: "2026-07-04T18:25:00Z"
        },
        {
            documentId: "550e8400-e29b-41d4-a716-446655440008",
            workspaceName: "Personal",
            fileName: "book-notes.md",
            blobKey: "docs/book-notes.md",
            fileSizeBytes: 18342,
            description: "Notes from a recent book.",
            fileText: "Chapter 1 discusses...",
            summary: "Personal reading notes.",
            createdAt: "2026-07-04T18:25:00Z"
        }
    ];

    lineChartData = computed<ChartConfiguration['data']>(() => {

        var dateToDocFreq: Map<string, number> = new Map<string, number>();

        this.documents.forEach(document => {
            var dateObj = new Date(document.createdAt)
            var dateString = dateObj.toDateString().match(/^.{4}(.*)$/)![1]

            dateToDocFreq.set(dateString, (dateToDocFreq.get(dateString) ?? 0) + 1)
        });


        return {
            labels: [...dateToDocFreq.keys()],
            datasets: [
                {
                    label: 'Documents',
                    data: [...dateToDocFreq.values()],
                    tension: 0.3,
                    borderColor: '#9810fa'
                }
            ]
        }
    });

    lineChartOptions: ChartConfiguration['options'] = {
        responsive: true,
        plugins: {
            legend: {
                display: false
            },
            tooltip: {
                enabled: true
            }
        },
        maintainAspectRatio: false,
        scales: {
            y: {
                ticks: {
                    maxTicksLimit: 5,
                    color: '#b4b4b5',
                    stepSize : 1
                }
            },
            x: {
                ticks: {
                    color: '#b4b4b5'
                }
            }
        }
    };

    lineChartType: ChartType = 'line';

}

