# MsImaging

### Visual MS imaging data(*.imzML)
 
 Mass spec imaging (MSI) is a technique measuring chemical composition and 
 linking it to spatial coordinates on a surface.  The chemical composition
 is determined by mass spectrometry, which measures the mass-to-charge ratios
 (m/z's) of any ions that can be generated from the surface.  Most commonly,
 the surface is a tissue section on a microscope slide; however, any flat 
 surface could be analyzed given it has suitable dimensions and is properly 
 prepared.

+ [split.layer](MsImaging/split.layer.1) split the ms-imaging layer into multiple parts
+ [TrIQ](MsImaging/TrIQ.1) Contrast optimization of mass spectrometry imaging(MSI) data
+ [intensityLimits](MsImaging/intensityLimits.1) trim the intensity data value in a pixels of a ion MS-Imaging layer
+ [write.mzImage](MsImaging/write.mzImage.1) write mzImage data file
+ [read.mzImage](MsImaging/read.mzImage.1) open the existed mzImage cache file
+ [FilterMz](MsImaging/FilterMz.1) Extract a spectrum matrix object from MSI data by a given set of m/z values
+ [MS1](MsImaging/MS1.1) get the ms1 spectrum data in a specific pixel position
+ [viewer](MsImaging/viewer.1) load imzML data into the ms-imaging render
+ [pixel](MsImaging/pixel.1) get the spatial spot pixel data
+ [ionLayers](MsImaging/ionLayers.1) load the raw pixels data from imzML file
+ [tag_layers](MsImaging/tag_layers.1) set cluster tags to the pixel tag property data
+ [sum_layers](MsImaging/sum_layers.1) merge multiple layers via intensity sum
+ [rgb](MsImaging/rgb.1) rendering ions MSI in (R,G,B) color channels
+ [MSIlayer](MsImaging/MSIlayer.1) get MSI pixels layer via given ``m/z`` value.
+ [intensity](MsImaging/intensity.1) Get intensity data vector from a given MS-imaging layer
+ [knnFill](MsImaging/knnFill.1) do pixel interpolation for run MS-imaging
+ [MSI_coverage](MsImaging/MSI_coverage.1) 
+ [assert](MsImaging/assert.1) test of a given MSI layer is target?
+ [layer](MsImaging/layer.1) render a ms-imaging layer by a specific ``m/z`` scan.
+ [MSI_summary.scaleMax](MsImaging/MSI_summary.scaleMax.1) 
+ [defaultFilter](MsImaging/defaultFilter.1) get the default ms-imaging filter pipeline
+ [parseFilters](MsImaging/parseFilters.1) 
+ [intensityFilter](MsImaging/intensityFilter.1) 
+ [render](MsImaging/render.1) MS-imaging of the MSI summary data result.
+ [as.pixels](MsImaging/as.pixels.1) extract the pixel [x,y] information for all of
+ [MeasureMSIions](MsImaging/MeasureMSIions.1) 
