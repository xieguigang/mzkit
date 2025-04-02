require(mzkit);

let precursor = 223.0845;
let ms2 = [
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.07752298556522008,"mz":62.9818},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.031009194226088034,"mz":80.9486},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.07752298556522008,"mz":84.0809},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.07752298556522008,"mz":105.0338},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.03876149278261004,"mz":116.071},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.06977068700869807,"mz":116.9664},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.03876149278261004,"mz":122.0709},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.0516768221777757,"mz":123.1063},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.03876149278261004,"mz":125.0901},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.0516768221777757,"mz":130.0859},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.0516768221777757,"mz":134.0331},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.03876149278261004,"mz":135.0372},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.3333333333333333,"mz":136.062},
    {"Annotation":"MS3,precursor_m/z=57.071","intensity":0.12919205544443926,"mz":139.9821},
    {"Annotation":"","intensity":0.05345505572167535,"mz":149.0238},
    {"Annotation":"","intensity":0.04284837828385683,"mz":223.0882},
    {"Annotation":"","intensity":0.5,"mz":225.0434},
    {"Annotation":"","intensity":0.04011065370441337,"mz":226.0437},
    {"Annotation":"","intensity":0.04557289323022088,"mz":227.0224}
];

ms2 = data::libraryMatrix(data.frame(
    mz = ms2@mz,
    intensity = ms2@intensity
), title = "demo spectrum data");

str(ms2);