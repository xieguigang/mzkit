# string helper module

def getTagValue(str, deli):
    "Get tagged string value as key-value pair"

    if (deli in str): 
        # contains a name
        pos = str.index(deli)
        name = str[0:pos]
        value = str[pos+len(deli):len(str)]
    else :
        # name is null
        name = ""
        value = str

    return name,value