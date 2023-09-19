require(mzkit);

imports "assembly" from "mzkit";
imports "formula" from "mzkit";

let spectrum = `${@dir}/leucine.mgf`
|> read.mgf()
|> mgf.ion_peaks(lazy = FALSE)
;

for(ion in spectrum) {
    let iondata = as.list(ion);
    
    ion = formula::peakAnnotations(
        ion, "C6H13NO2", iondata$precursor_type
    );

    str(ion);
    stop();
}