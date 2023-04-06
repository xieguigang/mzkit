#Region "Microsoft.VisualBasic::1702047411bf7a39105ea5ce84ab33d4, mzkit\src\assembly\mzPack\MemoryReader.vb"

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

    '   Total Lines: 65
    '    Code Lines: 51
    ' Comment Lines: 3
    '   Blank Lines: 11
    '     File Size: 2.42 KB


    ' Class MemoryReader
    ' 
    '     Properties: EnumerateIndex, rtmax, source
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetMetadata, hasMs2, ReadScan
    ' 
    '     Sub: ReadChromatogramTick
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ComponentModel.Collection

''' <summary>
''' read <see cref="mzPack"/> data in-memory
''' </summary>
Public Class MemoryReader : Implements IMzPackReader

    ReadOnly data As mzPack
    ReadOnly scan_index As Index(Of String)

    Public ReadOnly Property EnumerateIndex As IEnumerable(Of String) Implements IMzPackReader.EnumerateIndex
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return scan_index.Objects
        End Get
    End Property

    Public ReadOnly Property source As String Implements IMzPackReader.source
    Public ReadOnly Property rtmax As Double Implements IMzPackReader.rtmax

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(raw As mzPack)
        data = raw
        rtmax = data.MS.OrderByDescending(Function(s) s.rt).First.rt
        scan_index = data.MS.Select(Function(s) s.scan_id).Indexing
        source = raw.source
    End Sub

    Public Sub ReadChromatogramTick(scanId As String,
                                    <Out> ByRef scan_time As Double,
                                    <Out> ByRef BPC As Double,
                                    <Out> ByRef TIC As Double) Implements IMzPackReader.ReadChromatogramTick

        Dim scan As ScanMS1 = ReadScan(scanId)

        If scan Is Nothing Then
            scan_time = -1
            BPC = -1
            TIC = -1
        Else
            scan_time = scan.rt
            BPC = scan.BPC
            TIC = scan.TIC
        End If
    End Sub

    Public Function ReadScan(scan_id As String, Optional skipProducts As Boolean = False) As ScanMS1 Implements IMzPackReader.ReadScan
        Dim i As Integer = scan_index.IndexOf(scan_id)

        If i < 0 Then
            Return Nothing
        Else
            Return data.MS(i)
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMetadata(id As String) As Dictionary(Of String, String) Implements IMzPackReader.GetMetadata
        Return ReadScan(scan_id:=id)?.meta
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function hasMs2(Optional sampling As Integer = 64) As Boolean Implements IMzPackReader.hasMs2
        Return data.MS.Any(Function(scan1) scan1.products.Any)
    End Function
End Class
