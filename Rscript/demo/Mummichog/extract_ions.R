require(mzkit);

imports ["mzweb","MSI"] from "mzkit";

const raw = open.mzpack("C:\Users\Administrator\Desktop\demo.mzPack");
const ions = MSI::ionStat(raw);

write.csv(as.data.frame(ions), file = `${@dir}/test.csv`);