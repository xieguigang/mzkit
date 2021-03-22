imports "mzweb" from "mzkit";
imports "visual" from "mzkit.plot";

# demo test of open an mzPack binary file
const file as string = ?"--raw" || stop("no raw data file location was specific!");

bitmap(file = `${dirname(file)}/${basename(file)}.raw3D.png`) {
	file 
	:> open.mzpack
	:> ms1_scans
	:> raw_snapshot3D(size = [2400, 1200], noise_cutoff = 0.85)
	;
}
