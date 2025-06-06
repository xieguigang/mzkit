require(mzkit);

imports "visual" from "mzplot";

let align = "83.2911_8.1879_0 94.2428_5.643_0 95.2263_22.4261_0 107.1712_8.9349_6.2 109.2841_17.2329_0 110.2164_10.5293_8.6 121.0951_5.243_23.9 125.0227_27.1455_0 134.3491_8.1041_0 135.1541_61.2358_46.9 136.1409_100_100 137.1_0_6.8 143.6789_7.5756_0 148.9523_10.8823_0 154.1169_43.1755_5.3";

svg(file = file.path(@dir, "align.svg")) {
    plot(parse.spectrum_alignment(align),
    legend_layout = "title", bar_width = 2);
}

bitmap(file = file.path(@dir, "align.jpeg")) {
    plot(parse.spectrum_alignment(align),
    legend_layout = "title", bar_width = 2);
}