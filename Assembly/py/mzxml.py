import base64
import struct
import zlib

from collections import Iterable
from xml.dom import minidom

def read_mzXML(path, msLevel):
    "Parse mz data from mzxml document file."

    print("summary:")
    print("file: ", path)
    print("msLevel filter: ", msLevel)

    # parse xml document and then load all scan data
    doc = minidom.parse(path)
    scans = doc.getElementsByTagName('scan')
    msLevelFilter = (scan for scan in scans if scan.attributes["msLevel"].value == msLevel)

    return msLevelFilter

def decode_scan(scan):
    "Decode mz-int data in a scan."

    peaks = scan.getElementsByTagName('peaks')
    peaks = peaks[1]
    peakBase64 = peaks.text

    # decode base64 text in scan peaks
    # and get zip compressed bytes data
    zip = base64.b64decode(peakBase64)
    # uncompress this zip package
    buffer = zlib.decompress(zip, -zlib.MAX_WBITS)
    buffer = parse_peaks(buffer)

    return buffer

def parse_peaks(peaks_decoded):
    # Based on code by Taejoon Kwon (https://code.google.com/archive/p/massspec-toolbox/)
    tmp_size = len(peaks_decoded) / 4
    unpack_format1 = ">%dL" % tmp_size

    idx = 0
    mz_list = []
    intensity_list = []

    for tmp in struct.unpack(unpack_format1,peaks_decoded):
        tmp_i = struct.pack("I",tmp)
        tmp_f = struct.unpack("f",tmp_i)[0]

        if( idx % 2 == 0 ):
            mz_list.append( float(tmp_f) )
        else:
            intensity_list.append( float(tmp_f) )

        idx += 1

    return mz_list, intensity_list

def parse_scan_rt(scan):
    "Parse rt in seconds value for a given scan object."

    rt = scan.attributes["retentionTime"].value
    rt = rt.replace("PT", "", 3)
    rt = rt.replace("S", "", 3)

    return float(rt)