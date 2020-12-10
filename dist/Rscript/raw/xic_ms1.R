imports ["assembly", "math", "data"] from "mzkit";

const file = "Z:\lab\RSD test\neg\Controls\mzML\N02-B01-YQT1202-U01-1.mzML";
const mzi = 198.03;

let ms1 = ms1.scans(file);
let xic = XIC(ms1, mzi, ppm = 100);

write.csv(as.data.frame(xic), file = `${dirname(file)}/${basename(file)}.${mzi}_xic.csv`); 