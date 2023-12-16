
''' <summary>
''' digest
''' </summary>
Public Class TheoreticalDigestMass

    ' outputwrite(0, 0) = "Sequence"
    ' outputwrite(0, 1) = "Start"
    ' outputwrite(0, 2) = "End"
    ' outputwrite(0, 3) = "Length"
    ' outputwrite(0, 4) = "5' End"
    ' outputwrite(0, 5) = "3' End"
    ' outputwrite(0, 6) = "Theoretical Mass"
    ' outputwrite(0, 7) = "Name"

    ''' <summary>
    ''' 1
    ''' </summary>
    Public Sequence As String
    ''' <summary>
    ''' 2
    ''' </summary>
    Public Start As Integer
    ''' <summary>
    ''' 3
    ''' </summary>
    Public Ends As Integer
    ''' <summary>
    ''' 4
    ''' </summary>
    Public Length As Integer
    ''' <summary>
    ''' 5
    ''' </summary>
    Public End5 As String
    ''' <summary>
    ''' 6
    ''' </summary>
    Public End3 As String
    ''' <summary>
    ''' 7
    ''' </summary>
    Public TheoreticalMass As Double
    ''' <summary>
    ''' 8
    ''' </summary>
    Public Name As String

    Default Public ReadOnly Property Item(i As Integer) As Object
        Get
            Select Case i
                Case 1 : Return Sequence
                Case 2 : Return Start
                Case 3 : Return Ends
                Case 4 : Return Length
                Case 5 : Return End5
                Case 6 : Return End3
                Case 7 : Return TheoreticalMass
                Case 8 : Return Name

                Case Else
                    Throw New NotImplementedException(i)
            End Select
        End Get
    End Property

End Class
