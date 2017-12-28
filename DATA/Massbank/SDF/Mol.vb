Imports System.Windows.Media.Media3D
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text

Namespace File

    Public Class [Structure]

        Public Property Atoms As Atom()
        Public Property Bounds As Bound()

        ''' <summary>
        ''' Next comes the so-called "counts" line. This line is made up of twelve fixed-length fields 
        ''' 
        ''' + the first eleven are three characters long, 
        ''' + and the last six characters long. 
        ''' 
        ''' The first two fields are the most critical, and give the number of atoms and bonds 
        ''' described in the compound.
        ''' 
        ''' ```
        '''   9  8  0     0  0  0  0  0  0999 V2000
        ''' ```
        ''' 
        ''' So this compound will have 9 atoms And 8 bonds described. Often, hydrogens - especially 
        ''' those attached To elements such As carbon Or oxygen - are left implicit (And will be 
        ''' included based On the available valences) rather than being included In the file.
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Private Shared Function parseCounter(line As String) As (counts As String(), version$)
            Dim version$ = Mid(line, line.Length - 6 + 1).Trim
            Dim t$()

            line = Mid(line, 1, line.Length - 6)
            t = line _
                .Split(parTokens:=3, echo:=False) _
                .Select(Function(b) New String(b).Trim) _
                .ToArray

            Return (t, version)
        End Function

        ''' <summary>
        ''' 从分子结构文本数据之中解析出分子的结构模型数据
        ''' </summary>
        ''' <param name="mol">这个参数同时兼容文本内容或者文本文件的路径</param>
        ''' <returns></returns>
        Public Shared Function Parse(mol As String) As [Structure]
            Dim lines$() = mol _
                .SolveStream _
                .Trim(ASCII.CR, ASCII.LF) _
                .lTokens
            Dim countLine = parseCounter(lines(Scan0))
            Dim [dim] = (
                atoms:=CInt(countLine.counts(0)),
                bounds:=CInt(countLine.counts(1))
            )
            Dim atoms = lines _
                .Skip(1) _
                .Take([dim].atoms) _
                .Select(AddressOf Trim) _
                .Select(AddressOf Atom.Parse) _
                .ToArray
            Dim bounds = lines _
                .Skip(1 + [dim].atoms) _
                .Take([dim].bounds) _
                .Select(AddressOf Trim) _
                .Select(AddressOf Bound.Parse) _
                .ToArray

            Return New [Structure] With {
                .Atoms = atoms,
                .Bounds = bounds
            }
        End Function
    End Class

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