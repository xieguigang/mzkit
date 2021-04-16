imports "assembly" from "mzkit";

const xmlfile as string = "D:\lxy-CID30.mzXML";

using raw as open.xml_seek(xmlfile) {
	print("all of the scan id in the given raw data file:");
	print(scan_id(raw));
	
	print("test of seek a scan data:");
	print(raw :> seek(1));
}