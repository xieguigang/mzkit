import mzkit

from mzkit import mzPack
from mzkit import mzweb

pack = open.mzpack("D:\biodeep\biodeepDB\protocols\biodeepMSMS1\biodeepMSMS\test\lxy-CID30.mzXML")
convertTo_mzXML(pack, file = "F:\x.mzXML")