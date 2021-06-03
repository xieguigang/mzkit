# R# ./webTest.R --attach "D:\mzkit\Rscript\Library\mzkit_app"

options(pubchem.http_cache = `${dirname(@script)}/.pubchem`);

print(pubchem_meta(1259312-93-0));