export interface CreateDocumentRequest {
    workspaceId : string;
    fileName : string;
    description : string;
    file : File
}