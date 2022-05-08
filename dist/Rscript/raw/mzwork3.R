require(mzkit);

imports "mzPack" from "mzkit";

pack = mzPack::mzwork("F:\multiExport\lipids.mzWork");
fileNames = mzPack::ls(pack);

print(fileNames);

for(name in fileNames) {
	print(pack |> readFileCache(name));
}