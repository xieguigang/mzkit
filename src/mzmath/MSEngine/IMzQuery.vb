#Region "Microsoft.VisualBasic::b4d7e46fc422ccb55ac0d20a175780d5, mzmath\MSEngine\IMzQuery.vb"

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

    '   Total Lines: 99
    '    Code Lines: 50 (50.51%)
    ' Comment Lines: 35 (35.35%)
    '    - Xml Docs: 97.14%
    ' 
    '   Blank Lines: 14 (14.14%)
    '     File Size: 3.16 KB


    ' Interface IMzQuery
    ' 
    '     Function: MSetAnnotation, QueryByMz
    ' 
    ' Interface IMetaDb
    ' 
    '     Function: GetAnnotation, GetDbXref, GetMetadata
    ' 
    ' Module MetalIons
    ' 
    '     Function: HasMetalIon, IsMetalIon, IsOrganic
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection

''' <summary>
''' Query annotation candidates by the given m/z mass value
''' </summary>
Public Interface IMzQuery : Inherits IMetaDb

    ''' <summary>
    ''' Query annotation candidates by the given m/z mass value
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns></returns>
    Function QueryByMz(mz As Double) As IEnumerable(Of MzQuery)

    ''' <summary>
    ''' query a set of m/z peak list
    ''' </summary>
    ''' <param name="mzlist"></param>
    ''' <returns></returns>
    Function MSetAnnotation(mzlist As IEnumerable(Of Double), Optional topN As Integer = 3) As IEnumerable(Of MzQuery)

End Interface

''' <summary>
''' annotation data getter
''' </summary>
''' <remarks>
''' get by unique reference id
''' </remarks>
Public Interface IMetaDb

    Function GetAnnotation(uniqueId As String) As (name As String, formula As String)

    ''' <summary>
    ''' get the general annotation metadata object by its unique reference id
    ''' </summary>
    ''' <param name="uniqueId"></param>
    ''' <returns></returns>
    Function GetMetadata(uniqueId As String) As Object
    Function GetDbXref(uniqueId As String) As Dictionary(Of String, String)

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

    ''' <summary>
    ''' check of the given formula is metal ion or not?
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    Public Function IsMetalIon(formula As String) As Boolean
        Static ions As Index(Of String) = MetalIons.ions
        Return formula Like ions
    End Function

    ''' <summary>
    ''' check of the given formula has metal ion or not?
    ''' </summary>
    ''' <param name="formula"></param>
    ''' <returns></returns>
    Public Function HasMetalIon(formula As String) As Boolean
        For Each ion As String In ions
            ' must be case sensitive
            If InStr(formula, ion, CompareMethod.Binary) > 0 Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Function IsOrganic(formula As String) As Boolean
        Dim atoms As Formula = FormulaScanner.ScanFormula(formula)
        If atoms Is Nothing Then
            Return False
        End If
        Return atoms.CheckElement("C") AndAlso (atoms.CheckElement("H") OrElse atoms.CheckElement("O"))
    End Function
End Module
