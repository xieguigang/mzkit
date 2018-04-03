Imports System.Windows.Media.Media3D
Imports System.Xml.Serialization

Namespace File

    Public Class Atom

        <XmlAttribute> Public Property Atom As String
        <XmlElement("xyz")>
        Public Property Coordination As Point3D

        Public Overrides Function ToString() As String
            Return $"({Coordination}) {Atom}"
        End Function

        Public Shared Function Parse(line As String) As Atom
            Dim t$() = line.StringSplit("\s+")
            Dim xyz As New Point3D With {
                .X = t(0),
                .Y = t(1),
                .Z = t(2)
            }
            Dim name$ = t(3)

            Return New Atom With {
                .Atom = name,
                .Coordination = xyz
            }
        End Function
    End Class

    Public Class Bound

        <XmlAttribute> Public Property i As Integer
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

    Public Enum BoundTypes As Integer
        ''' <summary>
        ''' 非碳原子的化学键连接可能会存在其他数量的键
        ''' </summary>
        [Other] = 0
        ''' <summary>
        ''' 单键
        ''' </summary>
        [Single] = 1
        ''' <summary>
        ''' 双键
        ''' </summary>
        [Double] = 2
        ''' <summary>
        ''' 三键
        ''' </summary>
        [Triple] = 3
        ''' <summary>
        ''' 四键
        ''' </summary>
        [Aromatic] = 4
    End Enum

    ''' <summary>
    ''' 空间立体结构的类型
    ''' </summary>
    Public Enum BoundStereos As Integer
        NotStereo = 0
        Up = 1
        Down = 6
        Other
    End Enum
End Namespace