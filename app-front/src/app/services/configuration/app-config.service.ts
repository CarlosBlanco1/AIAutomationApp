import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { firstValueFrom } from "rxjs";

@Injectable({ providedIn: 'root' })
export class AppConfigService {
    httpClient = inject(HttpClient)

    config?: AppConfig;

    async load() {
        this.config = await firstValueFrom(this.httpClient.get<AppConfig>('/assets/config.json'))
    }

    get apiUrl() {
        if (!this.config) {
            throw new Error("No configuration file was loaded succesfully!");
        }
        else {
            return this.config.apiUrl;
        }
    }
}

export interface AppConfig {
    apiUrl: string;
}