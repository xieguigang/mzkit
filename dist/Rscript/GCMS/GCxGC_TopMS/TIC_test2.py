from mzkit import GCxGC, mzweb
from mzplot import visual

raw   = open.mzpack("F:\CONTROL-3_1.mzpack")
gcxgc = GCxGC::extract_2D_peaks(raw)

bitmap(plot(gcxgc), file = "F:\CONTROL-3_1.png")