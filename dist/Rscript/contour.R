imports "mzweb" from "mzkit";
imports "visual" from "mzkit.plot";

bitmap(file = `${dirname(@script)}/contour.png`) {
	"E:\test.mzPack"
	:> open.mzpack
	:> ms1_scans
	:> raw_scatter(contour = TRUE)
	;
}
