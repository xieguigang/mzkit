imports "app" from "ServiceHub";

require(MSImaging);
require(mzkit);
require(ggplot);

options(memory.load = "max");

const appPort as integer = ?"--app"    || stop("A MSimaging services hub app handle must be provided!");
const mz as string       = ?"--mzlist" || stop("target ions list must be provided!");
const mzdiff as string   = ?"--mzdiff" || "da:0.1";
const savefile as string = ?"--save"   || stop("A file path to save plot image must be specificed!");
const mzlist as double   = mz
|> strsplit(",", fixed = TRUE)
|> unlist()
|> as.numeric()
;
const pixelsData = app::getMSIData(
    MSI_service = appPort, 
    mz          = mzlist, 
    mzdiff      = mzdiff
);

print(`load ${length(pixelsData)} pixels data from given m/z:`);
print(mzlist);

bitmap(file = savefile, size = [3300, 2000]) {
    
    # load mzpack/imzML raw data file
    # and config ggplot data source driver 
    # as MSImaging data reader
    ggplot(pixelPack(pixelsData), 
           mapping = aes(), 
           padding = "padding: 200px 600px 200px 250px;"
    ) 
       # rendering of a single ion m/z
       # default color palette is Jet color set
       + geom_msimaging(mz = mzlist[1], tolerance = mzdiff)
       # add ggplot charting elements
       + ggtitle(`MSImaging of m/z ${round(mzlist[1], 4)}`)
       + labs(x = "Dimension(X)", y = "Dimension(Y)")
       + scale_x_continuous(labels = "F0")
       + scale_y_continuous(labels = "F0")
    ;
}