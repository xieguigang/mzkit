#Region "Microsoft.VisualBasic::c99c9e6a11dedb2fbda551877be27bda, mzmath\ms2_math-core\Ms1\CacheData\Ms1ScatterCache.vb"

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

    '   Total Lines: 48
    '    Code Lines: 38 (79.17%)
    ' Comment Lines: 3 (6.25%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (14.58%)
    '     File Size: 1.85 KB


    ' Module Ms1ScatterCache
    ' 
    '     Function: LoadDataFrame
    ' 
    '     Sub: (+2 Overloads) SaveDataFrame
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.BinaryDumping

''' <summary>
''' A binary data cache helper of the <see cref="ms1_scan"/> data point collection.
''' </summary>
Public Module Ms1ScatterCache

    ReadOnly network As New NetworkByteOrderBuffer

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub SaveDataFrame(scatter As Enumeration(Of ms1_scan), file As Stream)
        Call scatter.AsEnumerable.SaveDataFrame(file)
    End Sub

    <Extension>
    Public Sub SaveDataFrame(scatter As IEnumerable(Of ms1_scan), file As Stream)
        Dim pool As ms1_scan() = scatter.ToArray
        Dim s As New BinaryWriter(file)
        Dim mz As Double() = pool.Select(Function(i) i.mz).ToArray
        Dim rt As Double() = pool.Select(Function(i) i.scan_time).ToArray
        Dim into As Double() = pool.Select(Function(i) i.intensity).ToArray

        Call s.Write(pool.Length)
        Call s.Write(network.GetBytes(mz))
        Call s.Write(network.GetBytes(rt))
        Call s.Write(network.GetBytes(into))
        Call s.Flush()
    End Sub

    <Extension>
    Public Iterator Function LoadDataFrame(file As Stream) As IEnumerable(Of ms1_scan)
        Dim s As New BinaryReader(file)
        Dim size As Integer = s.ReadInt32
        Dim buffer_size As Integer = size * HeapSizeOf.double
        Dim mz = network.ParseDouble(s.ReadBytes(buffer_size))
        Dim rt = network.ParseDouble(s.ReadBytes(buffer_size))
        Dim into = network.ParseDouble(s.ReadBytes(buffer_size))

        For i As Integer = 0 To size - 1
            Yield New ms1_scan(mz(i), rt(i), into(i))
        Next
    End Function
End Module
