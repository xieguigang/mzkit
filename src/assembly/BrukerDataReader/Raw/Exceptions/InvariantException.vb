Imports System

Namespace Raw

    ''' <summary>
    ''' Exception raised when an invariant fails.
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class InvariantException
        Inherits DesignByContractException
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New()
        End Sub
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        ''' <summary>
        ''' Invariant Exception.
        ''' </summary>
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class
End Namespace