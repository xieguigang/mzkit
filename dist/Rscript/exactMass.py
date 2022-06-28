import mzkit

from mzkit import math
from mzkit import formula

setwd("D:\Database\BioDeepDB\MetaDB")

meta = read.csv("./hmdb.csv", check.names = False, row.names = 1)
meta[, "exact_mass (metlin)"] = None
meta[, "exactMass"] = formula::eval(meta[, "formula"])

write.csv(meta, file = "./hmdb_updated.csv")
