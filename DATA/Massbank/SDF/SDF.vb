Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

''' <summary>
''' #### Structure-data file
''' 
''' + LMSD Structure-data file (http://www.lipidmaps.org/resources/downloads/index.html)
''' + PubChem FTP SDF(ftp://ftp.ncbi.nlm.nih.gov/pubchem/Compound/CURRENT-Full/SDF)
''' </summary>
Public Class SDF : Implements INamedValue

    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property [Class] As String
    Public Property Molecule As String
    Public Property MetaData As Dictionary(Of String, String())

    Public Shared Iterator Function IterateParser(path$) As IEnumerable(Of SDF)
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
        Dim ID$ = block(0), class$ = block(1)
        Dim metas$()
        Dim mol$

        With block _
            .Skip(2) _
            .Split(Function(s) s = molEnds, includes:=False)

            metas = .Last
            mol = .First.Join({molEnds}).JoinBy(vbLf)
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

        Return New SDF With {
            .ID = ID.Trim,
            .Molecule = mol,
            .Class = [class].Trim,
            .MetaData = metaData
        }
    End Function
End Class
