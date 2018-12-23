Imports System.Data.Linq.Mapping
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Public Class FormulaComposition

    Default Public ReadOnly Property GetAtomCount(atom As String) As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return CountsByElement.TryGetValue(atom)
        End Get
    End Property

    Public ReadOnly Property CountsByElement As Dictionary(Of String, Integer)
    Public ReadOnly Property EmpiricalFormula As String

    Sub New(counts As IDictionary(Of String, Integer), Optional formula$ = Nothing)
        CountsByElement = New Dictionary(Of String, Integer)(counts)

        If formula.StringEmpty Then
            EmpiricalFormula = CountsByElement _
                .Select(Function(e) If(e.Value = 1, e.Key, e.Key & e.Value)) _
                .JoinBy("")
        Else
            EmpiricalFormula = formula
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return EmpiricalFormula
    End Function

    Public Shared Operator *(composition As FormulaComposition, n%) As FormulaComposition
        Dim newFormula$ = $"({composition}){n}"
        Dim newComposition = composition _
            .CountsByElement _
            .ToDictionary(Function(e) e.Key,
                          Function(e)
                              Return e.Value * n
                          End Function)

        Return New FormulaComposition(newComposition, newFormula)
    End Operator
End Class

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
