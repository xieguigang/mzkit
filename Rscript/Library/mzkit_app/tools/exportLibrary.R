require(mzkit);

imports "spectrumTree" from "mzkit";

let libfile = ?"--lib" || stop("the file path of the target library pack is required!");
let ion = ?"--ion" || "Unknown";
let export_mgf = ?"--out_mgf" || file.path(dirname(libfile), `${basename(libfile)}.mgf`);
let libpack = spectrumTree::readpack(file = libfile);
let spectrum = spectrumTree::export_spectrum(libpack, ionMode = ion);

spectrum |> write.mgf(file = export_mgf);

