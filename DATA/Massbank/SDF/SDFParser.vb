Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.UnixBash

Namespace File

    ''' <summary>
    ''' An internal file parser module
    ''' </summary>
    Module SDFParser

        ''' <summary>
        ''' 解析单个的SDF文件
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Iterator Function IterateParser(path As String) As IEnumerable(Of SDF)
            For Each block As String() In path _
                .IterateAllLines _
                .Split(Function(s) s = "$$$$", includes:=False)

                Yield SDFParser.StreamParser(block)
            Next
        End Function

        Const molEnds$ = "M  END"

        Private Function StreamParser(block$()) As SDF
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

            Dim struct As [Structure] = [Structure].Parse(mol)
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
        Public Function ScanKeys(directory As String) As String()
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

        ''' <summary>
        ''' Scan and parsing all of the ``*.sdf`` model file in the target <paramref name="directory"/>
        ''' </summary>
        ''' <param name="directory$"></param>
        ''' <param name="takes%"></param>
        ''' <param name="echo"></param>
        ''' <returns></returns>
        Public Iterator Function MoleculePopulator(directory$, Optional takes% = -1, Optional echo As Boolean = True) As IEnumerable(Of SDF)
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
    End Module
End Namespace