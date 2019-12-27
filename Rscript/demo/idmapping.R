imports "mzkit.massbank" from "mzkit.dll";

let chebi as string = "D:\Database\chebi\DataSet.Xml";
let hmdb as string = "D:\Database\hmdb_metabolites.xml";

chebi 
:> chebi.secondary2main.mapping 
:> save.mapping(file = "./chebi.json");

hmdb 
:> hmdb.secondary2main.mapping
:> save.mapping(file = "./hmdb.json");