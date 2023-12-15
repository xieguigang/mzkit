Public Class OligonucleotideCompositionOutput

    ''' <summary>
    ''' 1
    ''' </summary>
    Public ObservedMass As Double
    ''' <summary>
    ''' 2
    ''' </summary>
    Public TheoreticalMass As Double
    ''' <summary>
    ''' 3
    ''' </summary>
    Public Errorppm As Double
    ''' <summary>
    ''' 4
    ''' </summary>
    Public OfpA As Integer
    ''' <summary>
    ''' 5
    ''' </summary>
    Public OfpG As Integer
    ''' <summary>
    ''' 6
    ''' </summary>
    Public OfpC As Integer
    ''' <summary>
    ''' 7
    ''' </summary>
    Public OfpV As Integer
    ''' <summary>
    ''' 8
    ''' </summary>
    Public Modification As String
    ''' <summary>
    ''' 10
    ''' </summary>
    Public OfBases As Integer

    Public Sub SetBaseNumber(i As Integer, n As Integer)
        Select Case i
            Case 1 : OfpA = n
            Case 2 : OfpG = n
            Case 3 : OfpC = n
            Case 4 : OfpV = n

            Case Else
                Throw New OutOfMemoryException(i)
        End Select
    End Sub

End Class