import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

files = ["C:\MSI\GCxGC-plot\YCH-G-G-3_TIC2D.cdf","C:\MSI\GCxGC-plot\BYH-G-G-3_TIC2D.cdf","C:\MSI\GCxGC-plot\BYH-M-H-3_TIC2D.cdf","C:\MSI\GCxGC-plot\CDH-G-R-3-FC_TIC2D.cdf"]
gcxgc = list()

for inputfile in files:
    
    gcxgc[[basename(inputfile)]] = read.cdf(inputfile)
    
    
names = ["Carbon disulfide","Benzene","Acetonitrile","Benzaldehyde","Octadecanal","Pyrazine, ethyl-","Toluene","p-Xylene","p-Xylene","Pyrrole","Pyrazine, 2-ethyl-5-methyl-","Methylamine, N,N-dimethyl-","Acetaldehyde","Ethanol","Pyridine"]
rt1= [219.994,504.976,639.967,1504.91,2534.85,1209.93,699.964,889.951,879.952,1484.91,1294.93,154.998,204.995,489.977,954.947]
rt2 = [1.636,1.812,1.356,1.463,3.268,1.582,1.883,1.919,1.946,1.268,1.684,1.331,1.374,1.461,1.494]

metabolites = data.frame(name = names, rt1=rt1, rt2= rt2)
heatmap = "C:\MSI\GCxGC-plot\heatmap.png"

print(metabolites)

plt = gcxgc_heatmap(gcxgc, metabolites, space = [50,10],rt_width = [100,0.8],size = [2700,4000])

bitmap(plt, file = heatmap)