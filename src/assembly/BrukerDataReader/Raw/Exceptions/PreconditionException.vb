
Imports System

Namespace Raw


    ''' <summary>
    ''' Exception raised when a precondition fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class PreconditionException
        Inherits DesignByContractException
        ''' <summary>
        ''' Precondition Exception.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Precondition Exception.
        ''' </summary>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        ''' Precondition Exception.
        ''' </summary>
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class
End Namespace