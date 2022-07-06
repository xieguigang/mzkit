# R# ./webTest.R --attach "D:\mzkit\Rscript\Library\mzkit_app"

require(mzkit);

options(pubchem.http_cache = `${dirname(@script)}/.pubchem`);

print(pubchem_meta("Cyanidin"));


print(pubchem_meta("aspirin"));
print(pubchem_meta("56-65-5"));

