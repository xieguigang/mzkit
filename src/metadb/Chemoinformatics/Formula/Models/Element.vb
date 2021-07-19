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

        Public Property symbol As String

        Public Property charge As Integer
        ''' <summary>
        ''' isotopic Element Weights
        ''' </summary>
        ''' <returns></returns>
        Public Property isotopic As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function MemoryLoadElements() As Dictionary(Of String, Element)
            Return MemoryPopulateElements.ToDictionary(Function(e) e.symbol)
        End Function

        Public Const H As Double = 1.0078246

        Private Shared Iterator Function MemoryPopulateElements() As IEnumerable(Of Element)
            Yield New Element With {.symbol = "H", .charge = 1, .isotopic = H}
            Yield New Element With {.symbol = "He", .charge = 0, .isotopic = 4.0026029}
            Yield New Element With {.symbol = "Li", .charge = 1, .isotopic = 7.016005}
            Yield New Element With {.symbol = "Be", .charge = 2, .isotopic = 9.012183}
            Yield New Element With {.symbol = "B", .charge = 3, .isotopic = 11.009305}
            Yield New Element With {.symbol = "C", .charge = 4, .isotopic = 12}
            Yield New Element With {.symbol = "N", .charge = -3, .isotopic = 14.003074}
            Yield New Element With {.symbol = "O", .charge = -2, .isotopic = 15.994915}
            Yield New Element With {.symbol = "F", .charge = -1, .isotopic = 18.9984032}
            Yield New Element With {.symbol = "Ne", .charge = 0, .isotopic = 19.992439}
            Yield New Element With {.symbol = "Na", .charge = 1, .isotopic = 22.98977}
            Yield New Element With {.symbol = "Mg", .charge = 2, .isotopic = 23.98505}
            Yield New Element With {.symbol = "Al", .charge = 3, .isotopic = 26.981541}
            Yield New Element With {.symbol = "Si", .charge = 4, .isotopic = 27.976928}
            Yield New Element With {.symbol = "P", .charge = -3, .isotopic = 30.973763}
            Yield New Element With {.symbol = "S", .charge = -2, .isotopic = 31.972072}
            Yield New Element With {.symbol = "Cl", .charge = -1, .isotopic = 34.968853}
            Yield New Element With {.symbol = "Ar", .charge = 0, .isotopic = 39.962383}
            Yield New Element With {.symbol = "K", .charge = 1, .isotopic = 38.963708}
            Yield New Element With {.symbol = "Ca", .charge = 2, .isotopic = 39.962591}
            Yield New Element With {.symbol = "Sc", .charge = 3, .isotopic = 44.955914}
            Yield New Element With {.symbol = "Ti", .charge = 4, .isotopic = 47.947947}
            Yield New Element With {.symbol = "V", .charge = 5, .isotopic = 50.943963}
            Yield New Element With {.symbol = "Cr", .charge = 3, .isotopic = 51.94051}
            Yield New Element With {.symbol = "Mn", .charge = 2, .isotopic = 54.938046}
            Yield New Element With {.symbol = "Fe", .charge = 3, .isotopic = 55.934939}
            Yield New Element With {.symbol = "Co", .charge = 2, .isotopic = 58.933198}
            Yield New Element With {.symbol = "Ni", .charge = 2, .isotopic = 57.935347}
            Yield New Element With {.symbol = "Cu", .charge = 2, .isotopic = 62.929599}
            Yield New Element With {.symbol = "Zn", .charge = 2, .isotopic = 63.929145}
            Yield New Element With {.symbol = "Ga", .charge = 3, .isotopic = 68.925581}
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
