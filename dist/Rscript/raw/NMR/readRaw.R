imports "package_utils" from "devkit";

package_utils::attach("D:\mzkit\Rscript\Library\mzkit_app");

imports "NMR" from "mzplot";
imports "NMR" from "mzkit";

# "E:\\mzkit\\DATA\\nmr\\HMDB00005.nmrML"
# |> NMR::read.nmrML
# |> acquisition
# |> FID
# |> print
# ;

rawdata = "\mzkit\DATA\nmr\HMDB00005.nmrML"
|> NMR::read.nmrML()
;

for(m in spectrumList(rawdata)) {
	data = spectrum(m, rawdata);
	
	bitmap(file = `${@dir}/demo_plotNMR.png`) {
		NMR::plot_nmr(data);
	}
}

