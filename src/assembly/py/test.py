import mzxml

print("Test of parse demo mzxml file")

for scan in mzxml.read_mzXML("D:/biodeep/biodeepDB/protocols/biodeepMSMS1/biodeepMSMS/test/lxy-CID30.mzXML", 1):

    rt = mzxml.parse_scan_rt(scan)
    print("rt: " + str(rt))