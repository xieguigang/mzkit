<link rel="manifest" href="/manifest.json">

# <span style="font-size: 3em;">Mzkit</span>

![](docs/manual/splash.PNG)

Mzkit is an open source raw data file toolkit for mass spectrometry data analysis, provides by the ``BioNovoGene`` corporation. The features of mzkit inlcudes: raw data file content viewer(XIC/TIC/Mass spectrum plot), build molecule network, formula de-novo search and annotation.

```
MIT License

Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

<div style="page-break-after:always;"></div>

<div style="width: 100%; text-align: center;">
<div style="font-size: 3em;">--==== TOC ====--</div>
</div>

<hr />

<!-- vscode-markdown-toc -->
* 1. [Raw Data Viewer Instruction](#RawDataViewerInstruction)
	* 1.1. [Imports raw data file](#Importsrawdatafile)
	* 1.2. [Search Feature](#SearchFeature)
	* 1.3. [XIC plot](#XICplot)

		* 1.3.1. [XIC overlay](#XICoverlay)

	* 1.4. [TIC plot](#TICplot)
	* 1.5. [Raw Scatter](#RawScatter)
	* 1.6. [View Mass spectra](#ViewMassspectra)
	* 1.7. [Save Plot and Export matrix](#SavePlotandExportmatrix)

		* 1.7.1. [Export XIC](#ExportXIC)

* 2. [Data Annotations](#DataAnnotations)
	* 2.1. [Formula search](#Formulasearch)

		* 2.1.1. [Export Formula Search Result](#ExportFormulaSearchResult)

	* 2.2. [Molecular Networking](#MolecularNetworking)

		* 2.2.1. [step1 select ions](#step1selections)
		* 2.2.2. [step2 build network](#step2buildnetwork)
		* 2.2.3. [step3 view network data](#step3viewnetworkdata)
		* 2.2.4. [step4 network visualization](#step4networkvisualization)
		* 2.2.5. [Export Network Data](#ExportNetworkData)
		* 2.2.6. [Save Network Visual](#SaveNetworkVisual)

* 3. [Appendix](#Appendix)
	* 3.1. [Switch Between Toolkit](#SwitchBetweenToolkit)
	* 3.2. [Install Mzkit](#InstallMzkit)
	* 3.3. [Uninstall Mzkit](#UninstallMzkit)

<!-- vscode-markdown-toc-config
	numbering=true
	autoSave=true
	/vscode-markdown-toc-config -->
<!-- /vscode-markdown-toc -->

<hr />

**Credits**

> This open source mass spectrometry data toolkit is developed at the [BioDeep](http://www.biodeep.cn/) R&D laboratory and brought to you by ``BioNovoGene`` corporation.

![](docs/BioNovoGene.png)

<div style="page-break-after:always;"></div>
<div style="font-size: 2em;">Product Screenshots</div>

![](docs/manual/main.png)
![](docs/manual/BPC_overlay.PNG)
![](docs/manual/MS.PNG)

<div style="page-break-after:always;"></div>

##  1. <a name='RawDataViewerInstruction'></a>Raw Data Viewer Instruction

###  1.1. <a name='Importsrawdatafile'></a>Imports raw data file

For view the file content of the mzXML or mzML datafile in mzkit, you must imports the raw data file into the mzkit at first. Here is how: select the ``Main`` tabpage of mzkit you will see the ``Open`` command for the raw data imports operation. Then you are going to click this ``Open`` command button, choose the raw data file for imports and wait for it finished. 

![](docs/manual/open.png)

Then you should see the files that you've imports into mzkit on the ``File Explorer`` dock panel if there is no error occurs during the raw data file imports progress. Now you can click on the raw data file tree to expend it and click one of the feature in your raw file to view the content data.

![](docs/manual/file-explorer.png)

<div style="page-break-after:always;"></div>

###  1.2. <a name='SearchFeature'></a>Search Feature

the search bar on the top of the file tree is the ``m/z`` search input: you can input a specific ``m/z`` value or ``formula`` expression in the search bar for search the matched features in your raw data file. This operation is usually apply for the ``XIC`` data search.

When you have click on the search button, then all of the ``M/z`` feature in your raw data file that match the ``ppm`` condition will be listed in the ``Featrue List`` dock panel:

![](docs/manual/search-list.png)
> An example of search m/z 834.6 with tolerance error 30ppm. the result in the ``Feature List`` is usually used for create a XCI plot.

###  1.3. <a name='XICplot'></a>XIC plot

Expends the file content tree in the ``File Explorer``, and then mouse right click on one MS2 feature in your file, select ``XIC`` for create a XIC plot for a specific ion feature:

![](docs/manual/ion-XIC.png)
> The XIC plot is a kind of time-signal chromatography plot of a specific m/z ion.

<div style="page-break-after:always;"></div>

####  1.3.1. <a name='XICoverlay'></a>XIC overlay

you can click on the checkbox besides the Ms2 feature for select different ion feature for create the XIC overlay plot:

![](docs/manual/XIC-overlay.png)

###  1.4. <a name='TICplot'></a>TIC plot

as the same as create a XIC plot, you also can create TIC plot for a single file or multiple file by select multiple file by check on checkbox:

![](docs/manual/TIC.png)
> The TIC plot is similar to the XIC plot, data is generated from all ions.

<div style="page-break-after:always;"></div>

###  1.5. <a name='RawScatter'></a>Raw Scatter

just click on the node of the raw file, then you will open the raw scatter plot of your specific raw data file, example as:

![](docs/manual/raw-scatter.PNG)

###  1.6. <a name='ViewMassspectra'></a>View Mass spectra

For view the mass spectra data in your file, just click on one of the scan feature in your raw data file:

![](docs/manual/ms2-plot.png)

###  1.7. <a name='SavePlotandExportmatrix'></a>Save Plot and Export matrix

The mzkit application provides the function for save the plot image and the plot data in your raw data file. for example, select the ``Data Viewer`` tab page in mzkit, you will see two viewer action buttons in the menu:

1. [``Snapshot``] for export the XIC/TIC/MS2 data plot image to a specific file.
2. [``Save Matrix``] for export the Mass spectra or Chromatography data to a specific Excel table file.

![](docs/manual/export-plot-matrix.png)

####  1.7.1. <a name='ExportXIC'></a>Export XIC

Export the XIC data to a specific file is also keeps simple, just mouse right click on the file content tree and then choose ``Export XIC Ions``.

![](docs/manual/export-XIC.png)

<div style="page-break-after:always;"></div>

##  2. <a name='DataAnnotations'></a>Data Annotations

###  2.1. <a name='Formulasearch'></a>Formula search

You can search for the formula of one feature in your raw data file with mzkit for do some de-novo annotation with just simply mouse right click on one of the feature and then select ``Search Formula``, and then wait for a while to let mzkit for search all of the candidate formula by enumerates all of the possible element combinations:

![](docs/manual/formula-search-progress.png)

Once the mzkit have been done of formula search, then all of the matched formula will be shown on the result page. there are some information about the each formula search result is listed in the result table, includes: formula result, its corresponding exact mass value, mass error of the searched m/z and the m/z calculated from the exact mass, ion charge value from your raw file, precursor type information, etc.

![](docs/manual/de-novo-formulas.png)

<div style="page-break-after:always;"></div>

You also can search of the formula by input any ``m/z`` value in the input box of the search result page. Click on the formula then you could submit the candidate formula into the biodeep database for search of the metabolite information.

![](docs/manual/export-formula-list.png)

####  2.1.1. <a name='ExportFormulaSearchResult'></a>Export Formula Search Result

Export the formula search result just like other data that we've introduced before, just click on the ``Formula Result`` tab page and then click on the Export button. this command will let you save the formula search result into a specific Excel table file. 

<div style="page-break-after:always;"></div>

###  2.2. <a name='MolecularNetworking'></a>Molecular Networking

The molecular networking in mass spectrum data is a kind of spectrum similarity network. You can build a molecule network use mzkit in a very easy way:

####  2.2.1. <a name='step1selections'></a>step1 select ions

For create a spectrum similarity network, it required some ms2 ions data provides to mzkit for run spectrum matrix alignment and evaluate the simialrity scores between these spectrum matrix data. select the ions data just like plot XIC operation it does in mzkit: check on the ms2 feature in the ``file explorer`` dock panel.

####  2.2.2. <a name='step2buildnetwork'></a>step2 build network

Once we have the mass spectrum data selected, then we could run the matrix similarity between these matrix for build a network. now we mouse right click of the file tree in the ``file explorer``, and then choose the ``Molecular Networking`` menu item:

![](docs/manual/molecular_networking.png)

####  2.2.3. <a name='step3viewnetworkdata'></a>step3 view network data

once the networking progress have been done, then we could view the network result data in the ``Molecular Networking`` tool page. there are three tables in the tool page for show your network data: ``Network``, ``Compounds`` and ``Network Statistics``.

![](docs/manual/network_viewer_tabs.PNG)

+ 1. the ``Network`` tab page contains the edges data in your network, which is the spectrum cluster simialrity result. all of the spectrum alignment its simialrity value is less than the ``Spectrum Similarity`` threshold value that will be removes from the network.

![](docs/manual/network_edges_viewer.PNG)

there are columns in the edge table: ``CompoundA`` and ``CompoundB`` is the spectrum reference id in this edge connected. and the ``simialrity``, ``forward`` and ``reverse`` column is the simialrity score value of the two spectrum matrix and the last ``View`` column contains the button that let you view the spectrum matrix alignment result visual plot.

+ 2. the ``Compounds`` tab page contains the spectrum cluster (**network nodes**) information in your network.

![](docs/manual/network_nodes_viewer.PNG)

####  2.2.4. <a name='step4networkvisualization'></a>step4 network visualization

The mzkit program provides a small build-in network visualization engine that could let you visual the resulted molecular network with just a simple mouse click. As you can see in the previous screenshot, there is a ``Render Network`` command button on the top menu when the ``Network`` tab is actived. No we just click on it, and wait for the network layout calculation progress complete, and then we will see a new tag page which is named ``Molecular Networking Viewer`` will be shown in the document area of the mzkit program.

####  2.2.5. <a name='ExportNetworkData'></a>Export Network Data

There is not too much style tweaking in the mzkit build-in network visualizer, so that you may be want to export the network data into table and then visualize it in other network visualization software like the famous ``Cytoscape``.

Just click on the ``Export`` command button beside the ``Render Network`` button in the ``Network`` menu tab, then a dialog of save network data will be trigger and opened. Select a location in the dialog and then you can save the network table data into a given location for visualization in other software.

![](docs/manual/export-network.PNG)

<div style="page-break-after:always;"></div>

####  2.2.6. <a name='SaveNetworkVisual'></a>Save Network Visual

![](docs/manual/network_viewer.PNG)

For export the network image, just mouse right click on the viewer panel, then you could see a popout menu ``Save Image`` that could use for viewer image saved.

![](docs/manual/save_network.PNG)

You also can save the network image via ``Save`` command in the ``Main`` menu tab page when the molecular networking viewer is current active document page in mzkit program.

![](docs/manual/save_network2.PNG)

Now you can use the saved network image for your publications:

![](docs/manual/network_visualze.png)

<div style="page-break-after:always;"></div>

##  3. <a name='Appendix'></a>Appendix

###  3.1. <a name='SwitchBetweenToolkit'></a>Switch Between Toolkit

You can switch between toolkit pages via the start menu of mzkit:

![](docs/manual/switch_toolkit2.PNG)
> click ``[File]`` -> ``[Mzkit Data Toolkits]``, and then click on one toolkit item then you can switch to the required toolkit page.

or just select a page from the app switcher toolstrip menu:

![](docs/manual/switch_toolkit1.PNG)

<div style="page-break-after:always;"></div>

###  3.2. <a name='InstallMzkit'></a>Install Mzkit

![](docs/manual/setup.PNG)

When you have extract the zip package of the mzkit compression package, then you could found a ``setup.exe`` install application in the top of folder. Click on this setup application, then your will going to install mzkit program into your computer system.

![](docs/manual/installer_warning.PNG)

When you have launch the mzkit installer, then you will see a security warning dialog from your windows system, just click on the ``Install`` for start the install progress, and then just needs to wait for the installer finish the progress of copy the mzkit application files.

![](docs/manual/install_mzkit.png)

<div style="page-break-after:always;"></div>

###  3.3. <a name='UninstallMzkit'></a>Uninstall Mzkit

if you want to removes mzkit from your computer system, then you could follow this instruction for uninstall:

1. Open the control panel of your windows system, and then click of the link ``Uninstall a program``

![](docs/manual/control_panel.PNG)

2. Then you will see a list of program that installed on your windows, located the mzkit application

![](docs/manual/app_list.PNG)

3. Then right click on the mzkit, select ``uninstall``, then you can removes mzkit from your windows system

![](docs/manual/uninstall_right_click.png)

Just click ``OK`` on the dialog:

![](docs/manual/uninstaller.PNG)

<style type="text/css">
	#content-wrapper {
		background-color: white !important;    
	}
</style>

<script type="text/javascript">
	document.ready = function() {
		document.getElementById("sidebar").style.display = "none";
	}
</script>