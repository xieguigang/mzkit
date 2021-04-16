imports "assembly" from "mzkit";

const xmlfile as string = "D:\lxy-CID30.mzXML";

using raw as open.xml_seek(xmlfile) {
	print("all of the scan id in the given raw data file:");
	print(scan_id(raw));
	
	print("test of seek a MS1 scan data:");
	print(raw :> seek(1));
	print(raw :> seek(2746));
	
	print("test of seek a MS2 scan data:");
	print(raw :> seek(3430));
}

# test mzML file

using raw as open.xml_seek("D:\QC5.mzML") {
	print("all of the scan id in the given raw data file:");
	print(scan_id(raw));
}