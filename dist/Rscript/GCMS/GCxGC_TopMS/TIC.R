imports ["GCxGC", "chromatogram"] from "mzkit";

options(strict = FALSE);

[Time, Intensity] = read.csv("E:\mzkit\DATA\test\GCxGC\TIC.csv", row.names = NULL);

bitmap(file = `${@dir}/TIC.png`) {
	as.chromatogram(Time, Intensity)
	|> GCxGC::TIC2D(modtime = 5)
	|> plot()
	;
}

