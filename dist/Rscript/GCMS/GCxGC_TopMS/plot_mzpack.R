require(mzkit);

imports "mzweb" from "mzkit";
imports "GCxGC" from "mzkit";

setwd(@dir);

let raw   = open.mzpack( "D:/XXX.mzPack" );
let gcxgc = GCxGC::extract_2D_peaks(raw);

bitmap(file = "./GCxGC_TIC2D.png") {
   plot(gcxgc, size = [4800,3300], padding = "padding: 250px 500px 250px 250px;", TrIQ = 1);
};