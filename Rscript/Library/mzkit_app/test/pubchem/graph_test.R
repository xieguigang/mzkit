require(mzkit);

imports ["pubchem_kit" "massbank"] from "mzkit";

str(query.knowlegde_graph(1, cache = `${@dir}/test_data`));