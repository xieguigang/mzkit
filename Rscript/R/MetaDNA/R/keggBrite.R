KEGG.brites <- function() {
    list(
        br08001 = 'Compounds with biological roles',
        br08002 = 'Lipids',
        br08003 = 'Phytochemical compounds',
        br08021 = 'Glycosides',
        br08005 = 'Bioactive peptides',
        br08006 = 'Endocrine disrupting compounds',
        br08007 = 'Pesticides',
        br08008 = 'Carcinogens',
        br08009 = 'Natural toxins',
        br08010 = 'Target-based classification of compounds'
    );
}

append.KEGG_brite <- function(table, col = "KEGG", name = "*") {
    brite_names <- KEGG.brites();

    if (name == "*") {
        # all
        # do nothing
        table.list <- brite_names;
    } else {
        table.list <- list();

        for(id in name) {
            table.list[[id]] <- brite_names[[id]];
        }
    }

    xLoad("KEGG_brites.rda");

    table.keggIndex <- table[, col] %=>% as.vector;
    # rownames(table) <- table[, "ID"];

    for(id in names(table.list)) {
        brites <- get(id);
        brites[, "entry"] <- NULL;
        brites[, "name"]  <- NULL;
        brites <- .as.list(brites);
        names(brites) <- sapply(brites, function(t) t$ID);

        append.brites <- list();
        empty <- rep("NULL", length(brites[[1]]));
        names(empty) <- names(brites[[1]]);

        i <- 1

        for(key in table.keggIndex) {
            r <- brites[[key]];

            if (is.null(r)) {
                append.brites[[sprintf("X%s", i)]] <- empty;
            } else {
                append.brites[[sprintf("X%s", i)]] <- r;
            }

            i <- i + 1;
        }

        right <- NULL;

        # fix for Error in write.table(table, file = "S:\\mzCloudPlants\\MetaCluster\\DD2019041513001-<U+8336><U+6811>\\doMSMSalignment.report1_kegg_class.csv",  :
        # unimplemented type 'list' in 'EncodeElement'
        for(col in names(empty)) {
            right <- cbind(right, as.vector(sapply(append.brites, function(r) r[[col]])));
        }

        append.brites <- right;

        cols <- names(empty);
        cols <- sprintf("[%s].%s", brite_names[[id]], cols);
        colnames(append.brites) <- cols;
        rownames(append.brites) <- NULL;

        print(head(append.brites));

        table <- cbind(table, append.brites);
    }

    table;
}
