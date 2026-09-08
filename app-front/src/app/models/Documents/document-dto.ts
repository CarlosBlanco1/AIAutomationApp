export interface DocumentDto {
    documentId : string;
    workspaceName : string;
    fileName : string;
    blobKey : string;
    fileSizeBytes : number;
    processingStatus : string;
    description : string;
    summary : string;
    createdAt : string;
}