
''' <summary>
''' 一级信息表
''' </summary>
Public Class Peaktable

    ''' <summary>
    ''' 可以是差异代谢物的编号
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String
    Public Property mz As Double
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property rt As Double
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property into As Double
    Public Property intb As Double
    Public Property maxo As Double
    Public Property sn As Double
    Public Property sample As String
    Public Property index As Double
    ''' <summary>
    ''' The scan number
    ''' </summary>
    ''' <returns></returns>
    Public Property scan As Integer
    ''' <summary>
    ''' CID/HCD
    ''' </summary>
    ''' <returns></returns>
    Public Property ionization As String
    Public Property energy As String

    Public Overrides Function ToString() As String
        Return $"{mz}@{rt}#{scan}-{ionization}-{energy}"
    End Function
End Class
