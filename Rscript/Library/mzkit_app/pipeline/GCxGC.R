require(mzkit);

imports "netCDF.utils" from "base";
imports "mzweb" from "mzkit";
imports ["GCxGC", "chromatogram"] from "mzkit";
imports "visual" from "mzplot";

#' title: workflow script for processing GCxGC raw data file
#' 
#' author: xieguigang <gg.xie@bionovogene.com> 
#' description: workflow script for processing GCxGC raw data 
#'    file. pre-processing of the raw data files includes:
#'    convert cdf file to mzpack file format, and extract GCxGC
#'    2D TIC matrix data into a cdf file, and run rendering of 
#'    the GCxGC 1D/2D TIC data.

[@info "the file path of the GCxGC raw data file, or a folder path that contains multiple cdf raw data files."]
const cdfpath   = ?"--cdf"       || stop("no cdf file provided!");
[@info "the folder path for contains the result raw data files."]
const outputdir = ?"--outputdir" || `${dirname(cdfpath)}/processed/`;
[@info "GCxGC modtime in time unit seconds."]
const modtime   = ?"--modtime"   || 5;
[@info "the color palette name for do GCxGC 2D TIC rendering."]
const palette   = ?"--palette"   || "viridis:turbo";

if (!(file.exists(cdfpath) || dir.exists(cdfpath))) {
	stop(`missing raw data file at location: ${cdfpath}!`);
}

#' Process cdf raw data file
#' 
#' @param cdf_src the file path of the GCxGC cdf raw data file.
#' 
processCdfFile = function(cdf_src) {
    mzfile = `${outputdir}/mzpack/${basename(cdf_src)}.mzpack`;
    mzpack = NULL;

    print(`processing '${basename(cdf_src)}'!`);
    print("open raw data cdf file for read and then convert to mzpack raw data file...");

    using cdf as open.netCDF(cdf_src) {
        mzpack = cdf |> as.mzpack(modtime = modtime);
        mzpack       |> write.mzPack(file = mzfile);	
    }

    print("extract GCxGC 2d TIC data...");

    # extract TIC data
    image_TIC = `${outputdir}/TIC/${basename(cdf_src)}.cdf`;
    gcxgc = GCxGC::extract_2D_peaks(mzpack);
    save.cdf(gcxgc, file = image_TIC);
    image_TIC = `${outputdir}/TIC/${basename(cdf_src)}.png`;
    image_1D = `${outputdir}/TIC/${basename(cdf_src)}_1D.png`;

    print("rendering...");

    bitmap(file = image_TIC, size = [5000,3300]) {
        plot(gcxgc, 
            padding  = "padding: 250px 600px 300px 350px;", 
            TrIQ     = 1, 
            colorSet = palette
        );
    }

    # plot 1D TIC
    bitmap(file = image_1D) {
        plot(GCxGC::TIC1D(gcxgc));
    }    
	
    print("~~done!");
}

if (dir.exists(cdfpath)) {
    for(file in list.files(cdfpath, pattern = "*.cdf")) {
        processCdfFile(cdf_src = file);
    }
} else {
    processCdfFile(cdf_src = cdfpath);
}