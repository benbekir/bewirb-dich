using CreepyApi.Database.Models;

namespace CreepyApi.Helpers;

public static class DocumentBeitragHelper
{
    // NOTE: um den "Open/Closed" Aspekt der SOLID Prinzipien einzuhalten, sollte man hier noch für jede Berechnungsart eine eigene Klasse implementieren, die den jeweiligen Fall behandelt.
    // -> also eine Klasse welche die Logik für den Fall "Haushaltssumme" implementiert, eine weitere für den Fall "AnzahlMitarbeiter", und eine dritte für "Umsatz".
    // dann könnte man von hier aus einfach die korrekte methode aufrufen für das aktuelle dokument.
    // für diese implementierung habe ich aber leider keine zeit mehr gehabt :(
    public static void Calculate(Document document)
    {
        decimal beitrag;
        //Versicherungsnehmer, die nach Haushaltssumme versichert werden (primär Vereine) stellen immer ein mittleres Risiko da
        if (document.Berechnungsart == Berechnungsart.Haushaltssumme)
        {
            document.Risiko = Risiko.Mittel;
            decimal faktorHaushaltssumme = (decimal)Math.Log10((double)document.Versicherungssumme);
            beitrag = (1.0m + faktorHaushaltssumme * document.Berechnungbasis + 100m) * 1.2m;
        }
        //Versicherungsnehmer, die nach Anzahl Mitarbeiter abgerechnet werden und mehr als 5 Mitarbeiter haben, können kein Lösegeld absichern
        else if (document.Berechnungsart == Berechnungsart.AnzahlMitarbeiter)
        {
            if (document.Berechnungbasis > 5)
            {
                document.InkludiereZusatzschutz = false;
                document.ZusatzschutzAufschlag = 0;
            }

            decimal faktorMitarbeiter = document.Versicherungssumme / 1000;
            if (document.Berechnungbasis < 4)
            {
                beitrag = faktorMitarbeiter + document.Berechnungbasis * 250m;
            }
            else
            {
                beitrag = faktorMitarbeiter + document.Berechnungbasis * 200m;
            }

            if (document.Risiko == Risiko.Mittel)
            {
                beitrag *= 1.3m;
            }
        }
        //Versicherungsnehmer, die nach Umsatz abgerechnet werden, mehr als 100.000€ ausweisen und Lösegeld versichern, haben immer mittleres Risiko
        else if (document.Berechnungsart == Berechnungsart.Umsatz)
        {
            decimal faktorUmsatz = (decimal)Math.Pow((double)document.Versicherungssumme, 0.25d);
            beitrag = 1.1m + faktorUmsatz * (document.Berechnungbasis / 100000);
            //Webshop gibt es nur bei Unternehmen, die nach Umsatz abgerechnet werden
            if (document.HatWebshop)
            {
                beitrag *= 2;
            }

            if (document.Berechnungbasis > 100000m && document.InkludiereZusatzschutz)
            {
                document.Risiko = Risiko.Mittel;
                beitrag *= 1.2m;
            }
        }
        else
        {
            throw new Exception();
        }

        if (document.InkludiereZusatzschutz)
        {
            beitrag *= 1.0m + (decimal)document.ZusatzschutzAufschlag / 100.0m;
        }

        document.Berechnungbasis = Math.Round(document.Berechnungbasis, 2);
        document.Beitrag = Math.Round(beitrag, 2);
    }
}
