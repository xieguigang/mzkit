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