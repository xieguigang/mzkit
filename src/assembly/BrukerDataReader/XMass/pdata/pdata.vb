#Region "Microsoft.VisualBasic::f48b7a5dd6264f4a5c3b60815e6b67cb, E:/mzkit/src/assembly/BrukerDataReader//XMass/pdata/pdata.vb"

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

    '   Total Lines: 30
    '    Code Lines: 24
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 999 B


    '     Class pdata
    ' 
    '         Properties: id, peaklist, proc, procs
    ' 
    '         Function: LoadFolder, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace XMass

    Public Class pdata

        Public Property peaklist As pklist
        Public Property proc As NamedValue(Of String())()
        Public Property procs As NamedValue(Of String())()
        Public Property id As String

        Public Overrides Function ToString() As String
            Return id
        End Function

        Public Shared Function LoadFolder(dir As String) As pdata
            Dim pklist = $"{dir}/peaklist.xml".LoadXml(Of pklist)
            Dim proc = PropertyFileReader.ReadData($"{dir}/proc".OpenReader).ToArray
            Dim procs = PropertyFileReader.ReadData($"{dir}/procs".OpenReader).ToArray
            Dim id As String = dir.BaseName

            Return New pdata With {
                .peaklist = pklist,
                .proc = proc,
                .procs = procs,
                .id = id
            }
        End Function
    End Class
End Namespace
