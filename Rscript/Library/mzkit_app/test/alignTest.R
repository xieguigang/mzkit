require(mzkit);

export_dir = function(peakcache ) {
    let peakdata = NULL;
    let peakfile = NULL;
   

    for(file in list.files(peakcache, pattern = "*.csv")) {
        peakfile = load.csv(file, type = "peak_feature");
        peakdata = append(peakdata, peakfile);
    }

const mzdiff = "da:0.001";

    peakdata = peak_alignment(peakdata, mzdiff, norm = TRUE);
    peakdata = as.data.frame(peakdata);

    rownames(peakdata) = make.ROI_names(list(
        mz = peakdata$mz, 
        rt = peakdata$rt
    ));
    
    write.csv(peakdata, file = `${dirname(peakcache )}/peakdata.csv`, row.names = TRUE);
}

export_dir(peakcache = "W:\project\2022\QE_blood_20230105\20230317_large_batch_blood_sample\pos\raw\.cache\peaks");
export_dir(peakcache = "W:\project\2022\QE_blood_20230105\20230317_large_batch_blood_sample\neg\raw\.cache\peaks");