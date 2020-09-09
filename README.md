<link rel="manifest" href="/manifest.json">

# <span style="font-size: 3em;">Mzkit</span>

![](docs/manual/splash.PNG)

<div style="font-size: 3em;">--==== TOC ====--</div>

<!-- vscode-markdown-toc -->
* 1. [Raw Data Viewer Instruction](#RawDataViewerInstruction)
	* 1.1. [Imports raw data file](#Importsrawdatafile)
	* 1.2. [Search Feature](#SearchFeature)
	* 1.3. [XIC plot](#XICplot)

		* 1.3.1. [XIC overlay](#XICoverlay)

	* 1.4. [TIC plot](#TICplot)
	* 1.5. [View Mass spectra](#ViewMassspectra)
	* 1.6. [Save Plot and Export matrix](#SavePlotandExportmatrix)

		* 1.6.1. [Export XIC](#ExportXIC)

* 2. [Data Annotations](#DataAnnotations)
	* 2.1. [Formula search](#Formulasearch)

		* 2.1.1. [Export Formula Search Result](#ExportFormulaSearchResult)

	* 2.2. [Molecular Networking](#MolecularNetworking)

		* 2.2.1. [Save Network Visual](#SaveNetworkVisual)

* 3. [Appendix](#Appendix)
	* 3.1. [Switch Between Toolkit](#SwitchBetweenToolkit)
	* 3.2. [Install Mzkit](#InstallMzkit)
	* 3.3. [Uninstall Mzkit](#UninstallMzkit)
* 4. [Credits](#Credits)

<!-- vscode-markdown-toc-config
	numbering=true
	autoSave=true
	/vscode-markdown-toc-config -->
<!-- /vscode-markdown-toc -->

<div style="page-break-after:always;"></div>

Mzkit is an open source raw data file toolkit for mass spectrometry data analysis, provides by the ``BioNovoGene`` corporation. The features of mzkit inlcudes: raw data file content viewer(XIC/TIC/Mass spectrum plot), build molecule network, formula de-novo search and annotation.

<div style="font-size: 2em;">Product Screenshots</div>

![](docs/manual/main.png)

<div style="page-break-after:always;"></div>

![](docs/manual/BPC_overlay.PNG)
![](docs/manual/MS.PNG)

<div style="page-break-after:always;"></div>

##  1. <a name='RawDataViewerInstruction'></a>Raw Data Viewer Instruction

###  1.1. <a name='Importsrawdatafile'></a>Imports raw data file

For view the file content of the mzXML or mzML datafile in mzkit, you must imports the raw data file into the mzkit at first. Here is how: select the ``Main`` tabpage of mzkit you will see the ``Open`` command for the raw data imports operation. Then you are going to click this ``Open`` command button, choose the raw data file for imports and wait for it finished. 

![](docs/manual/open.png)

Then you should see the files that you've imports into mzkit on the ``File Explorer`` dock panel if there is no error occurs during the raw data file imports progress. Now you can click on the raw data file tree to expend it and click one of the feature in your raw file to view the content data.

![](docs/manual/file-explorer.png)

###  1.2. <a name='SearchFeature'></a>Search Feature

the search bar on the top of the file tree is the ``m/z`` search input: you can input a specific ``m/z`` value or ``formula`` expression in the search bar for search the matched features in your raw data file. This operation is usually apply for the ``XIC`` data search.

When you have click on the search button, then all of the ``M/z`` feature in your raw data file that match the ``ppm`` condition will be listed in the ``Featrue List`` dock panel:

![](docs/manual/search-list.png)
> An example of search m/z 834.6 with tolerance error 30ppm. the result in the ``Feature List`` is usually used for create a XCI plot.

###  1.3. <a name='XICplot'></a>XIC plot

Expends the file content tree in the ``File Explorer``, and then mouse right click on one MS2 feature in your file, select ``XIC`` for create a XIC plot for a specific ion feature:

![](docs/manual/ion-XIC.png)
> The XIC plot is a kind of time-signal chromatography plot of a specific m/z ion.

####  1.3.1. <a name='XICoverlay'></a>XIC overlay

you can click on the checkbox besides the Ms2 feature for select different ion feature for create the XIC overlay plot:

![](docs/manual/XIC-overlay.png)

###  1.4. <a name='TICplot'></a>TIC plot

as the same as create a XIC plot, you also can create TIC plot for a single file or multiple file by select multiple file by check on checkbox:

![](docs/manual/TIC.png)
> The TIC plot is similar to the XIC plot, data is generated from all ions.

###  1.5. <a name='ViewMassspectra'></a>View Mass spectra

For view the mass spectra data in your file, just click on one of the scan feature in your raw data file:

![](docs/manual/ms2-plot.png)

###  1.6. <a name='SavePlotandExportmatrix'></a>Save Plot and Export matrix

The mzkit application provides the function for save the plot image and the plot data in your raw data file. for example, select the ``Data Viewer`` tab page in mzkit, you will see two viewer action buttons in the menu:

1. [``Snapshot``] for export the XIC/TIC/MS2 data plot image to a specific file.
2. [``Save Matrix``] for export the Mass spectra or Chromatography data to a specific Excel table file.

![](docs/manual/export-plot-matrix.png)

####  1.6.1. <a name='ExportXIC'></a>Export XIC

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

####  2.2.1. <a name='SaveNetworkVisual'></a>Save Network Visual

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

1. Open the control panel of your windows system, and then click of the like ``Uninstall a program``

![](docs/manual/control_panel.PNG)

2. Then you will see a list of program that installed on your windows, located the mzkit application

![](docs/manual/app_list.PNG)

3. Then right click on the mzkit, select uninstall, then you can removes mzkit from your windows system

![](docs/manual/uninstall_right_click.png)

Just click ``OK`` on the dialog:

![](docs/manual/uninstaller.PNG)

<div style="page-break-after:always;"></div>

##  4. <a name='Credits'></a>Credits

This open source mass spectrometry data toolkit is developed at the BioDeep R&D laboratory and provided to you by BioNovoGene corporation.

![](docs/BioNovoGene.png)

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