require(mzkit);

imports ["GCxGC", "mzweb"] from "mzkit";
imports "visual" from "mzplot";

inputfile = ?"--2d_cdf" || stop("no source data!");

image_TIC = `${dirname(inputfile)}/${basename(inputfile)}.png`;
image_TIC1D = `${dirname(inputfile)}/${basename(inputfile)}_1D.png`;

gcxgc   = read.cdf(inputfile);
# gcxgc = GCxGC::extract_2D_peaks(raw)
plt = plot(gcxgc, size = [5000,3300], padding = "padding: 250px 600px 300px 350px;", TrIQ = 1, colorSet = "viridis:turbo");

bitmap(plt, file = image_TIC);

tic = GCxGC::TIC1D(gcxgc);
plt = plot(tic);

bitmap(plt, file = image_TIC1D );