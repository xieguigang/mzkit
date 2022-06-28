import mzkit

from mzkit import GCxGC, mzweb
from mzplot import visual

files = list.files("C:\MSI\GCxGC-plot\2d", pattern = "*.cdf")
gcxgc = list()

print(files)

for inputfile in files:
    
    gcxgc[[gsub(basename(inputfile), "_TIC2D", "")]] = read.cdf(inputfile)
    
    
names = ["Carbon disulfide","Benzene","Acetonitrile","Benzaldehyde","Octadecanal","Pyrazine, ethyl-","Toluene","p-Xylene","p-Xylene","Pyrrole","Pyrazine, 2-ethyl-5-methyl-","Methylamine, N,N-dimethyl-","Acetaldehyde","Ethanol","Pyridine"]
rt1= [219.994,504.976,639.967,1504.91,2534.85,1209.93,699.964,889.951,879.952,1484.91,1294.93,154.998,204.995,489.977,954.947]
rt2 = [1.636,1.812,1.356,1.463,3.268,1.582,1.883,1.919,1.946,1.268,1.684,1.331,1.374,1.461,1.494]

metabolites = data.frame(name = names, rt1=rt1, rt2= rt2)

print(metabolites)

for palette in ["viridis:magma", "viridis","viridis:inferno","viridis:plasma", "viridis:cividis","viridis:mako","viridis:rocket","viridis:turbo"]:
    
    heatmap = `C:\MSI\GCxGC-plot\heatmap_${gsub(palette, ":", "_")}.png`
    plt = gcxgc_heatmap(gcxgc, metabolites, space = [50,10],rt_width = [400,0.65],size = [3600,3600], padding = "padding: 100px 1100px 600px 100px;",colorSet = palette)

    bitmap(plt, file = heatmap)