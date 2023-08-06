#Region "Microsoft.VisualBasic::ded11b00b65592d5334cbd59eec0b265, mzkit\src\mzmath\MSEngine\IMzQuery.vb"

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

'   Total Lines: 49
'    Code Lines: 35
' Comment Lines: 5
'   Blank Lines: 9
'     File Size: 1.52 KB


' Interface IMzQuery
' 
'     Function: GetAnnotation, GetDbXref, GetMetadata, MSetAnnotation, QueryByMz
' 
' Module MetalIons
' 
'     Function: HasMetalIon, IsOrganic
' 
' /********************************************************************************/

#End Region

Public Interface IMzQuery

    Function QueryByMz(mz As Double) As IEnumerable(Of MzQuery)
    Function GetAnnotation(uniqueId As String) As (name As String, formula As String)
    Function GetMetadata(uniqueId As String) As Object
    Function GetDbXref(uniqueId As String) As Dictionary(Of String, String)

    ''' <summary>
    ''' query a set of m/z peak list
    ''' </summary>
    ''' <param name="mzlist"></param>
    ''' <returns></returns>
    Function MSetAnnotation(mzlist As IEnumerable(Of Double), Optional topN As Integer = 3) As IEnumerable(Of MzQuery)

End Interface

Public Module MetalIons

    Friend ReadOnly ions As String() = {
        "Cu", "Fe", "Co", "Mn", "Al",
        "Ba", "Be", "Cs", "Ca", "Cr",
        "Pb", "Li", "Mg", "Hg", "Ni",
        "Ag", "Na", "Sr", "Sn",
        "Au", "Zn", "Ce", "Tl", "In",
        "V", "As", "Mo", "Sm", "Cd",
        "Si", "Zr", "Ru", "Se", "Nd",
        "Sb", "I", "Re", "Er", "Tc",
        "Gd", "Te", "La", "Rb", "Lu",
        "Ho", "W", "Ga", "Br", "Y", "U",
        "Ti", "Th", "Ta", "Rh", "Pu", "Pt",
        "Po", "Pd", "Os", "Nb", "K", "Hf",
        "Bi", "Ge", "Cl", "Ac", "B", "Sc",
        "Np", "Ra", "F"
    } _
    .Distinct _
    .ToArray

    Public Function HasMetalIon(formula As String) As Boolean
        For Each ion As String In ions
            If InStr(formula, ion) > 0 Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function IsOrganic(formula As String) As Boolean
        Return InStr(formula, "C") > 0 AndAlso InStr(formula, "H") > 0
    End Function
End Module
