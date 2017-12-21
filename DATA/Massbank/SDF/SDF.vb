Imports System.Windows.Media.Media3D
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language.UnixBash

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
        Public Property ID As String Implements IKeyedEntity(Of String).Key
        ''' <summary>
        ''' Details of the software used to generate the compound structure
        ''' </summary>
        ''' <returns></returns>
        Public Property Software As String
        Public Property Comment As String
        Public Property [Structure] As [Structure]
        Public Property MetaData As Dictionary(Of String, String())

        Public Shared Iterator Function MoleculePopulator(directory As String) As IEnumerable(Of SDF)
            For Each path As String In ls - l - r - "*.sdf" <= directory
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
    End Class

    Public Class [Structure]

        Public Property Atoms As Atom()
        Public Property Bounds As Bound()

        Public Shared Function Parse(mol As String) As [Structure]
            Dim lines$() = mol.Trim.lTokens.Select(AddressOf Trim).ToArray
            Dim countLine$() = lines(Scan0).StringSplit("\s+")
            Dim [dim] = (atoms:=CInt(countLine(0)), bounds:=CInt(countLine(1)))
            Dim atoms = lines.Skip(1).Take([dim].atoms).Select(AddressOf Atom.Parse).ToArray
            Dim bounds = lines.Skip(1 + [dim].atoms).Select(AddressOf Bound.Parse).ToArray

            Return New [Structure] With {
                .Atoms = atoms,
                .Bounds = bounds
            }
        End Function
    End Class

    Public Class Atom

        Public Property Atom As String
        Public Property Coordination As Point3D

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

        Public Property i As Integer
        Public Property j As Integer
        Public Property Type As BoundTypes
        Public Property Stereo As BoundStereos

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