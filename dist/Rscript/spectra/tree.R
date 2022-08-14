require(mzkit);
require(JSON);

imports "spectrumTree" from "mzkit";
imports "data" from "mzkit";

test = "E:\C00006.json"
|> readText()
|> json_decode()
;

str(test$metainfo);

using tree as spectrumTree::open("E:\tree.pack") {

	for(ms2 in test$spectra) {
		x = libraryMatrix(
			data.frame(
				mz = as.numeric(ms2$spectra$ProductMz), 
				into = as.numeric(ms2$spectra$LibraryIntensity)
			)
		)
		;
	
		hits = tree |> query(x);
		id = [hits]::ClusterId;
		
		id 
		|> strsplit(" ")
		|> sapply(r -> r[1])
		|> groupBy(s -> s)
		|> lapply(s -> length(s))
		|> str()
		;
	}

}