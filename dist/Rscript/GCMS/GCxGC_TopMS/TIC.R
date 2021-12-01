imports ["GCxGC", "chromatogram"] from "mzkit";
imports "visual" from "mzplot";

options(strict = FALSE);

[Time, Intensity] = read.csv("E:\mzkit\DATA\test\GCxGC\TIC.csv", row.names = NULL);

bitmap(file = `${@dir}/TIC_1D.png`) {
	as.chromatogram(Time, Intensity)
	|> GCxGC::TIC2D(modtime = 5)
	|> GCxGC::TIC1D()
	|> plot()
	;
}

bitmap(file = `${@dir}/TIC.png`) {
	as.chromatogram(Time, Intensity)
	|> GCxGC::TIC2D(modtime = 5)
	|> plot()
	;
}

