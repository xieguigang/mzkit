#Region "Microsoft.VisualBasic::ad519c5d27317e93762953d295bfe2d7, assembly\assembly\mzPack\mzWebCache\Cache.vb"

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

    '   Total Lines: 85
    '    Code Lines: 52 (61.18%)
    ' Comment Lines: 19 (22.35%)
    '    - Xml Docs: 89.47%
    ' 
    '   Blank Lines: 14 (16.47%)
    '     File Size: 3.40 KB


    '     Module Cache
    ' 
    '         Function: (+2 Overloads) Load
    ' 
    '         Sub: Write, WriteTabularCache
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports Microsoft.VisualBasic.Serialization.BinaryDumping

Namespace mzData.mzWebCache

    Public Module Cache

        ''' <summary>
        ''' load scan data from ``mzml`` file
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzErr$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(raw As IEnumerable(Of spectrum), Optional mzErr$ = "da:0.1") As IEnumerable(Of ScanMS1)
            Return New mzMLScans(mzErr).Load(raw)
        End Function

        ''' <summary>
        ''' load scan data from ``mzxml`` file
        ''' </summary>
        ''' <param name="raw"></param>
        ''' <param name="mzErr$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(raw As IEnumerable(Of mzXML.scan), Optional mzErr$ = "da:0.1") As IEnumerable(Of ScanMS1)
            Return New mzXMLScans(mzErr).Load(raw)
        End Function

        ''' <summary>
        ''' write ASCII text format(for BioDeep javascript data reader)
        ''' </summary>
        ''' <param name="scans"></param>
        ''' <param name="file">auto flush to file</param>
        <Extension>
        Public Sub Write(scans As IEnumerable(Of ScanMS1), file As Stream)
            Static network As New NetworkByteOrderBuffer

            Using writer As New StreamWriter(file)
                For Each scan As ScanMS1 In scans
                    Call writer.WriteLine(scan.scan_id)
                    Call writer.WriteLine({scan.rt, scan.BPC, scan.TIC}.JoinBy(","))
                    Call writer.WriteLine(network.Base64String(scan.mz))
                    Call writer.WriteLine(network.Base64String(scan.into))

                    For Each product As ScanMS2 In scan.products
                        Call writer.WriteLine(product.scan_id)
                        Call writer.WriteLine({product.parentMz, product.rt, product.intensity, product.polarity}.JoinBy(","))
                        Call writer.WriteLine(network.Base64String(product.mz))
                        Call writer.WriteLine(network.Base64String(product.into))
                    Next

                    Call writer.WriteLine("-----")
                Next

                Call writer.Flush()
            End Using
        End Sub

        <Extension>
        Public Sub WriteTabularCache(scans As IEnumerable(Of ScanMS1), file As Stream)
            Dim mz, intensity As String
            Dim row As String()

            Static network As New NetworkByteOrderBuffer

            Using writer As New StreamWriter(file)
                For Each scan As ScanMS1 In scans
                    mz = network.Base64String(scan.mz)
                    intensity = network.Base64String(scan.into)
                    row = {scan.rt, scan.BPC, scan.TIC, mz, intensity}

                    Call writer.WriteLine(row.JoinBy(vbTab))
                Next

                Call writer.Flush()
            End Using
        End Sub
    End Module
End Namespace
