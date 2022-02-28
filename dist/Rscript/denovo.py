import mzkit

from mzkit import mzweb
from Rstudio import gtk

targets  = gtk::selectFiles(title = "Select a csv table contains target product fragments", filter = "Excel(*.csv)|*.csv", multiple = False)
files    = gtk::selectFiles(title = "Select raw data files for run ms2 filter", filter = "Thermofisher Raw(*.raw)|*.raw", throwCancel = False) 
savefile = gtk::selectFiles(title = "Select a table file for save result", filter = "Excel(*.xls)|*.xls", forSave = True)

# tolerance value for match ms2 data
mzdiff  = mzkit::tolerance("da", 0.3)
hits    = None
targets = read.csv(targets, row.names = None)

print("view of the ms2 product ion list:")
print(targets, max.print = 6)

if length(files) == 0:
    raise "no raw data file was selected for run data processing!"

if !all(["name", "mz"] in colnames(targets)):
    raise "missing one of the data fields in your target product fragments table: 'name' or 'mz'!"
else
    print("input check success")

def search_product(filepath):
    
    mzpack    = open.mzpack(filepath)
    products  = ms2_peaks(mzpack)
    i         = lapply(products, ms2 -> [ms2]::GetIntensity(mz, mzdiff)) > 0
    products  = products[i]
    mz        = sapply(products, ms2 -> [ms2]::mz)
    rt        = sapply(products, ms2 -> [ms2]::rt)
    rt_min    = round(rt / 60, 2)
    intensity = sapply(products, ms2 -> [ms2]::intensity)
    totalIons = sapply(products, ms2 -> [ms2]::Ms2Intensity)
    scan      = sapply(products, ms2 -> [ms2]::scan)
    nsize     = sapply(products, ms2 -> [ms2]::fragments)
    
    topIons   = lapply(product, ms2 -> topIons([ms2]::mzInto))
    basePeak  = sapply(topIons, ms2 -> basePeak_toString(ms2[1]))
    top2
    top3
    top4
    top5

    return mz, rt, rt_min, intensity, scan, basePeak, top2, top3, top4, top5

def basePeak_toString(mz2):
    

def topIons(ms2):
    
    

for filepath in files:
    
    print(`processing data [${filepath}]...`)
    
    peaks = data.frame(search_product(filepath))
    peaks[, "samplefile"] = basename(filepath)
    hits = rbind(hits, peaks)
       
print(" ~~job done!")
print(`save result file at location: '${savefile}'!`)

write.csv(hits, file = savefile, row.names = True)
