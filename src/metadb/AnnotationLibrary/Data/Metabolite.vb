Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

''' <summary>
''' data model of the metabolite ms reference data
''' </summary>
Public Class Metabolite

    Public ReadOnly Property Id As String
        Get
            Return annotation.ID
        End Get
    End Property

    <MessagePackMember(0)> Public Property annotation As MetaInfo
    <MessagePackMember(1)> Public Property precursors As PrecursorData()
    <MessagePackMember(2)> Public Property spectrums As Spectrum()

End Class

Public Class PrecursorData

    <MessagePackMember(0)> Public Property ion As String
    <MessagePackMember(1)> Public Property charge As Integer
    <MessagePackMember(2)> Public Property rt As Double
    ''' <summary>
    ''' the experiment m/z data
    ''' </summary>
    ''' <returns></returns>
    <MessagePackMember(3)> Public Property mz As Double

End Class

Public Class Spectrum

    <MessagePackMember(0)> Public Property guid As String
    <MessagePackMember(1)> Public Property mz As Double()
    <MessagePackMember(2)> Public Property ionMode As Integer
    <MessagePackMember(3)> Public Property intensity As Double()
    <MessagePackMember(4)> Public Property annotations As String()

End Class