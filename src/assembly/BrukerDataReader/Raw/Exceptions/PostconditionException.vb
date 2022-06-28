
Imports System

Namespace Raw

    ''' <summary>
    ''' Exception raised when a postcondition fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class PostconditionException
        Inherits DesignByContractException
        ''' <summary>
        ''' Postcondition Exception.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Postcondition Exception.
        ''' </summary>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        ''' Postcondition Exception.
        ''' </summary>
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class
End Namespace