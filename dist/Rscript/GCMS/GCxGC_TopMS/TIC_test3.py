import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

options(memory.loads = "max")

files = ["C:\MSI\GCxGC-plot\YCH-G-G-3_TIC2D.cdf","C:\MSI\GCxGC-plot\BYH-G-G-3_TIC2D.cdf","C:\MSI\GCxGC-plot\BYH-M-H-3_TIC2D.cdf","C:\MSI\GCxGC-plot\CDH-G-R-3-FC_TIC2D.cdf"]

for inputfile in files:
    image_TIC = `${dirname(inputfile)}/${basename(inputfile)}.png`
    image_TIC1D = `${dirname(inputfile)}/${basename(inputfile)}_1D.png`

    gcxgc   = read.cdf(inputfile)
    # gcxgc = GCxGC::extract_2D_peaks(raw)
    plt = plot(gcxgc, size = [5000,3300], padding = "padding: 250px 600px 300px 350px;", TrIQ = 1, colorSet = "viridis:turbo")

    bitmap(plt, file = image_TIC)

    tic = GCxGC::TIC1D(gcxgc)
	plt = plot(tic)
	
    bitmap(plt, file = image_TIC1D )


# XIC
# gcxgc = GCxGC::extract_2D_peaks(raw, mz = 43.036646)
# plt = plot(gcxgc, size = [4800,3300], padding = "padding: 250px 500px 250px 250px;")

# bitmap(plt, file = "D:\web\Lu6-1_XIC-43.0366.png")