#Region "Microsoft.VisualBasic::104c5700dbee77a36845167dff13a0a3, assembly\assembly\UnifyReader\Provider.vb"

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

    '   Total Lines: 74
    '    Code Lines: 59 (79.73%)
    ' Comment Lines: 4 (5.41%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (14.86%)
    '     File Size: 3.05 KB


    '     Module Provider
    ' 
    '         Function: GetMsMs, mzMLScanLoader, mzXMLScanLoader
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace DataReader

    Public Module Provider

        Public Function GetMsMs(Of Scan)() As Func(Of Scan, ms2())
            Dim reader As ISpectrumReader(Of Scan) = MsDataReader(Of Scan).ScanProvider
            Dim decoder As Func(Of Scan, ms2()) = AddressOf reader.GetMsMs

            Return decoder
        End Function

        <Extension>
        Public Iterator Function mzMLScanLoader(path As String,
                                                Optional relativeInto As Boolean = False,
                                                Optional onlyMs2 As Boolean = True) As IEnumerable(Of PeakMs2)
            Dim basename$ = path.BaseName

            For Each msscan As spectrum In indexedmzML _
                .LoadScans(path) _
                .Where(Function(s)
                           If Not onlyMs2 Then
                               Return True
                           Else
                               Return s.ms_level = "2"
                           End If
                       End Function)

                Dim msLevel As Integer = msscan.ms_level.DoCall(AddressOf ParseInteger)

                Select Case msLevel
                    Case 1
                        Yield msscan.ScanData(basename, centroid:=False, raw:=True)
                    Case 0
                        ' skip UV data?
                        ' Yield msscan.ScanData(basename, centroid:=False, raw:=True)
                    Case Else
                        ' msn
                        Yield msscan.ScanData(basename, centroid:=False, raw:=True)
                End Select
            Next
        End Function

        <Extension>
        Public Iterator Function mzXMLScanLoader(file As String,
                                                 Optional relativeInto As Boolean = False,
                                                 Optional onlyMs2 As Boolean = True) As IEnumerable(Of PeakMs2)
            Dim basename$ = file.FileName

            For Each ms2Scan As mzXML.scan In mzXML.XML _
                .LoadScans(file) _
                .Where(Function(s)
                           If Not onlyMs2 Then
                               Return True
                           Else
                               Return s.msLevel = 2
                           End If
                       End Function)

                If ms2Scan.msLevel = 1 Then
                    ' ms1的数据总是使用raw intensity值
                    Yield ms2Scan.ScanData(basename, raw:=True)
                Else
                    Yield ms2Scan.ScanData(basename, raw:=Not relativeInto)
                End If
            Next
        End Function
    End Module
End Namespace
