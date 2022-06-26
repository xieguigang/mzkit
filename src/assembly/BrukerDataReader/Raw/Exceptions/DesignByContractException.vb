
Imports System

Namespace Raw
    ''' <summary>
    ''' Exception raised when a contract is broken.
    ''' Catch this exception type if you wish to differentiate between
    ''' any DesignByContract exception and other runtime exceptions.
    '''
    ''' </summary>
    <CoverageExclude>
    <Serializable>
    Public Class DesignByContractException
        Inherits ApplicationException

        Protected Sub New()
        End Sub

        Protected Sub New(message As String)
            MyBase.New(message)
        End Sub

        Protected Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class
End Namespace