Imports System.Xml.Serialization

Public Structure MSMSPeak

    <XmlAttribute> Public Property mz As Double
    <XmlAttribute> Public Property intensity As Double

    Sub New(mz$, intensity$)
        Me.mz = Val(mz)
        Me.intensity = Val(intensity)
    End Sub

    Sub New(mz#, intensity#)
        Me.mz = mz
        Me.intensity = intensity
    End Sub

    Public Overrides Function ToString() As String
        Return $"{mz} ({intensity})"
    End Function
End Structure
