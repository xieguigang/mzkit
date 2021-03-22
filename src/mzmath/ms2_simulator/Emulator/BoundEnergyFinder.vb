#Region "Microsoft.VisualBasic::9b6e2a241bacb304a0213e381513cc68, src\mzmath\ms2_simulator\Emulator\BoundEnergyFinder.vb"

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

    ' Class BoundEnergyFinder
    ' 
    '     Properties: min
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FindByKCFAtoms, TranslateKCFAtom
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.Model

Public Class BoundEnergyFinder

    ReadOnly boundTable As Dictionary(Of String, BoundEnergy)

    Public ReadOnly Property min As Double

    Sub New()
        Dim bounds = BoundEnergy.GetEnergyTable _
            .GroupBy(Function(b) b.bound) _
            .ToArray
        Dim table As Dictionary(Of String, BoundEnergy) = bounds _
            .ToDictionary(Function(b) b.Key,
                          Function(g)
                              If g.Count = 1 Then
                                  Return g.First
                              Else
                                  Dim energy = Aggregate b In g Into Average(b.H0)
                                  Dim template As BoundEnergy = g.First
                                  Dim avg As New BoundEnergy With {
                                      .atom1 = template.atom1,
                                      .atom2 = template.atom2,
                                      .bound = template.bound,
                                      .bounds = template.bounds,
                                      .H0 = energy
                                  }

                                  Return avg
                              End If
                          End Function)

        boundTable = table
        min = Aggregate g In bounds Into Min(g.Min(Function(b) b.H0))
    End Sub

    ''' <summary>
    ''' Find energy by KCF atoms pair
    ''' </summary>
    ''' <param name="atom1"></param>
    ''' <param name="atom2"></param>
    ''' <param name="n">
    ''' Get bounds number from KCF model directly
    ''' </param>
    ''' <returns></returns>
    Public Function FindByKCFAtoms(atom1 As String, atom2 As String, n%) As Double
        Dim link As (f$, r$)

        atom1 = TranslateKCFAtom(atom1)
        atom2 = TranslateKCFAtom(atom2)

        Select Case n
            Case 1 : link = ($"{atom1}-{atom2}", $"{atom2}-{atom1}")
            Case 2 : link = ($"{atom1}={atom2}", $"{atom2}={atom1}")
            Case 3 : link = ($"{atom1}≡{atom2}", $"{atom2}≡{atom1}")
            Case Else
                Throw New NotImplementedException
        End Select

        If boundTable.ContainsKey(link.f) Then
            Return boundTable(link.f).H0
        ElseIf boundTable.ContainsKey(link.r) Then
            Return boundTable(link.r).H0
        Else
            Throw New KeyNotFoundException()
        End If
    End Function

    ''' <summary>
    ''' KCF atom label => atom character
    ''' </summary>
    ''' <param name="atom"></param>
    ''' <returns></returns>
    Public Shared Function TranslateKCFAtom(atom As String) As String
        Dim type As KegAtomType = KegAtomType.GetAtom(atom)

        Select Case type.type
            Case KegAtomType.Types.Carbon
                Return "C"
            Case KegAtomType.Types.Nitrogen
                Return "N"
            Case KegAtomType.Types.Oxygen
                Return "O"
            Case KegAtomType.Types.Sulfur
                Return "S"
            Case KegAtomType.Types.Phosphorus
                Return "P"
            Case KegAtomType.Types.Halogen
                ' F/Cl/Br/I
                Return type.formula
            Case Else
                Return "NA"
        End Select
    End Function
End Class
