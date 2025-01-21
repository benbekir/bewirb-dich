DROP TRIGGER IF EXISTS `TRG_on_insert_dokumente`;

DELIMITER $$
CREATE TRIGGER `TRG_on_insert_dokumente`
    BEFORE INSERT ON `dokumente` FOR EACH ROW
    BEGIN
        SET NEW.`uuid` = UUID_TO_BIN(UUID());
    END $$
DELIMITER ;