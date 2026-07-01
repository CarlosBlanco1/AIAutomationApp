import { Observable } from "rxjs";
import { DocumentDto } from "../../models/Documents/document-dto";
import { CreateDocumentRequest } from "../../models/Documents/create-document-request";

export interface DocumentService {
    userDocuments() : DocumentDto[];
    getUserDocuments() : Observable<DocumentDto[]>;
    createDocument(request : CreateDocumentRequest) : Observable<void>;
    deleteDocument(documentId : string) : Observable<void>;
    getDownloadUrl(documentId : string) : Observable<{downloadUrl : string}>;
}