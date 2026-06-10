Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Ms1

    ''' <summary>
    ''' an abstract mass window model
    ''' </summary>
    Public Interface IMassBin

        Property mass As Double
        Property min As Double
        Property max As Double

    End Interface

    ''' <summary>
    ''' an abstract model of the mass collection
    ''' </summary>
    Public Interface IMassSet

        Property mass As Double()
        Property min As Double()
        Property max As Double()

    End Interface

    <HideModuleName>
    Public Module MassExtensions

        ''' <summary>
        ''' get all mass value from the mass window data
        ''' </summary>
        ''' <param name="masslist"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Mass(masslist As IEnumerable(Of MassWindow)) As Double()
            Return masslist.Select(Function(mzi) mzi.mass).ToArray
        End Function

        <Extension>
        Public Function AverageMzDiff(masslist As IEnumerable(Of MassWindow)) As Double
            Return masslist.Select(Function(mzi) mzi.GetMzDiff).IteratesALL.Average
        End Function

        <Extension>
        Public Iterator Function MassList(massSet As IMassSet) As IEnumerable(Of MassWindow)
            For i As Integer = 0 To massSet.mass.Length - 1
                Yield New MassWindow With {
                    .mass = massSet.mass(i),
                    .mzmin = massSet.min(i),
                    .mzmax = massSet.max(i)
                }
            Next
        End Function

        Public Function MzWindowDescription(mzmax As Double, mzmin As Double, Optional ppm As Double = 30) As String
            If PPMmethod.PPM(mzmin, mzmax) > 30 Then
                Return $"da:{(mzmax - mzmin).ToString("F3")}"
            Else
                Return $"ppm:{PPMmethod.PPM(mzmin, mzmax).ToString("F1")}"
            End If
        End Function

    End Module
End Namespace