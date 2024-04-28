#Region "Microsoft.VisualBasic::fa035978c408ad21c516a477ca1b3f9e, E:/mzkit/src/assembly/assembly//mzPack/Binary/IMzPackReader.vb"

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

    '   Total Lines: 32
    '    Code Lines: 15
    ' Comment Lines: 12
    '   Blank Lines: 5
    '     File Size: 1.26 KB


    '     Interface IMzPackReader
    ' 
    '         Properties: EnumerateIndex, rtmax, source
    ' 
    '         Function: GetMetadata, hasMs2, ReadScan
    ' 
    '         Sub: ReadChromatogramTick
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices

Namespace mzData.mzWebCache

    ''' <summary>
    ''' a unify reader interface for <see cref="BinaryStreamReader"/> read 
    ''' data from file or read data from ``mzPack`` in-memory data object.
    ''' </summary>
    Public Interface IMzPackReader

        ''' <summary>
        ''' get all scan ms1 data its scan id collection
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property EnumerateIndex As IEnumerable(Of String)
        ''' <summary>
        ''' the source file name of current raw data file
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property source As String
        ReadOnly Property rtmax As Double
        Function ReadScan(scan_id As String, Optional skipProducts As Boolean = False) As ScanMS1
        Function GetMetadata(id As String) As Dictionary(Of String, String)

        Sub ReadChromatogramTick(scanId As String,
                                 <Out> ByRef scan_time As Double,
                                 <Out> ByRef BPC As Double,
                                 <Out> ByRef TIC As Double)
        Function hasMs2(Optional sampling As Integer = 64) As Boolean

    End Interface
End Namespace
