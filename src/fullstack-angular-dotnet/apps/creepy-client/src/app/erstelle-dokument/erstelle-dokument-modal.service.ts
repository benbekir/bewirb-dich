import { EventEmitter, Injectable } from '@angular/core';
import { ErzeugeNeuesAngebotDto} from '../models/dokument';
import { HttpService } from '../services/http.service';

@Injectable({ providedIn: 'root' })
export class ErstelleDokumentModalService {
    private modal: any;
    public dokument: ErzeugeNeuesAngebotDto | undefined | any;
    public saved = new EventEmitter<void>()

    constructor(private http: HttpService) {

    }

  set(modal: any) {
    this.modal = modal;
  }

  remove() {
    this.modal = undefined;
  }

  open() {
    this.modal.open();
  }

  async close(save: boolean) {
    if(save) {
      await this.http.createDocument(this.dokument);
      this.saved.emit();
    }
    this.modal.close();
  }
}
