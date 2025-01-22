import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DokumentenlisteEintragDto, ErzeugeNeuesAngebotDto } from '../models/dokument';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})

export class HttpService {
  constructor(public http: HttpClient) { }
  
  async getDocuments() {
    return await this.http.get<DokumentenlisteEintragDto[]>(environment.baseurl + '/dokumente').toPromise();
  }

  async acceptDocument(id: string) {
    return await this.http.put<void>(environment.baseurl + '/dokumente/' + id + '/annehmen', null).toPromise();
  }

  async exportDocument(id: string) {
    return await this.http.put<void>(environment.baseurl + '/dokumente/' + id + '/ausstellen', null).toPromise();
  }

  async createDocument(dokument: ErzeugeNeuesAngebotDto) {
    return await this.http.post(environment.baseurl + '/dokumente', {...dokument}).toPromise();
  }
}