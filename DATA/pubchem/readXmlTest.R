require(mzkit);
require(JSON);

dumpJSON = function(demo, saveName) {

demo = read.pugView(demo);
demo = as.list(metadata.pugView(demo));

str(demo);

print(demo$synonym);

demo 
|> json_encode()
|> writeLines(con = saveName);

}

dumpJSON(
demo = `${@dir}/.pubchem/21/213729ace8005a12960854286d217415.html`, 
saveName = `${@dir}/ATP.json`
);

dumpJSON(
demo = `${@dir}/.pubchem/21/213729ace8005a12960854286d217415.html`, 
saveName = `${@dir}/aspirin.json`
);