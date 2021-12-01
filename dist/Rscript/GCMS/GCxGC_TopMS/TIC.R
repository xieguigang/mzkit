imports ["GCxGC", "chromatogram"] from "mzkit";
imports "visual" from "mzplot";

options(strict = FALSE);

[Time, Intensity] = read.csv("E:\mzkit\DATA\test\GCxGC\TIC.csv", row.names = NULL);

# plot dimension 1 TIC
bitmap(file = `${@dir}/TIC_1D.png`) {
	as.chromatogram(Time, Intensity)
	|> GCxGC::TIC2D(modtime = 5)
	|> GCxGC::TIC1D()
	|> plot()
	;
}

bitmap(file = `${@dir}/TIC_d2_100th.png`) {
	d2 = as.chromatogram(Time, Intensity)
	|> GCxGC::TIC2D(modtime = 5)
	;
	
	# pick 100th dimension 2 TIC plot
	plot(d2[100]);
}

# plot GCxGC 2D TIC matrix heatmap
bitmap(file = `${@dir}/TIC.png`, size = [6000, 3000]) {
	as.chromatogram(Time, Intensity)
	|> GCxGC::TIC2D(modtime = 5)
	|> plot(
		padding = "padding: 250px 600px 300px 300px;",
		xlab = "Dimension 1 RT(s)",
		ylab = "Dimension 2 RT(s)"
	)
	;
}

