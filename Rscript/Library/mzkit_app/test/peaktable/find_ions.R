require(mzkit);

imports "mzDeco" from "mz_quantify";

options(memory.load = "max");

let table = readBin("D:\\pos.xcms", what = "peak_set");
let test_ion = table |> ionPeaks(
    mz = 113.1632, rt = 87.1,
    mzdiff = 0.01,
    rt_win = 90
);

# str();

write.csv(as.data.frame(test_ion), file = "D:\\pos_test.csv", row.names = TRUE);