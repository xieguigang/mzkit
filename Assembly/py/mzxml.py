from collections import Iterable
from xml.dom import minidom

def read_mzXML(path, msLevel):
    "Parse mz data from mzxml document file."

    # parse xml document and then load all scan data
    doc = minidom.parse('items.xml');
    scans = doc.getElementsByTagName('scan');
    msLevelFilter = (scan for scan in scans if scan.attributes["msLevel"].value == msLevel);

    return msLevelFilter;