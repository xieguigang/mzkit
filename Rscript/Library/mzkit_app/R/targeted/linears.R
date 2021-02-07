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

let output_datatables as function(quantify, linears, output_dir = "./") {
    print("dumping sample quantification result data:");

    result(quantify)     :> write.csv(file = `${output_dir}/quantify.csv`);
    scans.X(quantify)    :> write.csv(file = `${output_dir}/rawX.csv`);
    lines.table(linears) :> write.csv(file = `${output_dir}/linears.csv`);
    ionPeaks(quantify)   :> write.ionPeaks(file = `${output_dir}/ionPeaks.csv`);
}