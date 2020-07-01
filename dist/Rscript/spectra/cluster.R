imports ["math", "mzkit.assembly"] from "mzkit";

let clusters = 
"D:\mzkit\DATA\test\GABA_mgf.txt"
:> read.mgf
:> mgf.ion_peaks
:> centroid
:> spectrum_tree.cluster()
;