#Region "Microsoft.VisualBasic::c05023d32ff309dbdd4f5972a4253767, src\metadb\Chemoinformatics\Formula\Element.vb"

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

'     Class Element
' 
'         Properties: charge, isotopic, name
' 
'         Function: MemoryLoadElements, MemoryPopulateElements, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.IsotopicPatterns
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Formula

    ''' <summary>
    ''' Data Load Statements
    ''' Uncertainties from CRC Handbook of Chemistry and Physics
    ''' For Radioactive elements, the most stable isotope is NOT used;
    ''' instead, an average Mol. Weight is used, just like with other elements.
    ''' Data obtained from the Perma-Chart Science Series periodic table, 1993.
    ''' Uncertainties from CRC Handoobk of Chemistry and Physics, except for
    ''' Radioactive elements, where uncertainty was estimated to be .n5 where
    ''' intSpecificElementProperty represents the number digits after the decimal point but before the last
    ''' number of the molecular weight.
    ''' For example, for No, MW = 259.1009 (?.0005)
    ''' </summary>
    Public Class Element

        ''' <summary>
        ''' the element atom symbol
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

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function MemoryLoadElements() As Dictionary(Of String, Element)
            Return MemoryPopulateElements.ToDictionary(Function(e) e.symbol)
        End Function

        Public Const H As Double = 1.0078246

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function Isotope(mass As Double, prob As Double, num As Integer) As Isotope
            Return New Isotope With {.Mass = mass, .Prob = prob, .NumNeutrons = num}
        End Function

        Private Shared Iterator Function MemoryPopulateElements() As IEnumerable(Of Element)
            Yield New Element With {.symbol = "H", .name = "Hydrogen", .charge = 1, .isotopic = H, .isotopes = {Isotope(1.0078250321, 0.999885, 1), Isotope(2.014101778, 0.000115, 2)}}
            Yield New Element With {.symbol = "D", .name = "Deuterium", .charge = 1, .isotopic = 2.014101778, .isotopes = {Isotope(2.014101778, 0.000115, 2)}}
            Yield New Element With {.symbol = "He", .name = "Helium", .charge = 0, .isotopic = 4.0026029, .isotopes = {Isotope(3.0160293097, 0.00000137, 3), Isotope(4.0026032497, 0.99999863, 4)}}
            Yield New Element With {.symbol = "Li", .name = "Lithium", .charge = 1, .isotopic = 7.016005, .isotopes = {Isotope(6.0151223, 0.0759, 6), Isotope(7.016004, 0.9241, 7)}}
            Yield New Element With {.symbol = "Be", .name = "Beryllium", .charge = 2, .isotopic = 9.012183, .isotopes = {Isotope(9.0121821, 1.0, 9)}}
            Yield New Element With {.symbol = "B", .name = "Boron", .charge = 3, .isotopic = 11.009305, .isotopes = {Isotope(10.012937, 0.199, 10), Isotope(11.0093055, 0.801, 11)}}
            Yield New Element With {.symbol = "C", .name = "Carbon", .charge = 4, .isotopic = 12, .isotopes = {Isotope(12.0, 0.9893, 12), Isotope(13.0033548378, 0.0107, 13)}}
            Yield New Element With {.symbol = "N", .name = "Nitrogen", .charge = -3, .isotopic = 14.003074, .isotopes = {Isotope(14.0030740052, 0.99632, 14), Isotope(15.0001088984, 0.00368, 15)}}
            Yield New Element With {.symbol = "O", .name = "Oxygen", .charge = -2, .isotopic = 15.994915, .isotopes = {Isotope(15.9949146221, 0.99757, 16), Isotope(16.9991315, 0.00038, 17), Isotope(17.9991604, 0.00205, 18)}}
            Yield New Element With {.symbol = "F", .name = "Fluorine", .charge = -1, .isotopic = 18.9984032, .isotopes = {Isotope(18.9984032, 1.0, 19)}}
            Yield New Element With {.symbol = "Ne", .name = "Neon", .charge = 0, .isotopic = 19.992439, .isotopes = {Isotope(19.9924401759, 0.9048, 20), Isotope(20.99384674, 0.0027, 21), Isotope(21.99138551, 0.0925, 22)}}
            Yield New Element With {.symbol = "Na", .name = "Sodium", .charge = 1, .isotopic = 22.98977, .isotopes = {Isotope(22.98976967, 1.0, 23)}}
            Yield New Element With {.symbol = "Mg", .name = "Magnesium", .charge = 2, .isotopic = 23.98505, .isotopes = {Isotope(23.9850419, 0.7899, 24),
                   Isotope(24.98583702, 0.1, 25),
                   Isotope(25.98259304, 0.1101, 26)}}
            Yield New Element With {.symbol = "Al", .name = "Aluminium", .charge = 3, .isotopic = 26.981541, .isotopes = {Isotope(26.98153844, 1.0, 27)}}
            Yield New Element With {.symbol = "Si", .name = "Silicon", .charge = 4, .isotopic = 27.976928, .isotopes = {Isotope(27.9769265327, 0.922297, 28),
                   Isotope(28.97649472, 0.046832, 29),
                   Isotope(29.97377022, 0.030871, 30)}}
            Yield New Element With {.symbol = "P", .name = "Phosphorus", .charge = -3, .isotopic = 30.973763, .isotopes = {Isotope(30.97376151, 1.0, 31)}}
            Yield New Element With {.symbol = "S", .name = "Sulfur", .charge = -2, .isotopic = 31.972072, .isotopes = {Isotope(31.97207069, 0.9493, 32),
                   Isotope(32.9714585, 0.0076, 33),
                   Isotope(33.96786683, 0.0429, 34),
                   Isotope(35.96708088, 0.0002, 36)}}
            Yield New Element With {.symbol = "Cl", .name = "Chlorine", .charge = -1, .isotopic = 34.968853, .isotopes = {Isotope(34.96885271, 0.7578, 35),
                   Isotope(36.9659026, 0.2422, 37)}}
            Yield New Element With {.symbol = "Ar", .name = "Argon", .charge = 0, .isotopic = 39.962383, .isotopes = {Isotope(35.96754628, 0.003365, 36),
                  Isotope(37.9627322, 0.000632, 38),
                   Isotope(39.962383123, 0.996003, 40)}}
            Yield New Element With {.symbol = "K", .name = "Potassium", .charge = 1, .isotopic = 38.963708, .isotopes = {Isotope(38.9637069, 0.932581, 39),
                   Isotope(39.96399867, 0.000117, 40),
                   Isotope(40.96182597, 0.067302, 41)}}
            Yield New Element With {.symbol = "Ca", .name = "Calcium", .charge = 2, .isotopic = 39.962591, .isotopes = {Isotope(39.9625912, 0.96941, 40),
                   Isotope(41.9586183, 0.00647, 42),
                   Isotope(42.9587668, 0.00135, 43),
                   Isotope(43.9554811, 0.02086, 44),
                   Isotope(45.9536928, 4e-05, 46),
                   Isotope(47.952534, 0.00187, 48)}}
            Yield New Element With {.symbol = "Sc", .name = "Scandium", .charge = 3, .isotopic = 44.955914, .isotopes = {Isotope(44.9559102, 1.0, 45)}}
            Yield New Element With {.symbol = "Ti", .name = "Titanium", .charge = 4, .isotopic = 47.947947, .isotopes = {Isotope(45.9526295, 0.0825, 46),
                   Isotope(46.9517638, 0.0744, 47),
                   Isotope(47.9479471, 0.7372, 48),
                   Isotope(48.9478708, 0.0541, 49),
                   Isotope(49.9447921, 0.0518, 50)}}
            Yield New Element With {.symbol = "V", .name = "Vanadium", .charge = 5, .isotopic = 50.943963, .isotopes = {Isotope(49.9471628, 0.0025, 50),
                   Isotope(50.9439637, 0.9975, 51)}}
            Yield New Element With {.symbol = "Cr", .name = "Chromium", .charge = 3, .isotopic = 51.94051, .isotopes = {Isotope(49.9460496, 0.04345, 50),
                   Isotope(51.9405119, 0.83789, 52),
                   Isotope(52.9406538, 0.09501, 53),
                   Isotope(53.9388849, 0.02365, 54)}}
            Yield New Element With {.symbol = "Mn", .name = "Manganese", .charge = 2, .isotopic = 54.938046, .isotopes = {Isotope(54.9380496, 1.0, 55)}}
            Yield New Element With {.symbol = "Fe", .name = "Iron", .charge = 3, .isotopic = 55.934939, .isotopes = {Isotope(53.9396148, 0.05845, 54),
                   Isotope(55.9349421, 0.91754, 56),
                   Isotope(56.9353987, 0.02119, 57),
                   Isotope(57.9332805, 0.00282, 58)}}
            Yield New Element With {.symbol = "Co", .name = "Cobalt", .charge = 2, .isotopic = 58.933198, .isotopes = {Isotope(58.9332002, 1.0, 59)}}
            Yield New Element With {.symbol = "Ni", .name = "Nickel", .charge = 2, .isotopic = 57.935347, .isotopes = {Isotope(57.9353479, 0.680769, 58),
                   Isotope(59.9307906, 0.262231, 60),
                   Isotope(60.9310604, 0.011399, 61),
                   Isotope(61.9283488, 0.036345, 62),
                   Isotope(63.9279696, 0.009256, 64)}}
            Yield New Element With {.symbol = "Cu", .name = "Copper", .charge = 2, .isotopic = 62.929599, .isotopes = {Isotope(62.9296011, 0.6917, 63),
                   Isotope(64.9277937, 0.3083, 65)}}
            Yield New Element With {.symbol = "Zn", .name = "Zinc", .charge = 2, .isotopic = 63.929145, .isotopes = {Isotope(63.9291466, 0.4863, 64),
                   Isotope(65.9260368, 0.279, 66),
                   Isotope(66.9271309, 0.041, 67),
                   Isotope(67.9248476, 0.1875, 68),
                   Isotope(69.925325, 0.0062, 70)}}
            Yield New Element With {.symbol = "Ga", .name = "Gallium", .charge = 3, .isotopic = 68.925581, .isotopes = {Isotope(68.925581, 0.60108, 69),
                   Isotope(70.924705, 0.39892, 71)}}
            Yield New Element With {.symbol = "Ge", .charge = 4, .isotopic = 71.92208}
            Yield New Element With {.symbol = "As", .charge = -3, .isotopic = 74.921596}
            Yield New Element With {.symbol = "Se", .charge = -2, .isotopic = 79.916521}
            Yield New Element With {.symbol = "Br", .charge = -1, .isotopic = 78.918336}
            Yield New Element With {.symbol = "Kr", .charge = 0, .isotopic = 83.911506}
            Yield New Element With {.symbol = "Rb", .charge = 1, .isotopic = 84.9118}
            Yield New Element With {.symbol = "Sr", .charge = 2, .isotopic = 87.905625}
            Yield New Element With {.symbol = "Y", .charge = 3, .isotopic = 88.905856}
            Yield New Element With {.symbol = "Zr", .charge = 4, .isotopic = 89.904708}
            Yield New Element With {.symbol = "Nb", .charge = 5, .isotopic = 92.906378}
            Yield New Element With {.symbol = "Mo", .charge = 6, .isotopic = 97.905405}
            Yield New Element With {.symbol = "Tc", .charge = 7, .isotopic = 98}
            Yield New Element With {.symbol = "Ru", .charge = 4, .isotopic = 101.90434}
            Yield New Element With {.symbol = "Rh", .charge = 3, .isotopic = 102.905503}
            Yield New Element With {.symbol = "Pd", .charge = 2, .isotopic = 105.903475}
            Yield New Element With {.symbol = "Ag", .charge = 1, .isotopic = 106.905095}
            Yield New Element With {.symbol = "Cd", .charge = 2, .isotopic = 113.903361}
            Yield New Element With {.symbol = "In", .charge = 3, .isotopic = 114.903875}
            Yield New Element With {.symbol = "Sn", .charge = 4, .isotopic = 119.902199}
            Yield New Element With {.symbol = "Sb", .charge = -3, .isotopic = 120.903824}
            Yield New Element With {.symbol = "Te", .charge = -2, .isotopic = 129.906229}
            Yield New Element With {.symbol = "I", .charge = -1, .isotopic = 126.904477}
            Yield New Element With {.symbol = "Xe", .charge = 0, .isotopic = 131.904148}
            Yield New Element With {.symbol = "Cs", .charge = 1, .isotopic = 132.905433}
            Yield New Element With {.symbol = "Ba", .charge = 2, .isotopic = 137.905236}
            Yield New Element With {.symbol = "La", .charge = 3, .isotopic = 138.906355}
            Yield New Element With {.symbol = "Ce", .charge = 3, .isotopic = 139.905442}
            Yield New Element With {.symbol = "Pr", .charge = 4, .isotopic = 140.907657}
            Yield New Element With {.symbol = "Nd", .charge = 3, .isotopic = 141.907731}
            Yield New Element With {.symbol = "Pm", .charge = 3, .isotopic = 145}
            Yield New Element With {.symbol = "Sm", .charge = 3, .isotopic = 151.919741}
            Yield New Element With {.symbol = "Eu", .charge = 3, .isotopic = 152.921243}
            Yield New Element With {.symbol = "Gd", .charge = 3, .isotopic = 157.924111}
            Yield New Element With {.symbol = "Tb", .charge = 3, .isotopic = 158.92535}
            Yield New Element With {.symbol = "Dy", .charge = 3, .isotopic = 163.929183}
            Yield New Element With {.symbol = "Ho", .charge = 3, .isotopic = 164.930332}
            Yield New Element With {.symbol = "Er", .charge = 3, .isotopic = 165.930305}
            Yield New Element With {.symbol = "Tm", .charge = 3, .isotopic = 168.934225}
            Yield New Element With {.symbol = "Yb", .charge = 3, .isotopic = 173.938873}
            Yield New Element With {.symbol = "Lu", .charge = 3, .isotopic = 174.940785}
            Yield New Element With {.symbol = "Hf", .charge = 4, .isotopic = 179.946561}
            Yield New Element With {.symbol = "Ta", .charge = 5, .isotopic = 180.948014}
            Yield New Element With {.symbol = "W", .charge = 6, .isotopic = 183.950953}
            Yield New Element With {.symbol = "Re", .charge = 7, .isotopic = 186.955765}
            Yield New Element With {.symbol = "Os", .charge = 4, .isotopic = 191.960603}
            Yield New Element With {.symbol = "Ir", .charge = 4, .isotopic = 192.962942}
            Yield New Element With {.symbol = "Pt", .charge = 4, .isotopic = 194.964785}
            Yield New Element With {.symbol = "Au", .charge = 3, .isotopic = 196.96656}
            Yield New Element With {.symbol = "Hg", .charge = 2, .isotopic = 201.970632}
            Yield New Element With {.symbol = "Tl", .charge = 1, .isotopic = 204.97441}
            Yield New Element With {.symbol = "Pb", .charge = 2, .isotopic = 207.976641}
            Yield New Element With {.symbol = "Bi", .charge = 3, .isotopic = 208.980388}
            Yield New Element With {.symbol = "Po", .charge = 4, .isotopic = 209}
            Yield New Element With {.symbol = "At", .charge = -1, .isotopic = 210}
            Yield New Element With {.symbol = "Rn", .charge = 0, .isotopic = 222}
            Yield New Element With {.symbol = "Fr", .charge = 1, .isotopic = 223}
            Yield New Element With {.symbol = "Ra", .charge = 2, .isotopic = 227}
            Yield New Element With {.symbol = "Ac", .charge = 3, .isotopic = 227}
            Yield New Element With {.symbol = "Th", .charge = 4, .isotopic = 232.038054}
            Yield New Element With {.symbol = "Pa", .charge = 5, .isotopic = 231}
            Yield New Element With {.symbol = "U", .charge = 6, .isotopic = 238.050786}
            Yield New Element With {.symbol = "Np", .charge = 5, .isotopic = 237}
            Yield New Element With {.symbol = "Pu", .charge = 4, .isotopic = 244}
            Yield New Element With {.symbol = "Am", .charge = 3, .isotopic = 243}
            Yield New Element With {.symbol = "Cm", .charge = 3, .isotopic = 247}
            Yield New Element With {.symbol = "Bk", .charge = 3, .isotopic = 247}
            Yield New Element With {.symbol = "Cf", .charge = 3, .isotopic = 251}
            Yield New Element With {.symbol = "Es", .charge = 3, .isotopic = 252}
            Yield New Element With {.symbol = "Fm", .charge = 3, .isotopic = 257}
            Yield New Element With {.symbol = "Md", .charge = 3, .isotopic = 258}
            Yield New Element With {.symbol = "No", .charge = 3, .isotopic = 269}
            Yield New Element With {.symbol = "Lr", .charge = 3, .isotopic = 260}
        End Function

    End Class
End Namespace
