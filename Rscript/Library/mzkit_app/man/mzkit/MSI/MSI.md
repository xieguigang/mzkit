# MSI

MS-Imaging data handler
 
 Mass spectrometry imaging (MSI) is a technique used in mass spectrometry
 to visualize the spatial distribution of molecules, as biomarkers, 
 metabolites, peptides or proteins by their molecular masses.

+ [scale](MSI/scale.1) scale the spatial matrix by column
+ [msi_metadata](MSI/msi_metadata.1) get ms-imaging metadata
+ [as.layer](MSI/as.layer.1) cast the pixel collection to a ion imaging layer data
+ [splice](MSI/splice.1) split the raw 2D MSI data into multiple parts with given parts
+ [pixelId](MSI/pixelId.1) get pixels [x,y] tags collection for a specific ion
+ [pixels](MSI/pixels.1) get pixels size from the raw data file
+ [open.imzML](MSI/open.imzML.1) open the reader for the imzML ms-imaging file
+ [write.imzML](MSI/write.imzML.1) Save and write the given ms-imaging mzpack object as imzML file
+ [row.scans](MSI/row.scans.1) each raw data file is a row scan data
+ [MSI_summary](MSI/MSI_summary.1) Fetch MSI summary data
+ [correction](MSI/correction.1) calculate the X scale
+ [basePeakMz](MSI/basePeakMz.1) Get the mass spectrum data of the MSI base peak data
+ [ionStat](MSI/ionStat.1) Extract the ion features inside a MSI raw data slide sample file
+ [ions_jointmatrix](MSI/ions_jointmatrix.1) 
+ [scans2D](MSI/scans2D.1) combine each row scan raw data files as the pixels 2D matrix
+ [scanMatrix](MSI/scanMatrix.1) combine each row scan summary vector as the pixels 2D matrix
+ [peakMatrix](MSI/peakMatrix.1) Extract the ion data matrix
+ [peakSamples](MSI/peakSamples.1) split the raw MSI 2D data into multiple parts with given resolution parts
+ [pixelIons](MSI/pixelIons.1) get number of ions in each pixel scans
+ [getMatrixIons](MSI/getMatrixIons.1) get matrix ions feature m/z vector
+ [pixelMatrix](MSI/pixelMatrix.1) dumping raw data matrix as text table file.
+ [spatial.convolution](MSI/spatial.convolution.1) sum pixels for create pixel spot convolution
+ [pack_matrix](MSI/pack_matrix.1) pack the matrix file as the MSI mzpack
+ [moran_I](MSI/moran_I.1) evaluate the moran index for each ion layer
+ [sample_bootstraping](MSI/sample_bootstraping.1) make expression bootstrapping of current ion layer
+ [cast.spatial_layers](MSI/cast.spatial_layers.1) cast the rawdata matrix as the ms-imaging ion layer
