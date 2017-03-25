Imports System.Data.Linq.Mapping
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Public Class FormulaFinderResult

    Protected Friend SortKey As String

    Public ReadOnly EmpiricalFormula As String
    Public ReadOnly Property CountsByElement As Dictionary(Of String, Integer)
    Public Property Mass As Double
    Public Property DeltaMass As Double
    Public Property DeltaMassIsPPM As Boolean

    <Column(Name:="M/Z")>
    Public Property MZ As Double
    Public Property ChargeState As Integer

    ''' <summary>
    ''' Percent composition results (only valid if matching percent compositions)
    ''' </summary>
    ''' <remarks>Keys are element or abbreviation symbols, values are percent composition, between 0 and 100</remarks>
    Public Property PercentComposition As Dictionary(Of String, Double)

    Public Sub New(newEmpiricalFormula As String, empiricalResultSymbols As Dictionary(Of String, Integer))
        EmpiricalFormula = newEmpiricalFormula
        CountsByElement = empiricalResultSymbols
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
