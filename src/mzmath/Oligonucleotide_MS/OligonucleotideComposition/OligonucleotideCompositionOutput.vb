Imports System.IO

Public Class OligonucleotideCompositionOutput

    ''' <summary>
    ''' 1
    ''' </summary>
    Public Property ObservedMass As Double
    ''' <summary>
    ''' 2
    ''' </summary>
    Public Property TheoreticalMass As Double
    ''' <summary>
    ''' 3
    ''' </summary>
    Public Property ErrorPpm As Double
    ''' <summary>
    ''' 4
    ''' </summary>
    Public Property OfpA As Integer
    ''' <summary>
    ''' 5
    ''' </summary>
    Public Property OfpG As Integer
    ''' <summary>
    ''' 6
    ''' </summary>
    Public Property OfpC As Integer
    ''' <summary>
    ''' 7
    ''' </summary>
    Public Property OfpV As Integer
    ''' <summary>
    ''' 8
    ''' </summary>
    Public Property Modification As String
    ''' <summary>
    ''' 10
    ''' </summary>
    Public Property OfBases As Integer

    Friend Sub SetBaseNumber(i As Integer, n As Integer)
        Select Case i
            Case 1 : OfpA = n
            Case 2 : OfpG = n
            Case 3 : OfpC = n
            Case 4 : OfpV = n

            Case Else
                Throw New OutOfMemoryException(i)
        End Select
    End Sub

    ''' <summary>
    ''' print table
    ''' </summary>
    ''' <param name="output"></param>
    ''' <param name="dev"></param>
    Public Shared Sub Print(output As IEnumerable(Of OligonucleotideCompositionOutput), dev As TextWriter)
        Call tablep
        Call dev.Flush()
    End Sub

End Class