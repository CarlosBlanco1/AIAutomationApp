import { InjectionToken } from "@angular/core";
import { DocumentService } from "./document-service.interface";

export const DOCUMENT_SERVICE = new InjectionToken<DocumentService>("DOCUMENT_SERVICE")