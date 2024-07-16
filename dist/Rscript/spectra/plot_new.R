require(mzkit);

imports "visual" from "mzplot";

let msms = "53.0397_0_7.41 65.0397_0_15.23 67.0553_0_21.52 81.0347_92.5371_12.28 95.0502_0_25.02 105.0346_0_8.06 123.0454_100_100 123.9021_24.5309_0";
let align = parse.spectrum_alignment(msms);

setwd(@dir);

svg(file = "./plot_msms.svg") {
    plot(align, legend_layout = "none",
    title = "blablabla",
    bar_width = 2,
    grid_x = TRUE,
    show_hits = TRUE);
}

bitmap(file = "./plot_msms.png") {
    plot(align, legend_layout = "none",
    title = "blablabla",
    bar_width = 2,
    grid_x = TRUE,
    show_hits = FALSE);
}