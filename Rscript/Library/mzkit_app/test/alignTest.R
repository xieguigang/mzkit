require(mzkit);

const mzdiff = "da:0.001";

export_dir = function(peakcache ) {
const peakdata = alignment_peaksdata(peakcache, mzdiff);
    
    write.csv(peakdata, file = `${dirname(peakcache )}/peakdata.csv`, row.names = TRUE);
}

export_dir(peakcache = "W:\project\2022\QE_blood_20230105\20230317_large_batch_blood_sample\pos\raw\.cache\peaks");
export_dir(peakcache = "W:\project\2022\QE_blood_20230105\20230317_large_batch_blood_sample\neg\raw\.cache\peaks");