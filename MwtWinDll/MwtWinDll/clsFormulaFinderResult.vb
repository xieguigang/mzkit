Imports System.Collections.Generic

Public Class clsFormulaFinderResult

    Public ReadOnly EmpiricalFormula As String

    Private ReadOnly mCountsByElement As Dictionary(Of String, Integer)

    ' ReSharper disable once ConvertToVbAutoProperty
    Public ReadOnly Property CountsByElement As Dictionary(Of String, Integer)
        Get
            Return mCountsByElement
        End Get
    End Property

    Public Property Mass As Double
    Public Property DeltaMass As Double
    Public Property DeltaMassIsPPM As Boolean
    Public Property MZ As Double

    Public Property ChargeState As Integer

    ''' <summary>
    ''' Percent composition results (only valid if matching percent compositions)
    ''' </summary>
    ''' <remarks>Keys are element or abbreviation symbols, values are percent composition, between 0 and 100</remarks>
    Public Property PercentComposition As Dictionary(Of String, Double)

    Public SortKey As String

    Public Sub New(newEmpiricalFormula As String, empiricalResultSymbols As Dictionary(Of String, Integer))
        EmpiricalFormula = newEmpiricalFormula
        mCountsByElement = empiricalResultSymbols

        SortKey = String.Empty
        PercentComposition = New Dictionary(Of String, Double)
    End Sub

    Public Overloads Function ToString() As String
        If DeltaMassIsPPM Then
            Return EmpiricalFormula & "   MW=" & Mass.ToString("0.0000") & "   dm=" & DeltaMass.ToString("0.00" & " ppm")
        Else
            Return EmpiricalFormula & "   MW=" & Mass.ToString("0.0000") & "   dm=" & DeltaMass.ToString("0.0000")
        End If
    End Function

   

End Class
