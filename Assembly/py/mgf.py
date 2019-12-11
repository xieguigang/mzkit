

def readMgf (path) :
    "Read a mgf text file from a given file path."

    file = open(path, "r")
    lines = file.readlines()
    buf = []
    ions = []

    for line in lines:
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

    # loop for each text line until 
    # read a fragment data line
    #
    # The first line of the given data is the 
    # the "BEGIN IONS" tag
    # skip the first line
    #
    for i in range(1, len(linesBlock)):
        if ("=" in linesBlock[i]) :
            
