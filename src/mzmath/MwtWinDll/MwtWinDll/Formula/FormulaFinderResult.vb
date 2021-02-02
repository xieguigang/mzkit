#Region "Microsoft.VisualBasic::2e4aeeb3d855c946dfab0db7e44bf340, MwtWinDll\MwtWinDll\Formula\FormulaFinderResult.vb"

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

    '     Class FormulaFinderResult
    ' 
    '         Properties: ChargeState, DeltaMass, DeltaMassIsPPM, Mass, MZ
    '                     PercentComposition
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetPercentCompInfo, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data.Linq.Mapping
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace FormulaFinder

    Public Class FormulaFinderResult : Inherits FormulaComposition

        Protected Friend SortKey As String

        Public Property Mass As Double
        Public Property DeltaMass As Double
        Public Property DeltaMassIsPPM As Boolean

        <Column(Name:="M/Z")>
        Public Property MZ As Double
        Public Property ChargeState As Integer

        ''' <summary>
        ''' Percent composition results (only valid if matching percent compositions)
        ''' </summary>
        ''' <remarks>
        ''' Keys are element or abbreviation symbols, values are percent composition, between 0 and 100
        ''' </remarks>
        Public Property PercentComposition As Dictionary(Of String, Double)

        Public Sub New(newEmpiricalFormula$, empiricalResultSymbols As Dictionary(Of String, Integer))
            Call MyBase.New(counts:=empiricalResultSymbols, formula:=newEmpiricalFormula$)

            SortKey = String.Empty
            PercentComposition = New Dictionary(Of String, Double)
        End Sub

        Public Function GetPercentCompInfo() As String
            Dim sb As New StringBuilder

            For Each percentCompValue In PercentComposition
                sb.Append(" " & percentCompValue.Key & "=" & percentCompValue.Value.ToString("0.00") & "%")
            Next

            Return sb.ToString().TrimStart()
        End Function

        Public Overloads Function ToString() As String
            Dim list As New List(Of String) From {
                EmpiricalFormula, "MW=" & Mass.ToString("0.0000")
            }
            If DeltaMassIsPPM Then
                list += "dm=" & DeltaMass.ToString("0.00" & " ppm")
            Else
                list += "dm=" & DeltaMass.ToString("0.0000")
            End If

            Return list.JoinBy(ASCII.TAB)
        End Function
    End Class
End Namespace
