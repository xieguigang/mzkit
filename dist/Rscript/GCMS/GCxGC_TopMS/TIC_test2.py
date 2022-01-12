import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

raw   = open.mzpack("D:\web\Lu6-1.mzPack")
gcxgc = GCxGC::extract_2D_peaks(raw)

bitmap(plot(gcxgc), file = "D:\web\Lu6-1.png")