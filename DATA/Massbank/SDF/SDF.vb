Imports System.Windows.Media.Media3D
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text

Namespace File

    ' https://pubchemdocs.ncbi.nlm.nih.gov/data-specification

    ''' <summary>
    ''' #### Structure-data file
    ''' 
    ''' SDF or Structures Data File is a common file format developed by Molecular Design Limited 
    ''' to handle a list of molecular structures with associated properties. 
    ''' The file format has been published (Dalby et al., J. Chem. Inf. Comput. Sci. 1992, 32: 244-255).
    ''' 
    ''' + LMSD Structure-data file (http://www.lipidmaps.org/resources/downloads/index.html)
    ''' + PubChem FTP SDF(ftp://ftp.ncbi.nlm.nih.gov/pubchem/Compound/CURRENT-Full/SDF)
    ''' </summary>
    ''' <remarks>
    ''' https://cactus.nci.nih.gov/SDF_toolkit/
    ''' 
    ''' The SDF Toolkit in Perl 5
    ''' 
    ''' Dalby A, Nourse JG, Hounshell WD, Gushurst Aki, Grier DL, Leland BA, Laufer J. 
    ''' Description of several chemical-structure file formats used by computer-programs developed at Molecular Design Limited. 
    ''' Journal of Chemical Information and Computer Sciences 
    ''' 32:(3) 244-255, May-Jun 1992.
    ''' 
    ''' http://www.nonlinear.com/progenesis/sdf-studio/v0.9/faq/sdf-file-format-guidance.aspx
    ''' https://en.wikipedia.org/wiki/Chemical_table_file#SDF
    ''' </remarks>
    Public Class SDF : Implements INamedValue

        ''' <summary>
        ''' The name of the molecule
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' Details of the software used to generate the compound structure
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Software As String

        <XmlText>
        Public Property Comment As String
        Public Property [Structure] As [Structure]
        Public Property MetaData As Dictionary(Of String, String())

        ''' <summary>
        ''' Scan and parsing all of the ``*.sdf`` model file in the target <paramref name="directory"/>
        ''' </summary>
        ''' <param name="directory$"></param>
        ''' <param name="takes%"></param>
        ''' <param name="echo"></param>
        ''' <returns></returns>
        Public Shared Iterator Function MoleculePopulator(directory$, Optional takes% = -1, Optional echo As Boolean = True) As IEnumerable(Of SDF)
            Dim list = ls - l - r - "*.sdf" <= directory

            If takes > 0 Then
                list = list.Take(takes)
            End If

            For Each path As String In list

                If echo Then
                    Call path.__INFO_ECHO
                End If

                For Each model As SDF In IterateParser(path)
                    Yield model
                Next
            Next
        End Function

        ''' <summary>
        ''' 解析单个的SDF文件
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Shared Iterator Function IterateParser(path As String) As IEnumerable(Of SDF)
            Dim o As SDF

            For Each block As String() In path _
                .IterateAllLines _
                .Split(Function(s) s = "$$$$", includes:=False)

                o = SDF.StreamParser(block)

                Yield o
            Next
        End Function

        Const molEnds$ = "M  END"

        Private Shared Function StreamParser(block$()) As SDF
            Dim ID$ = block(0), program$ = block(1)
            Dim comment$ = block(2)
            Dim metas$()
            Dim mol$

            With block _
                .Skip(2) _
                .Split(Function(s) s = molEnds, includes:=False)

                metas = .Last
                mol = .First _
                    .Join({molEnds}) _
                    .JoinBy(vbLf)
            End With

            Dim metaData As Dictionary(Of String, String()) =
                metas _
                .Split(Function(s) s.StringEmpty, includes:=False) _
                .Where(Function(t) Not t.IsNullOrEmpty) _
                .ToDictionary(Function(t)
                                  Return Mid(t(0), 4, t(0).Length - 4)
                              End Function,
                              Function(t)
                                  Return t _
                                      .Skip(1) _
                                      .ToArray
                              End Function)
            Dim struct As [Structure] = [Structure].Parse(mol)

            Return New SDF With {
                .ID = ID.Trim,
                .[Structure] = struct,
                .Software = program.Trim,
                .Comment = comment.Trim,
                .MetaData = metaData
            }
        End Function

        ''' <summary>
        ''' 这个函数可能在构建csv文件进行数据存储的时候回有用
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <returns></returns>
        Public Shared Function ScanKeys(directory As String) As String()
            Dim keys As New Index(Of String)

            For Each model As SDF In MoleculePopulator(directory, takes:=20)
                For Each key As String In model.MetaData.Keys
                    If keys.IndexOf(key) = -1 Then
                        Call keys.Add(key)
                    End If
                Next
            Next

            Return keys.Objects
        End Function
    End Class

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

        Public Shared Function Parse(mol As String) As [Structure]
            Dim lines$() = mol.Trim(ASCII.CR, ASCII.LF).lTokens
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
        [Other] = 0
        [Single] = 1
        [Double] = 2
        [Triple] = 3
        [Aromatic] = 4
    End Enum

    Public Enum BoundStereos As Integer
        NotStereo = 0
        Up = 1
        Down = 6
        Other
    End Enum
End Namespace