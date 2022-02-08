import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

options(memory.loads = "max")

inputfile = "C:\MSI\GCxGC-plot\YCH-G-G-3.mzPack"
image_TIC = `${dirname(inputfile)}/${basename(inputfile)}_TIC2D.cdf`

raw   = open.mzpack(inputfile)
gcxgc = GCxGC::extract_2D_peaks(raw)

save.cdf(gcxgc, file = image_TIC)