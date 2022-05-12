Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

''' <summary>
''' the database index
''' </summary>
Public Class MassIndex

    ''' <summary>
    ''' mz value for the metabolites
    ''' </summary>
    ''' <returns>round to 4 digits</returns>
    <MessagePackMember(0)> Public Property mz As Double
    ''' <summary>
    ''' a reference id list to read metabolite data
    ''' </summary>
    ''' <returns></returns>
    <MessagePackMember(1)> Public Property referenceIds As String()

    Public Overrides Function ToString() As String
        Return mz.ToString("F4")
    End Function

End Class
