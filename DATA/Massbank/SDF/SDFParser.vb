#Region "Microsoft.VisualBasic::f980bfebaaec2933fc89b8a3a46aabbf, Massbank\SDF\SDFParser.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:

'     Module SDFParser
' 
'         Function: IterateParser, MoleculePopulator, ScanKeys, StreamParser
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
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
        Public Iterator Function IterateParser(path$, Optional parseStruct As Boolean = True) As IEnumerable(Of SDF)
            Dim offset%

            For Each block As String() In path _
                .IterateAllLines _
                .Split(Function(s) s = "$$$$", includes:=False)

                offset = block.solveOffset()
                block = block _
                    .Skip(offset) _
                    .ToArray

                Yield SDFParser.StreamParser(block, parseStruct)
            Next
        End Function

        Const MolEndMarks$ = "M  END"

        <Extension>
        Private Iterator Function SplitMolData(block As String()) As IEnumerable(Of String())
            For i As Integer = 3 To block.Length - 1
                If block(i) = MolEndMarks OrElse (InStr(block(i), ">") = 1) Then
                    Dim mol$() = block.Skip(3).Take(i - 3).ToArray

                    ' 抛出分子结构数据部分的文本数据行
                    If block(i) <> MolEndMarks Then
                        Yield mol.Join(MolEndMarks).ToArray
                    Else
                        Yield mol
                    End If

                    ' 抛出分子物质注释信息部分的文本行数据
                    Yield block.Skip(i).ToArray

                    Exit For
                End If
            Next
        End Function

        Const MolStartFlag$ = "((\d+)|(\s+))+V2000\s*"

        ''' <summary>
        ''' 假设Program名称的行总是不是空的
        ''' </summary>
        ''' <param name="block"></param>
        ''' <returns></returns>
        <Extension>
        Private Function solveOffset(block As String()) As Integer
            For i As Integer = 0 To block.Length - 1
                If block(i).IsPattern(MolStartFlag, RegexOptions.Singleline) Then
                    Return i - 3
                End If
            Next

            Throw New BadImageFormatException
        End Function

        Private Function StreamParser(block$(), parseStruct As Boolean) As SDF
            Dim ID$ = block(0), program$ = block(1)
            Dim comment$ = block(2)
            Dim metas$()
            Dim mol$()

            With block.SplitMolData
                mol = .First
                metas = .Last
            End With

            Dim struct As [Structure] = Nothing
            Dim metaData As Dictionary(Of String, String()) = metas _
                .Split(Function(s) s.StringEmpty, includes:=False) _
                .Where(Function(t) Not t.IsNullOrEmpty) _
                .ToDictionary(Function(t)
                                  Return Mid(t(0), 4, t(0).Length - 4)
                              End Function,
                              Function(t)
                                  Return t.Skip(1).ToArray
                              End Function)

            If parseStruct Then
                struct = [Structure].ParseStream(mol)
            End If

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
        Public Iterator Function MoleculePopulator(directory$,
                                                   Optional takes% = -1,
                                                   Optional echo As Boolean = True,
                                                   Optional parseStruct As Boolean = True) As IEnumerable(Of SDF)
            Dim list = ls - l - r - "*.sdf" <= directory

            If takes > 0 Then
                list = list.Take(takes)
            End If

            For Each path As String In list
                If echo Then
                    Call path.__INFO_ECHO
                End If

                For Each model As SDF In SDFParser.IterateParser(path, parseStruct)
                    Yield model
                Next
            Next
        End Function
    End Module
End Namespace
