imports "package_utils" from "devkit";

package_utils::attach(`${@dir}/../../../Rscript/Library/mzkit_app`);

const lipids = lipidmaps_repo();
const engine = metadb::ms1_handler(lipids, ["[M]+", "[M+H]+", "[M+H+H2O]+"], "da:0.1");
const query = ms1_search(engine, [798.5349, 820.5200, 772.5208]);

for (mz in names(query)) {
	print(`query result for m/z: ${mz}:`);
	print(as.data.frame(query[[mz]]));
}

pause();

