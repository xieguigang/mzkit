imports "gtk" from "Rstudio";

const rawfiles as string = gtk::selectFiles(
	title  = "Select raw data files in mzXML/mzML formats.", 
	filter = "LC-MS raw data(*.mzXML;*.mzML)|*.mzXML;*.mzML"
)
;

let TIC_overlaps as function() {
	const savefile as string = `${dirname(rawfiles[1])}/TIC_overlaps.png`;
	
}

if (length(rawfiles) == 0) {
	print("Run TIC overlaps has been canceled...");
} else {
	TIC_overlaps();
}

