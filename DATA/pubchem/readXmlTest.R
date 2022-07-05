require(mzkit);

demo = `${@dir}/.pubchem/21/213729ace8005a12960854286d217415.html`;
demo = read.pugView(demo);
demo = as.list(metadata.pugView(demo));

str(demo);

print(demo$synonym);