## actual warping of the sample intensities for continuous data.
warp.sample <- function(samp, w, mode) {
  if (mode == "backward") {
    apply(samp, 1, function(x) ##interpol(w, x))
      approx(x, NULL, w)$y)
  } else {
    apply(samp, 1, function(x) {
      approx(w, x, xout = 1:length(x))$y
    })
  }
}
