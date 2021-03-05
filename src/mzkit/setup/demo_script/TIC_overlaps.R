imports "gtk" from "Rstudio";
imports "visual" from "mzkit.plot";
imports ["assembly", "mzweb"] from "mzkit";

const rawfiles as string = gtk::selectFiles(
    title  = "Select raw data files in mzXML/mzML formats.", 
    filter = "LC-MS raw data(*.mzXML;*.mzML)|*.mzXML;*.mzML"
)
;

let TIC_overlaps as function() {
    const savefile as string = `${dirname(rawfiles[1])}/TIC_overlaps.png`;
    const TIC = new overlaps();
	
	print("loading TIC data of raw data files...");
	
	for(path in rawfiles) {		
		TIC[[basename(path)]] = path 
		:> raw.scans 
		:> load.chromatogram
		;
		
		print(path);
	}
	
	bitmap(file = savefile) {
		TIC :> plot(
			bpc     = FALSE, 
			opacity = 100, 
			colors  = "Paired:c12"
		)
		;
	}
	
	print("TIC plot job done!");
}

if (length(rawfiles) == 0) {
    print("Run TIC overlaps has been canceled...");
} else {
    TIC_overlaps();
}

