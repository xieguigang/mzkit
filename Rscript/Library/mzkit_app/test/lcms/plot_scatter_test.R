require(mzkit);

let peakmeta = read.csv("F:\peakmeta.csv", row.names = 1, check.names = FALSE);

setwd(@dir);

print(peakmeta, max.print = 6);

    bitmap(file = "peakset.png") {
        plot(as.peak_set(peakmeta), scatter = TRUE, 
            dimension = "npeaks");
    }