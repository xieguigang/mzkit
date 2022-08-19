require(mzkit);

imports "mzPack" from "mzkit";

"C:\Users\Public\2022-06-14-空间代谢组重新分析_to_Li\空间代谢组\group_A.mzPack"
|> split_samples()
;

# |> getSampleTags()
# |> print()
# ;