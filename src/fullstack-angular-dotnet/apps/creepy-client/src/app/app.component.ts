import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import {ErstelleDokumentModalService} from "./erstelle-dokument/erstelle-dokument-modal.service";
import {tap} from "rxjs";
import {DokumentenlisteEintragDto} from "./models/dokument";
import {CommonModule} from "@angular/common";
import {ErstelleDokumentModal} from "./erstelle-dokument/erstelle-dokument-modal.component";
import { HttpService } from './services/http.service';

@Component({
  standalone: true,
  imports: [CommonModule, RouterModule, ErstelleDokumentModal],
  selector: 'fullstack-angular-dotnet-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  dokumente: DokumentenlisteEintragDto[] = [];

  selectedDocument: DokumentenlisteEintragDto | undefined

  constructor(public http: HttpService, private eDMService: ErstelleDokumentModalService ) {
    eDMService.saved.pipe(tap(() => {
      this.ladeDokumente()
    })).subscribe()
    this.ladeDokumente()
  }

  openDocumentCreation() {
    this.eDMService.open();
  }

  selectDocument(dokument: DokumentenlisteEintragDto) {
    this.selectedDocument = dokument;
  }

  async ladeDokumente() {
    this.dokumente = (await this.http.getDocuments()) || [];
  }

  async selectedDocumentAnnehmen() {
    if(this.selectedDocument) {
      await this.http.acceptDocument(this.selectedDocument.id);
      this.selectedDocument = undefined;
      await this.ladeDokumente();

    }
  }
  async selectedDocumentAusstellen() {
    if(this.selectedDocument) {
      await this.http.exportDocument(this.selectedDocument.id);
      this.selectedDocument = undefined;
      await this.ladeDokumente();
    }
  }
}
