Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Query

    Public Class JaccardSet : Implements INamedValue

        Public Property libname As String Implements INamedValue.Key
        Public Property mz1 As Double
        Public Property ms2 As Double()

        ''' <summary>
        ''' rt equals to ZERO means no rt data
        ''' </summary>
        ''' <returns></returns>
        Public Property rt As Double

        Public Overrides Function ToString() As String
            Return libname
        End Function

    End Class
End Namespace