require(mzkit);

imports "assembly" from "mzkit";

let spectrum = `${@dir}/leucine.mgf`
|> read.mgf()
|> mgf.ion_peaks(lazy = FALSE)
;

for(ion in spectrum) {
    str(ion);
}