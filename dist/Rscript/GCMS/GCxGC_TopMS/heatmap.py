import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

files = list.files("C:\MSI\GCxGC-plot\2d", pattern = "*.cdf")
gcxgc = list()

print(files)

for inputfile in files:
    
    gcxgc[[basename(inputfile)]] = read.cdf(inputfile)
    
    
names = ["Carbon disulfide","Benzene","Acetonitrile","Benzaldehyde","Octadecanal","Pyrazine, ethyl-","Toluene","p-Xylene","p-Xylene","Pyrrole","Pyrazine, 2-ethyl-5-methyl-","Methylamine, N,N-dimethyl-","Acetaldehyde","Ethanol","Pyridine"]
rt1= [219.994,504.976,639.967,1504.91,2534.85,1209.93,699.964,889.951,879.952,1484.91,1294.93,154.998,204.995,489.977,954.947]
rt2 = [1.636,1.812,1.356,1.463,3.268,1.582,1.883,1.919,1.946,1.268,1.684,1.331,1.374,1.461,1.494]

metabolites = data.frame(name = names, rt1=rt1, rt2= rt2)
heatmap = "C:\MSI\GCxGC-plot\heatmap.png"

print(metabolites)

plt = gcxgc_heatmap(gcxgc, metabolites, space = [50,10],rt_width = [100,0.8],size = [3700,4000], padding = "padding: 200px 1800px 250px 250px;")

bitmap(plt, file = heatmap)