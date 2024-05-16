#Region "Microsoft.VisualBasic::ae1b00cffea5ae982526b574ae2a8d25, assembly\BrukerDataReader\flexImaging\Storage.vb"

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

    '   Total Lines: 60
    '    Code Lines: 50
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 2.34 KB


    ' Module Storage
    ' 
    '     Function: CreateValueIndex, GetMetaData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.My.JavaScript
Imports any = Microsoft.VisualBasic.Scripting


''' <summary>
''' Storage.mcf_idx
''' </summary>
Public Module Storage

    Public Iterator Function GetMetaData(mcf_file As String) As IEnumerable(Of NamedValue(Of String))
        Dim parser As New IndexParser(mcf_file)
        Dim metadataId = parser.LoadTable("MetadataId").ToArray
        Dim strs = parser.LoadTable("MetaDataString").CreateValueIndex
        Dim ints = parser.LoadTable("MetaDataInt").CreateValueIndex
        Dim dbls = parser.LoadTable("MetaDataDouble").CreateValueIndex
        Dim bytes = parser.LoadTable("MetaDataBlob").CreateValueIndex

        For Each meta As JavaScriptObject In metadataId
            Dim id As String = meta("metadataId").ToString
            Dim name As String = meta("permanentName").ToString
            Dim displayName As String = meta("displayName").ToString
            Dim type As String = "any"
            Dim value As String = ""

            If strs.ContainsKey(id) Then
                type = "chr"
                value = strs(id)
            ElseIf ints.ContainsKey(id) Then
                type = "int"
                value = ints(id)
            ElseIf dbls.ContainsKey(id) Then
                type = "num"
                value = dbls(id)
            ElseIf bytes.ContainsKey(id) Then
                type = "raw"
                value = bytes(id)
            End If

            Yield New NamedValue(Of String) With {
                .Name = name,
                .Value = value,
                .Description = $"{type}|{displayName}"
            }
        Next
    End Function

    <Extension>
    Private Function CreateValueIndex(data As IEnumerable(Of JavaScriptObject)) As Dictionary(Of String, String)
        Return data _
            .GroupBy(Function(a) a("MetaDataId").ToString) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a _
                                  .Select(Function(i) any.ToString(i("Value"))) _
                                  .JoinBy("; ")
                          End Function)
    End Function
End Module
