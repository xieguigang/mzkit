Imports System.Xml.Serialization

Public Structure MSMSPeak

    <XmlAttribute> Public Property mz As Double
    <XmlAttribute> Public Property intensity As Double
    <XmlAttribute> Public Property comment As String

    Sub New(mz$, intensity$, Optional comment$ = Nothing)
        Me.mz = Val(mz)
        Me.intensity = Val(intensity)
        Me.comment = comment
    End Sub

    Sub New(mz#, intensity#)
        Me.mz = mz
        Me.intensity = intensity
    End Sub

    Public Overrides Function ToString() As String
        If comment.StringEmpty Then
            Return $"{mz} ({intensity})"
        Else
            Return $"{mz} ({intensity})  #{comment}"
        End If
    End Function
End Structure
