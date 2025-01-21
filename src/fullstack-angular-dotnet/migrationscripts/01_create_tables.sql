-- manuell geschriebene migrationen da ich nur erfahrung mit custom "migration managern" habe.
-- ich habe also keine erfahrung mit dem EF Core migration feature (https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations)
-- ansonsten hätte man auch dies nutzen können für einen code-first ansatz

CREATE TABLE `dokumente` (
    `id` INT PRIMARY KEY AUTO_INCREMENT,
    `uuid` BINARY(16) NOT NULL,
    `berechnungsbasis` DECIMAL(12,2) NOT NULL,
    `inkludiere_zusatzschutz` BOOLEAN NOT NULL,
    `zusatzschutz_aufschlag` FLOAT NOT NULL,
    `hat_webshop` BOOLEAN NOT NULL,
    `beitrag` DECIMAL(12,2) NOT NULL,
    `versicherungsschein_ausgestellt` BOOLEAN NOT NULL DEFAULT FALSE,
    `versicherungssumme` DECIMAL(12,2) NOT NULL,
    `dokument_typ` VARCHAR(36) NOT NULL,
    `berechnungsart` VARCHAR(36) NOT NULL,
    `risiko` VARCHAR(36) NOT NULL
);