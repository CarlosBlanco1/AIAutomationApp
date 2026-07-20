import { map, Observable, tap } from "rxjs";
import { CreateDocumentRequest } from "../../models/Documents/create-document-request";
import { DocumentDto } from "../../models/Documents/document-dto";
import { DocumentService } from "./document-service.interface";
import { inject, signal } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { AppConfigService } from "../configuration/app-config.service";

export class ApiDocumentService implements DocumentService {
    private baseUrl? : string;

    private readonly httpClient = inject(HttpClient)
    private readonly configService = inject(AppConfigService);

    constructor() {
        this.baseUrl = `${this.configService.apiUrl}/api/Document`;
    }

    userDocuments = signal<DocumentDto[]>([])

    getSingleDocument(documentId : string) : Observable<DocumentDto> {
        return this.httpClient.get<DocumentDto>(`${this.baseUrl}/single-doc/${documentId}`)
    }
    
    getDownloadUrl(documentId: string): Observable<{downloadUrl : string}> {
        return this.httpClient.get<{ downloadUrl: string; }>(`${this.baseUrl}/download-url/${documentId}`)
    }
    
    deleteDocument(documentId: string): Observable<void> {
        return this.httpClient.delete(`${this.baseUrl}/${documentId}`).pipe(map(() => void 0))
    }

    getUserDocuments(): Observable<DocumentDto[]> {
        return this.httpClient.get<DocumentDto[]>(`${this.baseUrl}/me`)
            .pipe(tap(res => { this.userDocuments.set(res) }))
    }

    createDocument(request: CreateDocumentRequest): Observable<void> {

        const formData = new FormData();

        formData.append('workspaceId', request.workspaceId);
        formData.append('fileName', request.fileName);
        formData.append('description', request.description);
        formData.append('file', request.file, request.file.name)

        return this.httpClient.post<DocumentDto[]>(`${this.baseUrl}`, formData)
            .pipe(map(() => void 0))
    }

}