Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Formula

    Public Class Element

        ' Data Load Statements
        ' Uncertainties from CRC Handbook of Chemistry and Physics
        ' For Radioactive elements, the most stable isotope is NOT used;
        ' instead, an average Mol. Weight is used, just like with other elements.
        ' Data obtained from the Perma-Chart Science Series periodic table, 1993.
        ' Uncertainties from CRC Handoobk of Chemistry and Physics, except for
        ' Radioactive elements, where uncertainty was estimated to be .n5 where
        ' intSpecificElementProperty represents the number digits after the decimal point but before the last
        ' number of the molecular weight.
        ' For example, for No, MW = 259.1009 (?.0005)

        Public Property name As String
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
            Return MemoryPopulateElements.ToDictionary(Function(e) e.name)
        End Function

        Private Shared Iterator Function MemoryPopulateElements() As IEnumerable(Of Element)
            Yield New Element With {.name = "H", .charge = 1, .isotopic = 1.0078246}
            Yield New Element With {.name = "He", .charge = 0, .isotopic = 4.0026029}
            Yield New Element With {.name = "Li", .charge = 1, .isotopic = 7.016005}
            Yield New Element With {.name = "Be", .charge = 2, .isotopic = 9.012183}
            Yield New Element With {.name = "B", .charge = 3, .isotopic = 11.009305}
            Yield New Element With {.name = "C", .charge = 4, .isotopic = 12}
            Yield New Element With {.name = "N", .charge = -3, .isotopic = 14.003074}
            Yield New Element With {.name = "O", .charge = -2, .isotopic = 15.994915}
            Yield New Element With {.name = "F", .charge = -1, .isotopic = 18.9984032}
            Yield New Element With {.name = "Ne", .charge = 0, .isotopic = 19.992439}
            Yield New Element With {.name = "Na", .charge = 1, .isotopic = 22.98977}
            Yield New Element With {.name = "Mg", .charge = 2, .isotopic = 23.98505}
            Yield New Element With {.name = "Al", .charge = 3, .isotopic = 26.981541}
            Yield New Element With {.name = "Si", .charge = 4, .isotopic = 27.976928}
            Yield New Element With {.name = "P", .charge = -3, .isotopic = 30.973763}
            Yield New Element With {.name = "S", .charge = -2, .isotopic = 31.972072}
            Yield New Element With {.name = "Cl", .charge = -1, .isotopic = 34.968853}
            Yield New Element With {.name = "Ar", .charge = 0, .isotopic = 39.962383}
            Yield New Element With {.name = "K", .charge = 1, .isotopic = 38.963708}
            Yield New Element With {.name = "Ca", .charge = 2, .isotopic = 39.962591}
            Yield New Element With {.name = "Sc", .charge = 3, .isotopic = 44.955914}
            Yield New Element With {.name = "Ti", .charge = 4, .isotopic = 47.947947}
            Yield New Element With {.name = "V", .charge = 5, .isotopic = 50.943963}
            Yield New Element With {.name = "Cr", .charge = 3, .isotopic = 51.94051}
            Yield New Element With {.name = "Mn", .charge = 2, .isotopic = 54.938046}
            Yield New Element With {.name = "Fe", .charge = 3, .isotopic = 55.934939}
            Yield New Element With {.name = "Co", .charge = 2, .isotopic = 58.933198}
            Yield New Element With {.name = "Ni", .charge = 2, .isotopic = 57.935347}
            Yield New Element With {.name = "Cu", .charge = 2, .isotopic = 62.929599}
            Yield New Element With {.name = "Zn", .charge = 2, .isotopic = 63.929145}
            Yield New Element With {.name = "Ga", .charge = 3, .isotopic = 68.925581}
            Yield New Element With {.name = "Ge", .charge = 4, .isotopic = 71.92208}
            Yield New Element With {.name = "As", .charge = -3, .isotopic = 74.921596}
            Yield New Element With {.name = "Se", .charge = -2, .isotopic = 79.916521}
            Yield New Element With {.name = "Br", .charge = -1, .isotopic = 78.918336}
            Yield New Element With {.name = "Kr", .charge = 0, .isotopic = 83.911506}
            Yield New Element With {.name = "Rb", .charge = 1, .isotopic = 84.9118}
            Yield New Element With {.name = "Sr", .charge = 2, .isotopic = 87.905625}
            Yield New Element With {.name = "Y", .charge = 3, .isotopic = 88.905856}
            Yield New Element With {.name = "Zr", .charge = 4, .isotopic = 89.904708}
            Yield New Element With {.name = "Nb", .charge = 5, .isotopic = 92.906378}
            Yield New Element With {.name = "Mo", .charge = 6, .isotopic = 97.905405}
            Yield New Element With {.name = "Tc", .charge = 7, .isotopic = 98}
            Yield New Element With {.name = "Ru", .charge = 4, .isotopic = 101.90434}
            Yield New Element With {.name = "Rh", .charge = 3, .isotopic = 102.905503}
            Yield New Element With {.name = "Pd", .charge = 2, .isotopic = 105.903475}
            Yield New Element With {.name = "Ag", .charge = 1, .isotopic = 106.905095}
            Yield New Element With {.name = "Cd", .charge = 2, .isotopic = 113.903361}
            Yield New Element With {.name = "In", .charge = 3, .isotopic = 114.903875}
            Yield New Element With {.name = "Sn", .charge = 4, .isotopic = 119.902199}
            Yield New Element With {.name = "Sb", .charge = -3, .isotopic = 120.903824}
            Yield New Element With {.name = "Te", .charge = -2, .isotopic = 129.906229}
            Yield New Element With {.name = "I", .charge = -1, .isotopic = 126.904477}
            Yield New Element With {.name = "Xe", .charge = 0, .isotopic = 131.904148}
            Yield New Element With {.name = "Cs", .charge = 1, .isotopic = 132.905433}
            Yield New Element With {.name = "Ba", .charge = 2, .isotopic = 137.905236}
            Yield New Element With {.name = "La", .charge = 3, .isotopic = 138.906355}
            Yield New Element With {.name = "Ce", .charge = 3, .isotopic = 139.905442}
            Yield New Element With {.name = "Pr", .charge = 4, .isotopic = 140.907657}
            Yield New Element With {.name = "Nd", .charge = 3, .isotopic = 141.907731}
            Yield New Element With {.name = "Pm", .charge = 3, .isotopic = 145}
            Yield New Element With {.name = "Sm", .charge = 3, .isotopic = 151.919741}
            Yield New Element With {.name = "Eu", .charge = 3, .isotopic = 152.921243}
            Yield New Element With {.name = "Gd", .charge = 3, .isotopic = 157.924111}
            Yield New Element With {.name = "Tb", .charge = 3, .isotopic = 158.92535}
            Yield New Element With {.name = "Dy", .charge = 3, .isotopic = 163.929183}
            Yield New Element With {.name = "Ho", .charge = 3, .isotopic = 164.930332}
            Yield New Element With {.name = "Er", .charge = 3, .isotopic = 165.930305}
            Yield New Element With {.name = "Tm", .charge = 3, .isotopic = 168.934225}
            Yield New Element With {.name = "Yb", .charge = 3, .isotopic = 173.938873}
            Yield New Element With {.name = "Lu", .charge = 3, .isotopic = 174.940785}
            Yield New Element With {.name = "Hf", .charge = 4, .isotopic = 179.946561}
            Yield New Element With {.name = "Ta", .charge = 5, .isotopic = 180.948014}
            Yield New Element With {.name = "W", .charge = 6, .isotopic = 183.950953}
            Yield New Element With {.name = "Re", .charge = 7, .isotopic = 186.955765}
            Yield New Element With {.name = "Os", .charge = 4, .isotopic = 191.960603}
            Yield New Element With {.name = "Ir", .charge = 4, .isotopic = 192.962942}
            Yield New Element With {.name = "Pt", .charge = 4, .isotopic = 194.964785}
            Yield New Element With {.name = "Au", .charge = 3, .isotopic = 196.96656}
            Yield New Element With {.name = "Hg", .charge = 2, .isotopic = 201.970632}
            Yield New Element With {.name = "Tl", .charge = 1, .isotopic = 204.97441}
            Yield New Element With {.name = "Pb", .charge = 2, .isotopic = 207.976641}
            Yield New Element With {.name = "Bi", .charge = 3, .isotopic = 208.980388}
            Yield New Element With {.name = "Po", .charge = 4, .isotopic = 209}
            Yield New Element With {.name = "At", .charge = -1, .isotopic = 210}
            Yield New Element With {.name = "Rn", .charge = 0, .isotopic = 222}
            Yield New Element With {.name = "Fr", .charge = 1, .isotopic = 223}
            Yield New Element With {.name = "Ra", .charge = 2, .isotopic = 227}
            Yield New Element With {.name = "Ac", .charge = 3, .isotopic = 227}
            Yield New Element With {.name = "Th", .charge = 4, .isotopic = 232.038054}
            Yield New Element With {.name = "Pa", .charge = 5, .isotopic = 231}
            Yield New Element With {.name = "U", .charge = 6, .isotopic = 238.050786}
            Yield New Element With {.name = "Np", .charge = 5, .isotopic = 237}
            Yield New Element With {.name = "Pu", .charge = 4, .isotopic = 244}
            Yield New Element With {.name = "Am", .charge = 3, .isotopic = 243}
            Yield New Element With {.name = "Cm", .charge = 3, .isotopic = 247}
            Yield New Element With {.name = "Bk", .charge = 3, .isotopic = 247}
            Yield New Element With {.name = "Cf", .charge = 3, .isotopic = 251}
            Yield New Element With {.name = "Es", .charge = 3, .isotopic = 252}
            Yield New Element With {.name = "Fm", .charge = 3, .isotopic = 257}
            Yield New Element With {.name = "Md", .charge = 3, .isotopic = 258}
            Yield New Element With {.name = "No", .charge = 3, .isotopic = 269}
            Yield New Element With {.name = "Lr", .charge = 3, .isotopic = 260}
        End Function

    End Class
End Namespace