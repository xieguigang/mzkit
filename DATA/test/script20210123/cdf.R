imports "netCDF.utils" from "base";

setwd(dirname(@script));

const demo_cdf = "../200ppm-scan.CDF";

# open netCDF data file for read agilent GC-MS targeted data
using cdf as open.netCDF(file = demo_cdf) {

    # show a list of global attribute information
    str(globalAttributes(cdf, list = TRUE));

    print("data variables in current experiment data file:");
    print(variables(cdf)[, "name"]);

    # TIC is a kind of time-signal chromatogram plot of
    # time vs. total ion current
    let scan_time = var(cdf, name = "scan_acquisition_time") :> getValue;
    let totalIons = var(cdf, name = "total_intensity") :> getValue;
    let TIC = data.frame(scan_time, totalIons);
    
    print("read TIC data from the raw data file:");
    str(TIC);

    # dump data and export to a excel table file
    # for downstream analysis,
    # example as GC-MS targeted quantification.
    write.csv(TIC, file = "./200ppm-scan_TIC.csv");
}