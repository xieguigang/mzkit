#Region "Microsoft.ROpen::38808013a2bc47ab970082f6c17199f1, utils.R"

    # Summaries:

    # create_filter.skips <- function(kegg_id.skips, debug.echo = TRUE) {if (kegg_id.skips %=>% IsNothing) {...
    # filter.skips <- function(partners) {if (partners %=>% IsNothing) {...

#End Region

#' Create skips handler for KEGG id
#'
#' @param kegg_id.skips A character vector that contains KEGG id
#'   want to ignores in the metaDNA prediction process.
#'
#' @return This function returns a lambda function that can determine the
#'   given kegg id vector which is not in the input \code{kegg_id.skips}.
#'
create_filter.skips <- function(kegg_id.skips, debug.echo = TRUE) {

    if (kegg_id.skips %=>% IsNothing) {
        kegg_id.skips = "NA";
    } else if (debug.echo) {
        cat("\nThese KEGG compound will not be identified from metaDNA\n\n");
        print(kegg_id.skips);
        cat("\n");
    }

    kegg_id.skips <- as.index(kegg_id.skips);
    filter.skips <- function(partners) {
        if (partners %=>% IsNothing) {
            NULL;
        } else {

            # if the partner id is in the skip list
            # then it will be replaced as string "NA"
            partners <- sapply(partners, function(id) {
                if (kegg_id.skips(id)) {
                    "NA";
                } else {
                    id;
                }
            }) %=>% as.character;

            # Removes those NA string in the partner id list
            partners[partners != "NA"];
        }
    }

    filter.skips;
}
