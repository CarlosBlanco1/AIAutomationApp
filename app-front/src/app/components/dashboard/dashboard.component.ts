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

    userService = inject(USER_SERVICE)
    documentService = inject(DOCUMENT_SERVICE)

    constructor() {
        this.documentService.getUserDocuments().subscribe();

    }

    lineChartData = computed<ChartConfiguration['data']>(() => {

        var dateToDocFreq: Map<string, number> = new Map<string, number>();

        this.documentService.userDocuments().forEach(document => {
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

