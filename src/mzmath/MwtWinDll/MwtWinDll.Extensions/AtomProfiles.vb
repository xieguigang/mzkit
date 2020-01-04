#Region "Microsoft.VisualBasic::8a257369468aa046e22fbd133c7f0312, src\mzmath\MwtWinDll\MwtWinDll.Extensions\AtomProfiles.vb"

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

    ' Structure AtomProfiles
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: ToString
    ' 
    '     Sub: SetAtoms
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Public Structure AtomProfiles

    ''' <summary>
    ''' Values值的和必须要等于100或者全部为零！
    ''' </summary>
    Dim atoms As Dictionary(Of String, Integer)

    Sub New(atoms As IEnumerable(Of String))
        Me.atoms = atoms.ToDictionary(Function(atom) atom, Function() 0%)
    End Sub

    Const InvalidArguments$ = "Atom composition neither SUM(profile) <> 100 or ALL_SUM should equals to ZERO!"

    Public Sub SetAtoms(ByRef finder As MolecularWeightCalculator)
        Dim sum% = atoms.Values.Sum

        If sum = 100% Then
            For Each atom In atoms
                Call finder.FormulaFinder.AddCandidateElement(atom.Key, atom.Value)
            Next
        ElseIf sum = 0% Then
            For Each atom$ In atoms.Keys
                Call finder.FormulaFinder.AddCandidateElement(atom)
            Next
        Else
            Dim ex As New Exception(atoms.GetJson)
            Dim json$ = atoms.Values.ToArray.GetJson

            Throw New ArgumentOutOfRangeException(InvalidArguments & vbCrLf & vbCrLf & json, ex)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return atoms.GetJson
    End Function
End Structure
