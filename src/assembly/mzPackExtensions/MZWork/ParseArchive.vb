#Region "Microsoft.VisualBasic::4f4a1bf4d1226c7ab11cf25a8e1bf2b9, E:/mzkit/src/assembly/mzPackExtensions//MZWork/ParseArchive.vb"

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


    ' Code Statistics:

    '   Total Lines: 40
    '    Code Lines: 26
    ' Comment Lines: 8
    '   Blank Lines: 6
    '     File Size: 1.48 KB


    '     Module ParseArchive
    ' 
    '         Function: getTempref, LoadRawGroups
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace MZWork

    Public Module ParseArchive

        ''' <summary>
        ''' export and save mzpack cache files to system tempdir
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="msg"></param>
        ''' <returns>
        ''' returns the metadata collection
        ''' </returns>
        <Extension>
        Public Iterator Function LoadRawGroups(zip As ZipArchive, msg As Action(Of String)) As IEnumerable(Of NamedCollection(Of Raw))
            Dim access As New WorkspaceAccess(zip, msg)
            Dim contents As New List(Of NamedValue(Of Raw))

            For Each block In access.EnumerateBlocks
                Call contents.AddRange(access.ReleaseCache(block))
            Next

            For Each metaGroup In contents.GroupBy(Function(meta) meta.Name)
                Yield New NamedCollection(Of Raw) With {
                    .name = metaGroup.Key,
                    .value = metaGroup _
                        .Select(Function(i) i.Value) _
                        .ToArray
                }
            Next
        End Function

        Friend Function getTempref(meta As Raw) As String
            Return $"{App.AppSystemTemp}/.cache/{meta.cache.Substring(0, 2)}/{meta.cache}.mzPack"
        End Function
    End Module
End Namespace
