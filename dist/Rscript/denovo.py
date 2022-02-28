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

def search_product(filepath, mz):
    
    mzpack    = open.mzpack(filepath)
    products  = ms2_peaks(mzpack)
    
    i         = sapply(products, ms2 -> [ms2]::GetIntensity(mz, mzdiff)) > 0
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
    top2      = sapply(topIons, ms2 -> mz2_toString(ms2, 2))
    top3      = sapply(topIons, ms2 -> mz2_toString(ms2, 3))
    top4      = sapply(topIons, ms2 -> mz2_toString(ms2, 4))
    top5      = sapply(topIons, ms2 -> mz2_toString(ms2, 5))

    return (mz, rt, rt_min, intensity, totalIons, scan, nsize, basePeak, top2, top3, top4, top5)

def mz2_toString(ms2, i):
    into = sapply(ms2, x -> [x]::intensity)
    into = round(into / max(into) * 100, 2)
    mz2  = ms2[i]
    
    return `${toString([mz2]::mz, format = "F4")}:${into[i]}`

def basePeak_toString(mz2):
    return `${toString([mz2]::mz, format = "F4")}:${toString([mz2]::intensity, format = "G3")}`    

def topIons(ms2):
    into = sapply(ms2, x -> [x]::intensity)
    i    = order(into, decreasing = True)
    
    return ms2[i]
    
# loop through all raw data files
for filepath in files:
    
    print(`processing data [${filepath}]...`)
       
    # loop through all target ms2 product ions
    for(mz2 in as.list(targets, byrow = True)):
        
        str(mz2)        
           
        # name, mz    
        peaks                  = search_product(filepath, mz2[["mz"]])
        peaks                  = data.frame(peaks)
        peaks[, "samplefile"]  = basename(filepath)
        peaks[, "target_name"] = mz2[["name"]]
        peaks[, "target_mz"]   = mz2[["mz"]]
        
        hits = rbind(hits, peaks)
       
print(" ~~job done!")
print(`save result file at location: '${savefile}'!`)

print(hits, max.print = 13)

write.csv(hits, file = savefile, row.names = False)
