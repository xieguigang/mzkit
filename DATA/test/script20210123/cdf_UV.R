imports "netCDF.utils" from "base";

setwd(dirname(@script));

const demo_cdf = "./demo_UVscans.cdf";

# open netCDF data file for read agilent GC-MS targeted data
using cdf as open.netCDF(file = demo_cdf) {

    # show a list of global attribute information
    str(globalAttributes(cdf, list = TRUE));

    print("data variables in current experiment data file:");
    print(variables(cdf)[, "name"]);
  
    # PDA is a kind of TIC plot liked time-signal chromatogram plot of
    # time vs. total ion current
    let scan_time = var(cdf, name = "meta:scan_time") :> getValue;
    let totalIons = var(cdf, name = "meta:total_ion_current") :> getValue;
    let PDA = data.frame(scan_time, totalIons);
    
    print("read PDA data from the raw data file:");
    str(PDA);

    # dump data and export to a excel table file
    # for downstream analysis,
    # example as GC-MS targeted quantification.
    write.csv(PDA, file = "./demo_UV_PDA.csv");
}