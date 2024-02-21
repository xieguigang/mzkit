# assembly

The mass spectrum assembly file read/write library module.
> #### Mass spectrometry data format
>  
>  Mass spectrometry is a scientific technique for measuring the mass-to-charge ratio of ions.
>  It is often coupled to chromatographic techniques such as gas- or liquid chromatography and 
>  has found widespread adoption in the fields of analytical chemistry and biochemistry where 
>  it can be used to identify and characterize small molecules and proteins (proteomics). The 
>  large volume of data produced in a typical mass spectrometry experiment requires that computers 
>  be used for data storage and processing. Over the years, different manufacturers of mass 
>  spectrometers have developed various proprietary data formats for handling such data which 
>  makes it difficult for academic scientists to directly manipulate their data. To address this 
>  limitation, several open, XML-based data formats have recently been developed by the Trans-Proteomic
>  Pipeline at the Institute for Systems Biology to facilitate data manipulation and innovation 
>  in the public sector.

+ [read.msl](assembly/read.msl.1) read MSL data files
+ [read.mgf](assembly/read.mgf.1) Read the spectrum data inside a mgf ASCII data file.
+ [read.msp](assembly/read.msp.1) 
+ [mgf.ion_peaks](assembly/mgf.ion_peaks.1) this function ensure that the output result of the any input ion objects is peakms2 data type.
+ [open.xml_seek](assembly/open.xml_seek.1) 
+ [seek](assembly/seek.1) 
+ [scan_id](assembly/scan_id.1) get all scan id from the ms xml file
+ [load_index](assembly/load_index.1) 
+ [write.mgf](assembly/write.mgf.1) write spectra data in mgf file format.
+ [file.index](assembly/file.index.1) get file index string of the given ms2 peak data.
+ [mzxml.mgf](assembly/mzxml.mgf.1) Convert mzxml file as mgf ions.
+ [raw.scans](assembly/raw.scans.1) get raw scans data from the ``mzXML`` or ``mzMl`` data file
+ [polarity](assembly/polarity.1) get polarity data for each ms2 scans
+ [ms1.scans](assembly/ms1.scans.1) get all ms1 raw scans from the raw files
