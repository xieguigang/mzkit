.class_tolerance = function() {
    setClass("mzdiff", representation(
        threshold         = "numeric",
        method            = "character",
        massErr           = "function",
        assert            = "function",
        is.low.resolution = "logical",
        toString          = "character"
    ));
}

.class_precursor = function() {
    setClass("PrecursorType", representation(
        Name   = "character",
        calc   = "function",
        charge = "numeric",
        M      = "numeric",
        adduct = "numeric",
        cal.mz = "function"
    ));
}