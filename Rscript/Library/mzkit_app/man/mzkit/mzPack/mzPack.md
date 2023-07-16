# mzPack

raw data accessor for the mzpack data object

+ [getSampleTags](mzPack/getSampleTags.1) get all of the sample file data tags from target mzpack file
+ [split_samples](mzPack/split_samples.1) try to split target mzpack file into multiple parts based on the sample tags
+ [open.mzwork](mzPack/open.mzwork.1) open mzwork file and then populate all of the mzpack raw data file
+ [mzwork](mzPack/mzwork.1) open a mzwork package file
+ [readFileCache](mzPack/readFileCache.1) read mzpack data from the mzwork package by a 
+ [mzpack](mzPack/mzpack.1) open a mzpack data object reader, not read all data into memory in one time.
+ [ls](mzPack/ls.1) show all ms1 scan id in a mzpack data object or 
+ [metadata](mzPack/metadata.1) get metadata list from a specific ms1 scan
+ [scaninfo](mzPack/scaninfo.1) get ms scan information metadata list
+ [convertTo_mzXML](mzPack/convertTo_mzXML.1) method for write mzpack data object as a mzML file
+ [packData](mzPack/packData.1) pack mzkit ms2 peaks data as a mzpack data object
+ [packStream](mzPack/packStream.1) write mzPack in v2 format
+ [removeSciexNoise](mzPack/removeSciexNoise.1) Removes the sciex AB5600 noise data from the MS2 raw data
+ [pack_ms1](mzPack/pack_ms1.1) pack a given ms2 spectrum collection as a single ms1 scan product
