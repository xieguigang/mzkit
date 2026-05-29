require(mzkit);

imports "mzPack" from "mzkit";

let raw = open.mzpack("E:\\biodeep\\biodeep_pipeline\\biodeepMSMS_v5\\test\\lxy-CID30\\lxy-CID30.mzML");

mzPack::convertTo_mzXML(raw, file = "F:/test.mzXML");