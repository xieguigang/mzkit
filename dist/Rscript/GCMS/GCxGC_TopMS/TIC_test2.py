import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

options(memory.loads = "max")

inputfile = "C:\MSI\GCxGC-plot\YCH-G-G-3.mzPack"
image_TIC = `${dirname(inputfile)}/${basename(inputfile)}_TIC2D.png`

raw   = open.mzpack(inputfile)
gcxgc = GCxGC::extract_2D_peaks(raw)
plt = plot(gcxgc, size = [4800,3300], padding = "padding: 250px 500px 250px 250px;", TrIQ = 1)

bitmap(plt, file = image_TIC)

# XIC
# gcxgc = GCxGC::extract_2D_peaks(raw, mz = 43.036646)
# plt = plot(gcxgc, size = [4800,3300], padding = "padding: 250px 500px 250px 250px;")

# bitmap(plt, file = "D:\web\Lu6-1_XIC-43.0366.png")