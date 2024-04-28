#Region "Microsoft.VisualBasic::16967c7b8e151456c67ab62091622afa, E:/mzkit/src/metadb/Chemoinformatics//Formula/Models/Element.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 320
    '    Code Lines: 273
    ' Comment Lines: 34
    '   Blank Lines: 13
    '     File Size: 28.98 KB


    '     Class Element
    ' 
    '         Properties: charge, id, isotopes, isotopic, meta
    '                     name, symbol, z
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: Isotope, MemoryLoadElements, MemoryPopulateElements, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns

Namespace Formula

    ''' <summary>
    ''' Data Load Statements
    ''' 
    ''' Uncertainties from CRC Handbook of Chemistry and Physics
    ''' For Radioactive elements, the most stable isotope is NOT used;
    ''' instead, an average Mol. Weight is used, just like with other elements.
    ''' Data obtained from the Perma-Chart Science Series periodic table, 1993.
    ''' Uncertainties from CRC Handoobk of Chemistry and Physics, except for
    ''' Radioactive elements, where uncertainty was estimated to be .n5 where
    ''' intSpecificElementProperty represents the number digits after the decimal point but before the last
    ''' number of the molecular weight.
    ''' 
    ''' For example, for No, MW = 259.1009 (?.0005)
    ''' </summary>
    Public Class Element

        ''' <summary>
        ''' the iupac ID
        ''' </summary>
        ''' <returns></returns>
        Public Property id As Integer

        ''' <summary>
        ''' the element atom symbol, or the element name in MS-DIAL
        ''' </summary>
        ''' <returns></returns>
        Public Property symbol As String
        Public Property z As Integer
        Public Property meta As Dictionary(Of String, Object)
        ''' <summary>
        ''' common name of current element atom symbol
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        Public Property charge As Integer
        ''' <summary>
        ''' isotopic Element Weights
        ''' </summary>
        ''' <returns></returns>
        Public Property isotopic As Double
        Public Property isotopes As Isotope()

        Sub New()
        End Sub

        Sub New(name As String, mass As Double)
            Me.symbol = name
            Me.name = name
            Me.isotopic = mass
        End Sub

        Sub New(id As Integer)
            Me.id = id
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{id}] {symbol} - {name} ({isotopic})"
        End Function

        Public Shared Function MemoryLoadElements() As Dictionary(Of String, Element)
            Return MemoryPopulateElements.ToDictionary(Function(e) e.symbol)
        End Function

        Public Const H As Double = 1.0078246

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function Isotope(mass As Double, prob As Double, num As Integer) As Isotope
            Return New Isotope With {.Mass = mass, .Prob = prob, .NumNeutrons = num}
        End Function

        ''' <summary>
        ''' A in-memory database for the element mass
        ''' </summary>
        ''' <returns></returns>
        Friend Shared Iterator Function MemoryPopulateElements() As IEnumerable(Of Element)
            Yield New Element(1) With {.symbol = "H", .name = "Hydrogen", .charge = 1, .isotopic = H, .isotopes = {Isotope(1.0078250321, 0.999885, 1), Isotope(2.014101778, 0.000115, 2)}}
            Yield New Element(1) With {.symbol = "D", .name = "Deuterium", .charge = 1, .isotopic = 2.014101778, .isotopes = {Isotope(2.014101778, 0.000115, 2)}}
            Yield New Element(2) With {.symbol = "He", .name = "Helium", .charge = 0, .isotopic = 4.0026029, .isotopes = {Isotope(3.0160293097, 0.00000137, 3), Isotope(4.0026032497, 0.99999863, 4)}}
            Yield New Element(3) With {.symbol = "Li", .name = "Lithium", .charge = 1, .isotopic = 7.016005, .isotopes = {Isotope(6.0151223, 0.0759, 6), Isotope(7.016004, 0.9241, 7)}}
            Yield New Element(4) With {.symbol = "Be", .name = "Beryllium", .charge = 2, .isotopic = 9.012183, .isotopes = {Isotope(9.0121821, 1.0, 9)}}
            Yield New Element(5) With {.symbol = "B", .name = "Boron", .charge = 3, .isotopic = 11.009305, .isotopes = {Isotope(10.012937, 0.199, 10), Isotope(11.0093055, 0.801, 11)}}
            Yield New Element(6) With {.symbol = "C", .name = "Carbon", .charge = 4, .isotopic = 12, .isotopes = {Isotope(12.0, 0.9893, 12), Isotope(13.0033548378, 0.0107, 13)}}
            Yield New Element(7) With {.symbol = "N", .name = "Nitrogen", .charge = -3, .isotopic = 14.003074, .isotopes = {Isotope(14.0030740052, 0.99632, 14), Isotope(15.0001088984, 0.00368, 15)}}
            Yield New Element(8) With {.symbol = "O", .name = "Oxygen", .charge = -2, .isotopic = 15.994915, .isotopes = {Isotope(15.9949146221, 0.99757, 16), Isotope(16.9991315, 0.00038, 17), Isotope(17.9991604, 0.00205, 18)}}
            Yield New Element(9) With {.symbol = "F", .name = "Fluorine", .charge = -1, .isotopic = 18.9984032, .isotopes = {Isotope(18.9984032, 1.0, 19)}}
            Yield New Element(10) With {.symbol = "Ne", .name = "Neon", .charge = 0, .isotopic = 19.992439, .isotopes = {Isotope(19.9924401759, 0.9048, 20), Isotope(20.99384674, 0.0027, 21), Isotope(21.99138551, 0.0925, 22)}}
            Yield New Element(11) With {.symbol = "Na", .name = "Sodium", .charge = 1, .isotopic = 22.98977, .isotopes = {Isotope(22.98976967, 1.0, 23)}}
            Yield New Element(12) With {.symbol = "Mg", .name = "Magnesium", .charge = 2, .isotopic = 23.98505, .isotopes = {Isotope(23.9850419, 0.7899, 24), Isotope(24.98583702, 0.1, 25), Isotope(25.98259304, 0.1101, 26)}}
            Yield New Element(13) With {.symbol = "Al", .name = "Aluminium", .charge = 3, .isotopic = 26.981541, .isotopes = {Isotope(26.98153844, 1.0, 27)}}
            Yield New Element(14) With {.symbol = "Si", .name = "Silicon", .charge = 4, .isotopic = 27.976928, .isotopes = {Isotope(27.9769265327, 0.922297, 28), Isotope(28.97649472, 0.046832, 29), Isotope(29.97377022, 0.030871, 30)}}
            Yield New Element(15) With {.symbol = "P", .name = "Phosphorus", .charge = -3, .isotopic = 30.973763, .isotopes = {Isotope(30.97376151, 1.0, 31)}}
            Yield New Element(16) With {.symbol = "S", .name = "Sulfur", .charge = -2, .isotopic = 31.972072, .isotopes = {Isotope(31.97207069, 0.9493, 32), Isotope(32.9714585, 0.0076, 33), Isotope(33.96786683, 0.0429, 34), Isotope(35.96708088, 0.0002, 36)}}
            Yield New Element(17) With {.symbol = "Cl", .name = "Chlorine", .charge = -1, .isotopic = 34.968853, .isotopes = {Isotope(34.96885271, 0.7578, 35), Isotope(36.9659026, 0.2422, 37)}}
            Yield New Element(18) With {.symbol = "Ar", .name = "Argon", .charge = 0, .isotopic = 39.962383, .isotopes = {Isotope(35.96754628, 0.003365, 36), Isotope(37.9627322, 0.000632, 38), Isotope(39.962383123, 0.996003, 40)}}
            Yield New Element(19) With {.symbol = "K", .name = "Potassium", .charge = 1, .isotopic = 38.963708, .isotopes = {Isotope(38.9637069, 0.932581, 39), Isotope(39.96399867, 0.000117, 40), Isotope(40.96182597, 0.067302, 41)}}
            Yield New Element(20) With {.symbol = "Ca", .name = "Calcium", .charge = 2, .isotopic = 39.962591, .isotopes = {Isotope(39.9625912, 0.96941, 40), Isotope(41.9586183, 0.00647, 42), Isotope(42.9587668, 0.00135, 43), Isotope(43.9554811, 0.02086, 44), Isotope(45.9536928, 0.00004, 46), Isotope(47.952534, 0.00187, 48)}}
            Yield New Element(21) With {.symbol = "Sc", .name = "Scandium", .charge = 3, .isotopic = 44.955914, .isotopes = {Isotope(44.9559102, 1.0, 45)}}
            Yield New Element(22) With {.symbol = "Ti", .name = "Titanium", .charge = 4, .isotopic = 47.947947, .isotopes = {Isotope(45.9526295, 0.0825, 46), Isotope(46.9517638, 0.0744, 47), Isotope(47.9479471, 0.7372, 48), Isotope(48.9478708, 0.0541, 49), Isotope(49.9447921, 0.0518, 50)}}
            Yield New Element(23) With {.symbol = "V", .name = "Vanadium", .charge = 5, .isotopic = 50.943963, .isotopes = {Isotope(49.9471628, 0.0025, 50), Isotope(50.9439637, 0.9975, 51)}}
            Yield New Element(24) With {.symbol = "Cr", .name = "Chromium", .charge = 3, .isotopic = 51.94051, .isotopes = {Isotope(49.9460496, 0.04345, 50), Isotope(51.9405119, 0.83789, 52), Isotope(52.9406538, 0.09501, 53), Isotope(53.9388849, 0.02365, 54)}}
            Yield New Element(25) With {.symbol = "Mn", .name = "Manganese", .charge = 2, .isotopic = 54.938046, .isotopes = {Isotope(54.9380496, 1.0, 55)}}
            Yield New Element(26) With {.symbol = "Fe", .name = "Iron", .charge = 3, .isotopic = 55.934939, .isotopes = {Isotope(53.9396148, 0.05845, 54), Isotope(55.9349421, 0.91754, 56), Isotope(56.9353987, 0.02119, 57), Isotope(57.9332805, 0.00282, 58)}}
            Yield New Element(27) With {.symbol = "Co", .name = "Cobalt", .charge = 2, .isotopic = 58.933198, .isotopes = {Isotope(58.9332002, 1.0, 59)}}
            Yield New Element(28) With {.symbol = "Ni", .name = "Nickel", .charge = 2, .isotopic = 57.935347, .isotopes = {Isotope(57.9353479, 0.680769, 58), Isotope(59.9307906, 0.262231, 60), Isotope(60.9310604, 0.011399, 61), Isotope(61.9283488, 0.036345, 62), Isotope(63.9279696, 0.009256, 64)}}
            Yield New Element(29) With {.symbol = "Cu", .name = "Copper", .charge = 2, .isotopic = 62.929599, .isotopes = {Isotope(62.9296011, 0.6917, 63), Isotope(64.9277937, 0.3083, 65)}}
            Yield New Element(30) With {.symbol = "Zn", .name = "Zinc", .charge = 2, .isotopic = 63.929145, .isotopes = {Isotope(63.9291466, 0.4863, 64), Isotope(65.9260368, 0.279, 66), Isotope(66.9271309, 0.041, 67), Isotope(67.9248476, 0.1875, 68), Isotope(69.925325, 0.0062, 70)}}
            Yield New Element(31) With {.symbol = "Ga", .name = "Gallium", .charge = 3, .isotopic = 68.925581, .isotopes = {Isotope(68.925581, 0.60108, 69), Isotope(70.924705, 0.39892, 71)}}
            Yield New Element(32) With {.symbol = "Ge", .name = "Germanium", .charge = 4, .isotopic = 71.92208, .isotopes = {Isotope(69.9242504, 0.2084, 70),
                   Isotope(71.9220762, 0.2754, 72),
                   Isotope(72.9234594, 0.0773, 73),
                   Isotope(73.9211782, 0.3628, 74),
                   Isotope(75.9214027, 0.0761, 76)}}
            Yield New Element(33) With {.symbol = "As", .name = "Arsenic", .charge = -3, .isotopic = 74.921596, .isotopes = {Isotope(74.9215964, 1.0, 75)}}
            Yield New Element(34) With {.symbol = "Se", .name = "Selenium", .charge = -2, .isotopic = 79.916521, .isotopes = {Isotope(73.9224766, 0.0089, 74),
                   Isotope(75.9192141, 0.0937, 76),
                  Isotope(76.9199146, 0.0763, 77),
                   Isotope(77.9173095, 0.2377, 78),
                   Isotope(79.9165218, 0.4961, 80),
                   Isotope(81.9167, 0.0873, 82)}}
            Yield New Element(35) With {.symbol = "Br", .name = "Bromine", .charge = -1, .isotopic = 78.918336, .isotopes = {Isotope(78.9183376, 0.5069, 79),
                   Isotope(80.916291, 0.4931, 81)}}
            Yield New Element(36) With {.symbol = "Kr", .name = "Krypton", .charge = 0, .isotopic = 83.911506, .isotopes = {Isotope(77.920386, 0.0035, 78),
                   Isotope(79.916378, 0.0228, 80),
                   Isotope(81.9134846, 0.1158, 82),
                   Isotope(82.914136, 0.1149, 83),
                  Isotope(83.911507, 0.57, 84),
                   Isotope(85.9106103, 0.173, 86)}}
            Yield New Element(37) With {.symbol = "Rb", .name = "Rubidium", .charge = 1, .isotopic = 84.9118, .isotopes = {Isotope(84.9117893, 0.7217, 85),
                   Isotope(86.9091835, 0.2783, 87)}}
            Yield New Element(38) With {.symbol = "Sr", .name = "Strontium", .charge = 2, .isotopic = 87.905625, .isotopes = {Isotope(83.913425, 0.0056, 84),
                   Isotope(85.9092624, 0.0986, 86),
                   Isotope(86.9088793, 0.07, 87),
                   Isotope(87.9056143, 0.8258, 88)}}
            Yield New Element(39) With {.symbol = "Y", .name = "Yttrium", .charge = 3, .isotopic = 88.905856, .isotopes = {Isotope(88.9058479, 1.0, 89)}}
            Yield New Element(40) With {.symbol = "Zr", .name = "Zirconium", .charge = 4, .isotopic = 89.904708, .isotopes = {Isotope(89.9047037, 0.5145, 90),
                   Isotope(90.905645, 0.1122, 91),
                   Isotope(91.9050401, 0.1715, 92),
                   Isotope(93.9063158, 0.1738, 94),
                   Isotope(95.908276, 0.028, 96)}}
            Yield New Element(41) With {.symbol = "Nb", .name = "Niobium", .charge = 5, .isotopic = 92.906378, .isotopes = {Isotope(92.9063775, 1.0, 93)}}
            Yield New Element(42) With {.symbol = "Mo", .name = "Molybdenum", .charge = 6, .isotopic = 97.905405, .isotopes = {Isotope(91.90681, 0.1484, 92),
                   Isotope(93.9050876, 0.0925, 94),
                   Isotope(94.9058415, 0.1592, 95),
                   Isotope(95.9046789, 0.1668, 96),
                   Isotope(96.906021, 0.0955, 97),
                   Isotope(97.9054078, 0.2413, 98),
                   Isotope(99.907477, 0.0963, 100)}}
            Yield New Element(43) With {.symbol = "Tc", .name = "Technetium", .charge = 7, .isotopic = 98, .isotopes = {Isotope(97.907216, 1.0, 98)}}
            Yield New Element(44) With {.symbol = "Ru", .name = "Ruthenium", .charge = 4, .isotopic = 101.90434, .isotopes = {Isotope(95.907598, 0.0554, 96),
                   Isotope(97.905287, 0.0187, 98),
                   Isotope(98.9059393, 0.1276, 99),
                   Isotope(99.9042197, 0.126, 100),
                   Isotope(100.9055822, 0.1706, 101),
                   Isotope(101.9043495, 0.3155, 102),
                   Isotope(103.90543, 0.1862, 104)}}
            Yield New Element(45) With {.symbol = "Rh", .name = "Rhodium", .charge = 3, .isotopic = 102.905503, .isotopes = {Isotope(102.905504, 1.0, 103)}}
            Yield New Element(46) With {.symbol = "Pd", .name = "Palladium", .charge = 2, .isotopic = 105.903475, .isotopes = {Isotope(101.905608, 0.0102, 102),
                   Isotope(103.904035, 0.1114, 104),
                   Isotope(104.905084, 0.2233, 105),
                   Isotope(105.903483, 0.2733, 106),
                   Isotope(107.903894, 0.2646, 108),
                   Isotope(109.905152, 0.1172, 110)}}
            Yield New Element(47) With {.symbol = "Ag", .name = "Silver", .charge = 1, .isotopic = 106.905095, .isotopes = {Isotope(106.905093, 0.51839, 107),
                   Isotope(108.904756, 0.48161, 109)}}
            Yield New Element(48) With {.symbol = "Cd", .name = "Cadmium", .charge = 2, .isotopic = 113.903361, .isotopes = {Isotope(105.906458, 0.0125, 106),
                   Isotope(107.904183, 0.0089, 108),
                   Isotope(109.903006, 0.1249, 110),
                   Isotope(110.904182, 0.128, 111),
                   Isotope(111.9027572, 0.2413, 112),
                   Isotope(112.9044009, 0.1222, 113),
                   Isotope(113.9033581, 0.2873, 114),
                   Isotope(115.904755, 0.0749, 116)}}
            Yield New Element(49) With {.symbol = "In", .name = "Indium", .charge = 3, .isotopic = 114.903875, .isotopes = {Isotope(112.904061, 0.0429, 113),
                   Isotope(114.903878, 0.9571, 115)}}
            Yield New Element(50) With {.symbol = "Sn", .name = "Tin", .charge = 4, .isotopic = 119.902199, .isotopes = {Isotope(111.904821, 0.0097, 112),
                   Isotope(113.902782, 0.0066, 114),
                  Isotope(114.903346, 0.0034, 115),
                   Isotope(115.901744, 0.1454, 116),
                   Isotope(116.902954, 0.0768, 117),
                   Isotope(117.901606, 0.2422, 118),
                   Isotope(118.903309, 0.0859, 119),
                   Isotope(119.9021966, 0.3258, 120),
                   Isotope(121.9034401, 0.0463, 122),
                   Isotope(123.9052746, 0.0579, 124)}}
            Yield New Element(51) With {.symbol = "Sb", .name = "Antimony", .charge = -3, .isotopic = 120.903824, .isotopes = {Isotope(120.903818, 0.5721, 121),
                   Isotope(122.9042157, 0.4279, 123)}}
            Yield New Element(52) With {.symbol = "Te", .name = "Tellurium", .charge = -2, .isotopic = 129.906229, .isotopes = {Isotope(119.90402, 0.0009, 120),
                   Isotope(121.9030471, 0.0255, 122),
                   Isotope(122.904273, 0.0089, 123),
                   Isotope(123.9028195, 0.0474, 124),
                   Isotope(124.9044247, 0.0707, 125),
                   Isotope(125.9033055, 0.1884, 126),
                   Isotope(127.9044614, 0.3174, 128),
                   Isotope(129.9062228, 0.3408, 130)}}
            Yield New Element(53) With {.symbol = "I", .name = "Iodine", .charge = -1, .isotopic = 126.904477, .isotopes = {Isotope(126.904468, 1.0, 127)}}
            Yield New Element(54) With {.symbol = "Xe", .name = "Xenon", .charge = 0, .isotopic = 131.904148, .isotopes = {Isotope(123.9058958, 0.0009, 124),
                  Isotope(125.904269, 0.0009, 126),
                   Isotope(127.9035304, 0.0192, 128),
                   Isotope(128.9047795, 0.2644, 129),
                   Isotope(129.9035079, 0.0408, 130),
                   Isotope(130.9050819, 0.2118, 131),
                   Isotope(131.9041545, 0.2689, 132),
                   Isotope(133.9053945, 0.1044, 134),
                   Isotope(135.90722, 0.0887, 136)}}
            Yield New Element(55) With {.symbol = "Cs", .name = "Caesium", .charge = 1, .isotopic = 132.905433, .isotopes = {Isotope(132.905447, 1.0, 133)}}
            Yield New Element(56) With {.symbol = "Ba", .name = "Barium", .charge = 2, .isotopic = 137.905236, .isotopes = {Isotope(129.90631, 0.00106, 130),
                   Isotope(131.905056, 0.00101, 132),
                   Isotope(133.904503, 0.02417, 134),
                   Isotope(134.905683, 0.06592, 135),
                   Isotope(135.90457, 0.07854, 136),
                   Isotope(136.905821, 0.11232, 137),
                   Isotope(137.905241, 0.71698, 138)}}
            Yield New Element(57) With {.symbol = "La", .name = "Lanthanum", .charge = 3, .isotopic = 138.906355, .isotopes = {Isotope(137.907107, 0.0009, 138),
                   Isotope(138.906348, 0.9991, 139)}}
            Yield New Element(58) With {.symbol = "Ce", .name = "Cerium", .charge = 3, .isotopic = 139.905442, .isotopes = {Isotope(135.90714, 0.00185, 136),
                   Isotope(137.905986, 0.00251, 138),
                   Isotope(139.905434, 0.8845, 140),
                   Isotope(141.90924, 0.11114, 142)}}
            Yield New Element(59) With {.symbol = "Pr", .name = "Praseodymium", .charge = 4, .isotopic = 140.907657, .isotopes = {Isotope(140.907648, 1.0, 141)}}
            Yield New Element(60) With {.symbol = "Nd", .name = "Neodymium", .charge = 3, .isotopic = 141.907731, .isotopes = {Isotope(141.907719, 0.272, 142),
                   Isotope(142.90981, 0.122, 143),
                   Isotope(143.910083, 0.238, 144),
                   Isotope(144.912569, 0.083, 145),
                   Isotope(145.913112, 0.172, 146),
                   Isotope(147.916889, 0.057, 148),
                   Isotope(149.920887, 0.056, 150)}}
            Yield New Element(61) With {.symbol = "Pm", .name = "Promethium", .charge = 3, .isotopic = 145, .isotopes = {Isotope(144.912744, 1.0, 145)}}
            Yield New Element(62) With {.symbol = "Sm", .name = "Samarium", .charge = 3, .isotopic = 151.919741, .isotopes = {Isotope(143.911995, 0.0307, 144),
                   Isotope(146.914893, 0.1499, 147),
                   Isotope(147.914818, 0.1124, 148),
                   Isotope(148.91718, 0.1382, 149),
                   Isotope(149.917271, 0.0738, 150),
                   Isotope(151.919728, 0.2675, 152),
                   Isotope(153.922205, 0.2275, 154)}}
            Yield New Element(63) With {.symbol = "Eu", .name = "Europium", .charge = 3, .isotopic = 152.921243, .isotopes = {Isotope(150.919846, 0.4781, 151),
                   Isotope(152.921226, 0.5219, 153)}}
            Yield New Element(64) With {.symbol = "Gd", .name = "Gadolinium", .charge = 3, .isotopic = 157.924111, .isotopes = {Isotope(151.919788, 0.002, 152),
                   Isotope(153.920862, 0.0218, 154),
                   Isotope(154.922619, 0.148, 155),
                   Isotope(155.92212, 0.2047, 156),
                   Isotope(156.923957, 0.1565, 157),
                   Isotope(157.924101, 0.2484, 158),
                   Isotope(159.927051, 0.2186, 160)}}
            Yield New Element(65) With {.symbol = "Tb", .name = "Terbium", .charge = 3, .isotopic = 158.92535, .isotopes = {Isotope(158.925343, 1.0, 159)}}
            Yield New Element(66) With {.symbol = "Dy", .name = "Dysprosium", .charge = 3, .isotopic = 163.929183, .isotopes = {Isotope(155.924278, 0.0006, 156),
                   Isotope(157.924405, 0.001, 158),
                   Isotope(159.925194, 0.0234, 160),
                   Isotope(160.92693, 0.1891, 161),
                   Isotope(161.926795, 0.2551, 162),
                   Isotope(162.928728, 0.249, 163),
                   Isotope(163.929171, 0.2818, 164)}}
            Yield New Element(67) With {.symbol = "Ho", .name = "Holmium", .charge = 3, .isotopic = 164.930332, .isotopes = {Isotope(164.930319, 1.0, 165)}}
            Yield New Element(68) With {.symbol = "Er", .name = "Erbium", .charge = 3, .isotopic = 165.930305, .isotopes = {Isotope(161.928775, 0.0014, 162),
                   Isotope(163.929197, 0.0161, 164),
                   Isotope(165.93029, 0.3361, 166),
                   Isotope(166.932045, 0.2293, 167),
                   Isotope(167.932368, 0.2678, 168),
                   Isotope(169.93546, 0.1493, 170)}}
            Yield New Element(69) With {.symbol = "Tm", .name = "Thulium", .charge = 3, .isotopic = 168.934225, .isotopes = {Isotope(168.934211, 1.0, 169)}}
            Yield New Element(70) With {.symbol = "Yb", .name = "Ytterbium", .charge = 3, .isotopic = 173.938873, .isotopes = {Isotope(167.933894, 0.0013, 168),
                   Isotope(169.934759, 0.0304, 170),
                   Isotope(170.936322, 0.1428, 171),
                   Isotope(171.9363777, 0.2183, 172),
                   Isotope(172.9382068, 0.1613, 173),
                   Isotope(173.9388581, 0.3183, 174),
                   Isotope(175.942568, 0.1276, 176)}}
            Yield New Element(71) With {.symbol = "Lu", .name = "Lutetium", .charge = 3, .isotopic = 174.940785, .isotopes = {Isotope(174.9407679, 0.9741, 175),
                   Isotope(175.9426824, 0.0259, 176)}}
            Yield New Element(72) With {.symbol = "Hf", .name = "Hafnium", .charge = 4, .isotopic = 179.946561, .isotopes = {Isotope(173.94004, 0.0016, 174),
                   Isotope(175.9414018, 0.0526, 176),
                   Isotope(176.94322, 0.186, 177),
                   Isotope(177.9436977, 0.2728, 178),
                   Isotope(178.9458151, 0.1362, 179),
                   Isotope(179.9465488, 0.3508, 180)}}
            Yield New Element(73) With {.symbol = "Ta", .name = "Tantalum", .charge = 5, .isotopic = 180.948014, .isotopes = {Isotope(179.947466, 0.00012, 180),
                   Isotope(180.947996, 0.99988, 181)}}
            Yield New Element(74) With {.symbol = "W", .name = "Tungsten", .charge = 6, .isotopic = 183.950953, .isotopes = {Isotope(179.946706, 0.0012, 180),
                   Isotope(181.948206, 0.265, 182),
                   Isotope(182.9502245, 0.1431, 183),
                   Isotope(183.9509326, 0.3064, 184),
                   Isotope(185.954362, 0.2843, 186)}}
            Yield New Element(75) With {.symbol = "Re", .name = "Rhenium", .charge = 7, .isotopic = 186.955765, .isotopes = {Isotope(184.9529557, 0.374, 185),
                  Isotope(186.9557508, 0.626, 187)}}
            Yield New Element(76) With {.symbol = "Os", .name = "Osmium", .charge = 4, .isotopic = 191.960603, .isotopes = {Isotope(183.952491, 0.0002, 184), Isotope(185.953838, 0.0159, 186), Isotope(186.9557479, 0.0196, 187), Isotope(187.955836, 0.1324, 188), Isotope(188.9581449, 0.1615, 189), Isotope(189.958445, 0.2626, 190), Isotope(191.961479, 0.4078, 192)}}
            Yield New Element(77) With {.symbol = "Ir", .name = "Iridium", .charge = 4, .isotopic = 192.962942, .isotopes = {Isotope(190.960591, 0.373, 191), Isotope(192.962924, 0.627, 193)}}
            Yield New Element(78) With {.symbol = "Pt", .name = "Platinum", .charge = 4, .isotopic = 194.964785, .isotopes = {Isotope(189.95993, 0.00014, 190), Isotope(191.961035, 0.00782, 192), Isotope(193.962664, 0.32967, 194), Isotope(194.964774, 0.33832, 195), Isotope(195.964935, 0.25242, 196), Isotope(197.967876, 0.07163, 198)}}
            Yield New Element(79) With {.symbol = "Au", .name = "Gold", .charge = 3, .isotopic = 196.96656, .isotopes = {Isotope(196.966552, 1.0, 197)}}
            Yield New Element(80) With {.symbol = "Hg", .name = "Mercury", .charge = 2, .isotopic = 201.970632, .isotopes = {Isotope(195.965815, 0.0015, 196), Isotope(197.966752, 0.0997, 198), Isotope(198.968262, 0.1687, 199), Isotope(199.968309, 0.231, 200), Isotope(200.970285, 0.1318, 201), Isotope(201.970626, 0.2986, 202), Isotope(203.973476, 0.0687, 204)}}
            Yield New Element(81) With {.symbol = "Tl", .name = "Thallium", .charge = 1, .isotopic = 204.97441, .isotopes = {Isotope(202.972329, 0.29524, 203), Isotope(204.974412, 0.70476, 205)}}
            Yield New Element(82) With {.symbol = "Pb", .name = "Lead", .charge = 2, .isotopic = 207.976641, .isotopes = {Isotope(203.973029, 0.014, 204), Isotope(205.974449, 0.241, 206), Isotope(206.975881, 0.221, 207), Isotope(207.976636, 0.524, 208)}}
            Yield New Element(83) With {.symbol = "Bi", .name = "Bismuth", .charge = 3, .isotopic = 208.980388, .isotopes = {Isotope(208.980383, 1.0, 209)}}
            Yield New Element(84) With {.symbol = "Po", .name = "Polonium", .charge = 4, .isotopic = 209, .isotopes = {Isotope(208.982416, 1.0, 209)}}
            Yield New Element(85) With {.symbol = "At", .name = "Astatine", .charge = -1, .isotopic = 210, .isotopes = {Isotope(209.987131, 1.0, 210)}}
            Yield New Element(86) With {.symbol = "Rn", .name = "Radon", .charge = 0, .isotopic = 222, .isotopes = {Isotope(222.0175705, 1.0, 222)}}
            Yield New Element(87) With {.symbol = "Fr", .name = "Francium", .charge = 1, .isotopic = 223, .isotopes = {Isotope(223.0197307, 1.0, 223)}}
            Yield New Element(88) With {.symbol = "Ra", .name = "Radium", .charge = 2, .isotopic = 227, .isotopes = {Isotope(226.0254026, 1.0, 226)}}
            Yield New Element(89) With {.symbol = "Ac", .name = "Actinium", .charge = 3, .isotopic = 227, .isotopes = {Isotope(227.027747, 1.0, 227)}}
            Yield New Element(90) With {.symbol = "Th", .name = "Thorium", .charge = 4, .isotopic = 232.038054, .isotopes = {Isotope(232.0380504, 1.0, 232)}}
            Yield New Element(91) With {.symbol = "Pa", .name = "Protactinium", .charge = 5, .isotopic = 231, .isotopes = {Isotope(231.0358789, 1.0, 231)}}
            Yield New Element(92) With {.symbol = "U", .name = "Uranium", .charge = 6, .isotopic = 238.050786, .isotopes = {Isotope(234.0409456, 0.000055, 234), Isotope(235.0439231, 0.0072, 235), Isotope(238.0507826, 0.992745, 238)}}
            Yield New Element(93) With {.symbol = "Np", .name = "Neptunium", .charge = 5, .isotopic = 237, .isotopes = {Isotope(237.0481673, 1.0, 237)}}
            Yield New Element(94) With {.symbol = "Pu", .name = "Plutonium", .charge = 4, .isotopic = 244, .isotopes = {Isotope(244.064198, 1.0, 244)}}
            Yield New Element(95) With {.symbol = "Am", .name = "Americium", .charge = 3, .isotopic = 243, .isotopes = {Isotope(243.0613727, 1.0, 243)}}
            Yield New Element(96) With {.symbol = "Cm", .name = "Curium", .charge = 3, .isotopic = 247, .isotopes = {Isotope(247.070347, 1.0, 247)}}
            Yield New Element(97) With {.symbol = "Bk", .name = "Berkelium", .charge = 3, .isotopic = 247, .isotopes = {Isotope(247.070299, 1.0, 247)}}
            Yield New Element(98) With {.symbol = "Cf", .name = "Californium", .charge = 3, .isotopic = 251, .isotopes = {Isotope(251.07958, 1.0, 251)}}
            Yield New Element(99) With {.symbol = "Es", .name = "Einsteinium", .charge = 3, .isotopic = 252, .isotopes = {Isotope(252.08297, 1.0, 252)}}
            Yield New Element(100) With {.symbol = "Fm", .name = "Fermium", .charge = 3, .isotopic = 257, .isotopes = {Isotope(257.095099, 1.0, 257)}}
            Yield New Element(101) With {.symbol = "Md", .name = "Mendelevium", .charge = 3, .isotopic = 258, .isotopes = {Isotope(258.098425, 1.0, 258)}}
            Yield New Element(102) With {.symbol = "No", .name = "Nobelium", .charge = 3, .isotopic = 269, .isotopes = {Isotope(259.10102, 1.0, 259)}}
            Yield New Element(103) With {.symbol = "Lr", .name = "Lawrencium", .charge = 3, .isotopic = 260, .isotopes = {Isotope(262.10969, 1.0, 262)}}
        End Function

    End Class
End Namespace
