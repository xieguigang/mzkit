
#' Read ion pairs data from MSL dataset
#'
#' @details this function will auto handling of the MSL ions
#'    data source of the raw dataset or raw file path of the
#'    ions data in MSL file format.
#' 
#' @param ions the MSL ions dataset or a file path for read MSL ions data.
#' @param unit the data unit of the time values that stored in 
#'    the MSL ions data, default is in unit of minute.
#'
#' @return A data collection of ion pairs dataset for run
#'    MRM data quantification.
#'
const ionPairsFromMsl as function(ions, unit = "Minute") {
    if (typeof(ions) == "string") {
        ions = ions 
        # the time unit is minute by default
        # required convert to second by 
        # specific that the time unit is Minute
        # at here
        |> read.msl(unit = unit) 
        ;
    }

    as.ion_pairs(ions);
}