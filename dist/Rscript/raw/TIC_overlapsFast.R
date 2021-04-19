imports ["assembly", "chromatogram"] from "mzkit";
imports "visual" from "mzkit.plot";

const rawInput as string  = ?"--input" || stop("require an ``--input`` argument for specific the input data directory!");

bitmap(file = ?"--png" || `${rawInput}/TIC.png`) {
	rawInput 
	:> list.files( pattern = "*.mzXML" ) 
	:> lapply(load_index, names = basename) 
	:> chromatogram::overlaps 
	:> plot(parallel = TRUE)
	;
}