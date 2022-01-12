import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

raw   = open.mzpack("D:\web\Lu6-1.mzPack")
# gcxgc = GCxGC::extract_2D_peaks(raw)

# bitmap(plot(gcxgc, size = [4800,3300], padding = "padding: 250px 500px 250px 250px;"), file = "D:\web\Lu6-1.png")

# XIC
gcxgc = GCxGC::extract_2D_peaks(raw, mz = 43.036646)

bitmap(plot(gcxgc, size = [4800,3300], padding = "padding: 250px 500px 250px 250px;"), file = "D:\web\Lu6-1_XIC-43.0366.png")