Imports System.Xml.Serialization

Namespace SDF.Models

    ''' <summary>
    ''' Connection between atoms
    ''' </summary>
    Public Class Bound

        ''' <summary>
        ''' index of atom 1
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property i As Integer
        ''' <summary>
        ''' index of atom 2
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property j As Integer
        <XmlAttribute> Public Property Type As BoundTypes
        <XmlAttribute> Public Property Stereo As BoundStereos

        Public Overrides Function ToString() As String
            Return $"[{i}, {j}] {Type} AND {Stereo}"
        End Function

        Public Shared Function Parse(line As String) As Bound
            Dim t$() = line.StringSplit("\s+")
            Dim i% = t(0)
            Dim j = t(1)
            Dim type As BoundTypes = CInt(t(2))
            Dim stereo As BoundStereos = CInt(t(3))

            Return New Bound With {
                .i = i,
                .j = j,
                .Type = type,
                .Stereo = stereo
            }
        End Function
    End Class
End Namespace