plot.ptw <- function(x, what = c("signal", "function"),
                     type = c("simultaneous", "individual"),
                     ask = TRUE, ...) {
  mode <- x$mode
  what <- match.arg(what)
  type <- match.arg(type)

  if (what == "function") {
    if (mode == "forward") {
      m <- 1
      ylbl <- "Warped 'time' - 'time'"
    } else {
      m <- -1
      ylbl <- "'Time' - warped 'time'"
    }
  }

  switch(type,
    simultaneous = {
      if (what == "signal") {
        matplot(t(rbind(x$reference, x$sample, x$warped.sample)),
          type = "n", xlab = "'Time'", ylab = "Signal",
          ...
        )
        matlines(t(x$reference), lty = 1, col = 1, ...)
        matlines(t(x$sample), lty = 1, col = 2, ...)
        matlines(t(x$warped.sample), lty = 1, col = 3, ...)
        legend("topleft", legend = c(
          "Reference", "Sample",
          "Warped sample"
        ), lty = rep(1, 3), col = 1:3)
      }
      if (what == "function") {
        time <- 1:ncol(x$warp.fun)
        w.time <- m * sweep(x$warp.fun, 2, time)
        matplot(time, t(w.time),
          type = "n", xlab = "'Time'",
          ylab = ylbl, ...
        )
        abline(h = 0, col = "gray", ...)
        matlines(t(w.time),
          lty = 1, type = "l", col = rainbow(nrow(x$warp.fun)),
          ...
        )
      }
    },
    {
      opar <- par(no.readonly = TRUE)
      on.exit(par(opar))
      par(ask = ask)
      if (what == "signal") {
        for (i in 1:nrow(x$sample)) {
          if (nrow(x$reference) == 1) {
            matplot(cbind(x$reference[1, ], x$sample[i, ], x$warped.sample[i, ]),
              type = "n", xlab = "'Time'",
              ylab = "Signal", ...
            )
          } else {
            matplot(cbind(x$reference[i, ], x$sample[i, ], x$warped.sample[i, ]),
              type = "n", xlab = "'Time'",
              ylab = "Signal", ...
            )
          }
          if (nrow(x$reference) == 1) {
            lines(x$reference[1, ], lty = 1, col = 1, ...)
          } else {
            lines(x$reference[i, ], lty = 1, col = 1, ...)
          }
          lines(x$sample[i, ], lty = 1, col = 2, ...)
          lines(x$warped.sample[i, ],
            lty = 1, col = 3,
            ...
          )
          legend("topleft", legend = c(
            "Reference", "Sample",
            "Warped sample"
          ), lty = rep(1, 3), col = 1:3)
        }
      }
      if (what == "function") {
        time <- 1:ncol(x$warp.fun)
        w.time <- m * sweep(x$warp.fun, 2, time)
        for (i in 1:nrow(x$warp.fun)) {
          plot(time, w.time[i, ],
            type = "n", xlab = "'Time'",
            ylab = ylbl, ...
          )
          abline(h = 0, col = "gray", ...)
          lines(time, w.time[i, ],
            col = rainbow(nrow(x$warp.fun))[i],
            ...
          )
        }
      }
    }
  )
}