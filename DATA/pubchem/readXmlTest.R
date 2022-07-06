require(mzkit);
require(JSON);

dumpJSON = function(demo, saveName) {

demo = read.pugView(demo);
demo = as.list(metadata.pugView(demo));

str(demo);

str(demo$synonym);

demo 
|> json_encode()
|> writeLines(con = saveName);

}




dumpJSON(
demo = `${@dir}/.pubchem/65/65ad7f20ec1d4fcfe8afa6a1965dfa52.html`, 
saveName = `${@dir}/cyanidin.json`
);


dumpJSON(
demo = `${@dir}/.pubchem/99/9912fc6f0d40dadc570f6a96f8af966a.html`, 
saveName = `${@dir}/aspirin.json`
);

dumpJSON(
demo = `${@dir}/.pubchem/21/213729ace8005a12960854286d217415.html`, 
saveName = `${@dir}/ATP.json`
);

