import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

options(memory.loads = "max")

files = ["C:\MSI\GCxGC-plot\YCH-G-G-3.mzPack", "C:\MSI\GCxGC-plot\BYH-G-G-3.mzPack", "C:\MSI\GCxGC-plot\BYH-M-H-3.mzPack", "C:\MSI\GCxGC-plot\CDH-G-R-3-FC.mzPack"]

for inputfile in files:
  
    # inputfile = "C:\MSI\GCxGC-plot\YCH-G-G-3.mzPack"
    image_TIC = `${dirname(inputfile)}/${basename(inputfile)}_TIC2D.cdf`
    
    print(inputfile)

    raw   = open.mzpack(inputfile)
    gcxgc = GCxGC::extract_2D_peaks(raw)

    save.cdf(gcxgc, file = image_TIC)
    
    image_TIC = `${dirname(inputfile)}/${basename(inputfile)}.png`
    
    plt = plot(gcxgc, size = [5000,3300], padding = "padding: 250px 600px 300px 350px;", TrIQ = 1, colorSet = "viridis:inferno")

    bitmap(plt, file = image_TIC)