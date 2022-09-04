# Update History

Mzkit is an open source raw data file toolkit for mass spectrometry data analysis, provides by the BioNovoGene corporation. The features of mzkit inlcudes: raw data file content viewer(XIC/TIC/Mass spectrum plot), build molecule network, formula de-novo search and de-novo annotation.

This open source mass spectrometry data toolkit is developed at the BioDeep R&D laboratory and brought to you by BioNovoGene corporation.

## ``3.5.593.3968`` Sep 1, 2022

> 10.5281/zenodo.7040586

+ ``new`` add tissue morphology map overlaps to MSI viewer
+ ``new`` add tissue morphology map editor feature to create custom tissue region for data analysis 
+ ``new`` add HE stain image analysis module
+ ``new`` add a new general table data viewer for open microsoft excel table files 
+ ``new`` add a general data visualization module for plot data based on the table viewer content
+ ``new`` add new ggplot package for data plot pipeline task
+ ``enhancement`` add data visualization template rendering for MS-imaging plot
+ ``enhancement`` enable view multiple sample MS-imaging data
+ ``enhancement`` add online pubchem metabolite database query function for the ion feature in MSI raw data 
+ ``enhancement`` add data compatibility with the bruker SCiLS lab software
+ ``enhancement`` make improvements of the ms1 peak list data annotation function 
+ ``enhancement`` update internal metabolite database, extends database list from KEGG only to KEGG/lipidmaps/chebi
+ ``enhancement`` make the molecular networking viewer interactive
+ ``enhancement`` add ms1 peak deconvolution function to raw data viewer
+ ``enhancement`` add peak finding analysis feature to the general signal data analysis 
+ ``enhancement`` new application installer experience
+ ``enhancement`` upgrade the internal Rstudio environment from .NET4.8 assembly to .NET6.0 assembly
+ ``enhancement`` add mzwork project file for share the workspace between the device
+ ``enhancement`` make improvements of the MRM/GCMS targetted data viewer
+ ``fixed`` add fix patch script to install internal Rstudio environment
+ ``fixed`` upgrade mzPack format to version 2.0, make improvements of the data compatibility between the single sample data and multiple sample data
+ ``fixed`` make bugs fixed of the style tweaking for raw data plot viewer
+ ``fixed`` handling of the data serialization error when raw data file its file path is a windows UNC path

## ``1.32.7743.6345`` Mar 14, 2021

> 10.5281/zenodo.4603277

+ ``enhancement`` improvements of the raw data charting plot styles
+ ``new`` add peak annotation result for MS matrix viewer
+ ``new`` targetted quantification linear modelling and sample quantify evaluation (required login BioDeep web services)
+ ``enhancement`` improvements of the GCMS/LC-MSMS targetted file explorer 
+ ``enhancement`` improvements of the GCMS feature ROI explorer
+ ``enhancement`` improvements of the document page model
+ ``fixed`` fixed of R# script editor syntax highlight problem
+ ``update`` update to latest mzkit R# package
+ ``new`` add 3D plot of LC-MSMS MRM ion TIC overlaps plot
+ ``enhancement`` add background task progress display on the status bar
+ ``new`` add demo scripts into the file explorer

## ``1.32.7692.29666`` Jan 22, 2021

> 10.5281/zenodo.4456618

+ ``enhancement`` Add plot tweaks function
+ ``new`` MS-imaging
+ ``new`` View MRM/GCMS SIM data files
+ ``enhancement`` Improvements of the R# scripting editor
+ ``enhancement`` Update R# interpreter engine

## ``v1.0.0.2-beta`` Nov 15, 2020

> 10.5281/zenodo.4274582

+ ``fixed`` Bugs fixed of the mzkit_win32 UI
+ ``enhancement`` Improvements of the content project file model
+ ``new`` Mass spectrum similarity search

## ``v1.0.0.1-beta`` Oct 15, 2020

> 10.5281/zenodo.4091067

+ ``new`` View mzML/mzXML raw data files
+ ``new`` Formula de-novol search
+ ``new`` Molecular networking
+ ``new`` Feature search
+ ``new`` R# scripting for mzkit automation
