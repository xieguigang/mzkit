imports "spectrumPool" from "mzDIA";
imports "mzweb" from "mzkit";

setwd(@dir);

using pool as spectrumPool::openPool("./cluster_1", level = 0.9, split = 15) {
    const raw = "demo.mgf"
    |> open.mzpack()
    |> ms2_peaks()
    ;

    addPool(pool, raw);
}