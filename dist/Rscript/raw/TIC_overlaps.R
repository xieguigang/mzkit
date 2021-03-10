imports "visual" from "mzkit.plot";
imports ["assembly", "mzweb", "chromatogram"] from "mzkit";

const rawfiles as string   = ?"--data"       || getwd();
const sampleinfo as string = ?"--sampleinfo" || `${getwd()}/sampleinfo.txt`;
const savedir as string    = ifelse(file.exists(rawfiles), dirname(rawfiles), rawfiles);

let load_overlaps as function(files as string) {
	print("Loading raw data files");
	print(basename(files));
	
	files :> lapply(function(path) {
		cat("*");
	
		path 
		:> raw.scans 
		:> load.chromatogram
		;
		
	}, names = path -> basename(path))
	:> overlaps
	;
}

let TIC_overlaps as function(overlaps, key = "") {
    const savefileTIC as string = ifelse(key == "", `${savedir}/TIC_overlaps.png`, `${savedir}/${key}_TIC_overlaps.png`);
	const savefileBPC as string = ifelse(key == "", `${savedir}/BPC_overlaps.png`, `${savedir}/${key}_BPC_overlaps.png`);
	const colorSet = "green,red,blue,yellow,black,purple,orange";
	
	bitmap(file = savefileTIC) {	
		overlaps :> plot(
			bpc         = FALSE, 
			opacity     = 30, 
			colors      = colorSet,
			show.labels = FALSE,
			xlab        = "Scan Time(minutes)"
		)
		;
	}
	bitmap(file = savefileBPC) {	
		overlaps :> plot(
			bpc         = TRUE, 
			opacity     = 30, 
			colors      = colorSet,
			show.labels = FALSE,
			xlab        = "Scan Time(minutes)"
		)
		;
	}
	
	# parallel style
	bitmap(file = `${savedir}/${basename(savefileTIC)}_parallel.png`) {	
		overlaps :> plot(
			bpc         = FALSE, 
			opacity     = 100, 
			colors      = colorSet,
			show.labels = FALSE,
			parallel    = TRUE,
			fill        = FALSE,
			padding     = "padding:100px 500px 150px 250px;",
			size        = "3600,2000",
			line.stroke = "stroke: black; stroke-width: 6px; stroke-dash: solid;",
			axis.cex    = "font-style: normal; font-size: 36; font-family: Bookman Old Style;",
			legend.cex  = "font-style: normal; font-size: 28; font-family: Bookman Old Style;",
			xlab        = "Scan Time(minutes)",
			tick.cex    = "font-style: normal; font-size: 24; font-family: Bookman Old Style;"
		)
		;
	}
	bitmap(file = `${savedir}/${basename(savefileBPC)}_parallel.png`) {	
		overlaps :> plot(
			bpc         = TRUE, 
			opacity     = 100, 
			colors      = colorSet,
			show.labels = FALSE,
			parallel    = TRUE,
			fill        = FALSE,
			padding     = "padding:100px 500px 150px 250px;",
			size        = "3600,2000",
			line.stroke = "stroke: black; stroke-width: 6px; stroke-dash: solid;",
			axis.cex    = "font-style: normal; font-size: 36; font-family: Bookman Old Style;",
			legend.cex  = "font-style: normal; font-size: 28; font-family: Bookman Old Style;",
			xlab        = "Scan Time(minutes)",
			tick.cex    = "font-style: normal; font-size: 24; font-family: Bookman Old Style;"
		)
		;
	}
	
	print("TIC plot job done!");
}

let loadAuto as function() {
	if (file.exists(rawfiles)) {
		chromatogram::read.pack(cdf = rawfiles);
	} else {
		rawfiles 
		:> list.files(pattern = "*.mz*ML") 
		:> load_overlaps 
		;
	}
}

const overlaps = loadAuto() :> scale_time("minute");

print(overlaps);

if (!file.exists(sampleinfo)) {
	print("No sampleinfo table file was found, TIC/BPC overlaps for all raw data files will be run!");
	
	TIC_overlaps(overlaps);
} else {
	const meta   = read.csv(sampleinfo, tsv = TRUE);	
	const groups = unique(meta[, "sample_info"]);
	const single = list();

	print("we get sample groups:");
	print(groups);

	for(name in groups) {
		const sample_id    = (meta[, "ID"])[meta[, "sample_info"] == name];
		const sample_names = (meta[, "sample_name"])[meta[, "sample_info"] == name]; 
	
		if (name != "QC") {		
			print("sample group contains sample_id list:");
			print(name);
			print(sample_id);
			
			single[[name]] = sample_id[1];
			
			overlaps 
			:> subset(sample_id) 
			:> labels(sample_names)
			:> TIC_overlaps(key = name)
			;
		}
	}
	
	print("TIC/BPC overlaps for overviews:");
	str(single);
	
	single 
	:> lapply(ref -> overlaps[[ref]])
	:> overlaps
	:> TIC_overlaps()
	;
}
