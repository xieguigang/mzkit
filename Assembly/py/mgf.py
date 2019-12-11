import strings

def readMgf (path) :
    "Read a mgf text file from a given file path."

    file = open(path, "r")
    lines = []   
    ions = []

    for line in file.readlines():
        if (line == "END ION") :
            ions.append(parseIon(lines))
            lines = []
        else :
            lines.append(line)

    if (len(lines) > 0) :
        ions.append(parseIon(lines))

    file.close()

    return ions

def parseIon(linesBlock) :
    "Parse a mgf ion data from a given text lines of data."

    # BEGIN IONS
    # TITLE=250a1a3e-75c3-4918-bc70-78852f153e92 activation:"HCD", collisionEnergy:"20", ...
    # RTINSECONDS=850.77348
    # PEPMASS=296.235144894825 158596.1
    # CHARGE=1
    # ACCESSION=5a3ab4ec-84eb-4f97-bf17-e4912ebd1c19
    # DB=mzCloud (Autoprocessed)
    # LOCUS=Autoprocessed-6501
    # 52.96542 628.3123
    # 53.0136 1403.576
    # 53.01506 1006.565
    # 53.07506 2556.898
    # 53.107 2704.549
    # 53.10882 4232.059
    # ...
    # END IONS

    meta = {}
    mz   = []
    into = []
    pms2 = 0
    l = len(linesBlock)

    # loop for each text line until 
    # read a fragment data line
    #
    # The first line of the given data is the 
    # the "BEGIN IONS" tag
    # skip the first line
    #
    for i in range(1, l):
        if ("=" in linesBlock[i]) :
            name,value = strings.getTagValue(linesBlock[i])
            meta[name] = value
        else :
            # this is the end of the meta data region
            pms2 = i
            break

    # parse ms2 data
    for i in range(pms2, l - 1):
        mzinto = linesBlock[i].split(' ')
        mz.append(float(mzinto[0]))
        into.append(float(mzinto[1]))

    # post processing
    title_meta = parseTitleMeta(meta["TITLE"])
    meta["TITLE"] = title_meta["title"]
    ms2 = {"mz": mz, "into": into}
    ion = {"meta" : meta, "ms2": ms2, "annotation": title_meta["meta"]}

    return ion

def parseTitleMeta(title):
    "Parse meta data that contains in title value string"

    title_str = title.split(" ", 1)
    meta_str = title[len(title_str) + 1: len(title)]
    meta_str = meta_str.split('", ')
    meta = {}

    for attr in meta_str:
        name,value = strings.getTagValue(attr, ':"')
        meta[name] = value

    return {"title": title_str, "meta" : meta}