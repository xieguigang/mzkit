#Region "Microsoft.VisualBasic::efe51e4aa7016f4b1448b87b29491437, assembly\BrukerDataReader\SCiLSLab\ImportsUtils.vb"

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

    '   Total Lines: 102
    '    Code Lines: 65 (63.73%)
    ' Comment Lines: 20 (19.61%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 17 (16.67%)
    '     File Size: 3.85 KB


    '     Module ImportsUtils
    ' 
    '         Function: CheckFileHandler, (+2 Overloads) CheckSpotFiles, getTagName, InternalCheckSpotFiles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace SCiLSLab

    ''' <summary>
    ''' tools handler for import SCiLSLab csv data
    ''' </summary>
    Public Module ImportsUtils

        ''' <summary>
        ''' check csv table type
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="maxLines"></param>
        ''' <returns>
        ''' <see cref="MsPack"/> or <see cref="SpotPack"/>, nothing for invalid table
        ''' </returns>
        Public Function CheckFileHandler(file As String, Optional maxLines As Integer = 64) As Type
            For Each line As String In file.IterateAllLines.Take(maxLines)
                If Not line.StringEmpty Then
                    If Not line.StartsWith("#") Then
                        If line.StartsWith("m/z;") Then
                            Return GetType(MsPack)
                        ElseIf line.StartsWith("") Then
                            Return GetType(SpotPack)
                        End If
                    End If
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' file the filepath of the spot index/ms scan data tuple
        ''' </summary>
        ''' <param name="msDataDir">
        ''' handling multiple spot data export
        ''' </param>
        ''' <returns></returns>
        Public Function CheckSpotFiles(msDataDir As String) As IEnumerable(Of (sportIndex$, msData$))
            Return msDataDir.ListFiles("*.csv").InternalCheckSpotFiles
        End Function

        <Extension>
        Public Function CheckSpotFiles(filelist As IEnumerable(Of String)) As IEnumerable(Of (sportIndex$, msData$))
            Dim pairfiles As New List(Of String)

            For Each path As String In filelist
                Dim dir As String = path.ParentPath
                Dim files As String() = dir.ListFiles("*.csv").ToArray
                Dim tag As String = getTagName(path)

                Call files _
                    .Where(Function(filepath)
                               Return filepath.FileName.StartsWith(tag)
                           End Function) _
                    .DoCall(AddressOf pairfiles.AddRange)
            Next

            Return pairfiles.Distinct.InternalCheckSpotFiles
        End Function

        Private Function getTagName(path As String) As String
            ' TPT_X3-2_spot_sites.csv -> TPT_X3-2
            ' TPT_X3-2_data.csv -> TPT_X3-2
            Dim name As String = path.FileName

            name = name.Replace("_spot_sites.csv", "")
            name = name.Replace("_data.csv", "")

            Return name
        End Function

        <Extension>
        Private Iterator Function InternalCheckSpotFiles(filelist As IEnumerable(Of String)) As IEnumerable(Of (sportIndex$, msData$))
            For Each group In filelist.GroupBy(Function(path) PackFile.GetGuid(path))
                Dim index As String = Nothing
                Dim msData As String = Nothing
                Dim handler As Type = Nothing

                For Each path As String In group
                    handler = CheckFileHandler(path)

                    If handler Is GetType(MsPack) Then
                        msData = path
                    ElseIf handler Is GetType(SpotPack) Then
                        index = path
                    End If
                Next

                If Not (index.StringEmpty OrElse msData.StringEmpty) Then
                    Yield (index, msData)
                Else
                    Call $"missing spot index or ms scan data for {group.Key}".Warning
                End If
            Next
        End Function

    End Module
End Namespace
