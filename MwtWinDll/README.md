An algorithm for filtering molecular formulas is derived from seven heuristic rules: 

+ ``(1)`` restrictions for the number of elements, 
+ ``(2)`` LEWIS and SENIOR chemical rules, 
+ ``(3)`` isotopic patterns, 
+ ``(4)`` hydrogen/carbon ratios, 
+ ``(5)`` element ratio of nitrogen, oxygen, phosphor, and sulphur versus carbon, 
+ ``(6)`` element ratio probabilities and 
+ ``(7)`` presence of trimethylsilylated compounds.

> Seven Golden Rules for heuristic filtering of molecular formulas obtained by accurate mass spectrometry (``doi:10.1186/1471-2105-8-105``)


### Rule #1 – restrictions for element numbers

For developing this rule, the absolute element limits were calculated by simply dividing the mass range through the element mass (e.g. for ``carbon = 12 Da`` at 1000 Da follows ``1000/12 = 83``) maximum limit for a hypothetical molecule that consists exclusively of carbon.

Table 1: Restrictions for number of elements during formula generation for small molecules based on examination of the DNP and Wiley mass spectral databases. For each element, the higher count was taken for denominating the element restriction rule #1

|Mass Range [Da]|Library|C max|H max|N max|O max|P max|S max|F max|Cl max|Br max|Si max|
|---------------|-------|-----|-----|-----|-----|-----|-----|-----|------|------|------|
|     < 500     |  DNP  |  29 |  72 | 10  | 18  |  4  |  7  |  15 |  8   |   5  |      |
|               | Wiley |  39 |  72 | 20  | 20  |  9  |  10 |  16 |  10  |   4  |  8   |
|     < 1000    |  DNP  |  66 | 126 | 25  | 27  |  6  |  8  |  16 |  11  |   8  |      |
|               | Wiley |  78 | 126 | 20  | 27  |  9  |  14 |  34 |  12  |   8  |  14  |
|     < 2000    |  DNP  | 115 | 236 | 32  | 63  |  6  |  8  |  16 |  11  |   8  |      |
|               | Wiley | 156 | 180 | 20  | 40  |  9  |  14 |  48 |  12  |  10  |  15  |
|     < 3000    |  DNP  | 162 | 208 | 48  | 78  |  6  |  9  |  16 |  11  |   8  |      |