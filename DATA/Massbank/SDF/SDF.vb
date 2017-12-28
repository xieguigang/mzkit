Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
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
End Namespace