require(mzkit);

imports ["spectrumTree", "annotation"] from "mzkit";

pack = spectrumTree::readpack("F:\etc\MoNA\lib.pos.pack");
const [peaktable, rawdata] = get_testSample(pack);
test = populateIonData(rawdata);
lib = spectrumTree::open("F:\etc\MoNA\lib.pos.pack");

result = spectrumTree::query(lib, test);