#Region "Microsoft.VisualBasic::573e1f9790a5a6904e20041d23a3e65b, assembly\LoadR.NET5\ReadXcmsRData.vb"

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

    '   Total Lines: 88
    '    Code Lines: 72
    ' Comment Lines: 1
    '   Blank Lines: 15
    '     File Size: 2.98 KB


    ' Class XcmsRData
    ' 
    '     Properties: into, ms2, mz, names, rt
    ' 
    '     Function: GetMsMs, readNumeric, ReadRData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports SMRUCC.Rsharp.RDataSet
Imports SMRUCC.Rsharp.RDataSet.Convertor
Imports SMRUCC.Rsharp.RDataSet.Struct.LinkedList
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class XcmsRData

    Public Property mz As Double()
    Public Property rt As Double()
    Public Property into As Double()
    Public Property ms2 As ms2()()
    Public Property names As String()

    Public Iterator Function GetMsMs() As IEnumerable(Of PeakMs2)
        For i As Integer = 0 To names.Length - 1
            Yield New PeakMs2 With {
                .mz = mz(i),
                .rt = rt(i),
                .intensity = into(i),
                .lib_guid = names(i),
                .scan = names(i),
                .mzInto = ms2(i)
            }
        Next
    End Function

    Private Shared Function readNumeric(root As RObject, node As String) As Double()
        Dim query = root.LinkVisitor(node)
        Dim car As RObject = query.value.CAR
        Dim vec As Double() = RStreamReader.ReadNumbers(car)

        Return vec
    End Function

    Public Shared Function ReadRData(buffer As Stream) As XcmsRData
        ' mz1, rt2, into, ms2
        Dim root = Reader.ParseData(buffer).object
        Dim mz = readNumeric(root, "mz1")
        Dim rt = readNumeric(root, "rt2")
        Dim into = readNumeric(root, "into")
        Dim matrix As New List(Of ms2())
        Dim raw As list = ConvertToR.ToRObject(root.LinkVisitor("ms2"))
        Dim names As New List(Of String)
        Dim msms As ms2()

        raw = raw!ms2

        For Each name As String In raw.getNames
            Dim rawObject As Object = raw(name)

            If TypeOf rawObject Is dataframe Then
                Dim df As dataframe = rawObject
                Dim mz2 As Double() = CLRVector.asNumeric(df!mz)
                Dim into2 As Double() = CLRVector.asNumeric(df!intensity)

                msms = New ms2(mz2.Length - 1) {}

                For i As Integer = 0 To mz2.Length - 1
                    msms(i) = New ms2(mz2(i), into2(i))
                Next
            Else
                Dim vec As Double() = CLRVector.asNumeric(raw(name))
                msms = New ms2(CInt(vec.Length / 2) - 1) {}
                Dim tuple = vec.Split(msms.Length)

                For i As Integer = 0 To msms.Length - 1
                    msms(i) = New ms2 With {
                        .mz = tuple(0)(i),
                        .intensity = tuple(1)(i)
                    }
                Next
            End If

            Call names.Add(name)
            Call matrix.Add(msms)
        Next

        Return New XcmsRData With {
            .mz = mz, .rt = rt, .into = into,
            .ms2 = matrix.ToArray,
            .names = names.ToArray
        }
    End Function

End Class
