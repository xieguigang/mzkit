imports ["GCMS", "Linears"] from "mzkit.quantify";
imports "visualPlots" from "mzkit.quantify";

let plotLinears as function(linears, mslIons = NULL, output_dir = "./linears") {
    print("create linear plot output of your reference samples:");
    
    for (line in linears) {
        const linear_bitmap as string = `${output_dir}/${as.object(line)$name}.png`;
	
        bitmap(file = linear_bitmap) {
            visualPlots::standard_curve(line, gridFill = "white");
        }
    }
    
    if (!is.null(mslIons)) {
        for(ion in mslIons) {
            const linear_tableoutput = `${output_dir}/${as.object(ion)$id}.csv`;

            if (as.object(ion)$id != "IS") {                
                write.points(points(linears, ion), file = linear_tableoutput)
            }	
        }
    }
}

#' Output result data table
#' 
#' @details this function will output 4 tables in the target ``output_dir`` folder.
#'    the 4 tables that saved in target folder contains:
#'    
#'    + ``quantify.csv`` the linear quantify content result table
#'    + ``rawX.csv`` the raw peak area data table, if the metabolite has ``IS`` calibration, 
#'                   then the value of this table will be the peak area to IS peak area ratio 
#'                   values.
#'    + ``linears.csv`` the linear equation table.
#'    + ``ionPeaks.csv`` all ions peak ROI in all samples files, data in this table contains 
#'                       ``rt`` range and peak area, etc.
#' 
let output_datatables as function(quantify, linears, output_dir = "./") {
    print("dumping sample quantification result data:");

    result(quantify)     :> write.csv(file = `${output_dir}/quantify.csv`);
    scans.X(quantify)    :> write.csv(file = `${output_dir}/rawX.csv`);
    lines.table(linears) :> write.csv(file = `${output_dir}/linears.csv`);
    ionPeaks(quantify)   :> write.ionPeaks(file = `${output_dir}/ionPeaks.csv`);
}