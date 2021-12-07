#' Create peakms2 data schema model
#' 
.class_peakMs2 = function() {
    setClass("PeakMs2", representation(
        mz = "numeric",
        rt = "numeric",
        intensity = "numeric",
        file = "character",
        scan = "character",
        activation = "character",
        collisionEnergy = "numeric",
        lib_guid = "character",
        precursor_type = "character",
        meta = "list",
        mzInto = "mzInto"
    ));
}

#' Create peak matrix data schema model
#' 
.class_mzInto = function() {
    setClass("mzInto", representation(
        mz         = "numeric",
        intensity  = "numeric",
        annotation = "character"
    ));
}