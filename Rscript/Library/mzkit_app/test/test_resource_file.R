require(mzkit);

imports "metadna" from "mzDIA";

# test for the internal messagepack resource data file

# test lipidmaps dataset
str(lipidmaps_repo());

# test kegg dataset
# use for metaDNA analysis of the rawdata files
str(kegg.network(repo = system.file("data/reaction_class.msgpack", package = "mzkit")));