import { map, Observable, tap } from "rxjs";
import { CreateDocumentRequest } from "../../models/Documents/create-document-request";
import { DocumentDto } from "../../models/Documents/document-dto";
import { DocumentService } from "./document-service.interface";
import { inject, signal } from "@angular/core";
import { HttpClient } from "@angular/common/http";

export class ApiDocumentService implements DocumentService {
    httpClient = inject(HttpClient)

    userDocuments = signal<DocumentDto[]>([])

    getUserDocuments(): Observable<DocumentDto[]> {
        return this.httpClient.get<DocumentDto[]>("http://localhost:8080/api/Documents/me")
            .pipe(tap(res => { this.userDocuments.set(res) }))
    }

    createDocument(request: CreateDocumentRequest): Observable<void> {
        return this.httpClient.post<DocumentDto[]>("http://localhost:8080/api/Documents", request)
            .pipe(map(() => void 0))
    }

}