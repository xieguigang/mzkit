#Region "Microsoft.VisualBasic::f0eb7ef7e51d602d362f5896d8a241a9, mzkit\src\assembly\mzPackExtensions\VendorStream\VendorStreamLoader.vb"

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

    '   Total Lines: 66
    '    Code Lines: 52
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 2.48 KB


    ' Class VendorStreamLoader
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: StreamTo
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language

Public MustInherit Class VendorStreamLoader(Of T As IMsScanData)

    Protected MS1 As ScanMS1 = Nothing
    Protected MS2 As New List(Of ScanMS2)
    Protected MSscans As New List(Of ScanMS1)
    Protected scanIdFunc As Func(Of T, Integer, String)

    Public MustOverride ReadOnly Property rawFileName As String

    Protected Sub New(scanIdFunc As Func(Of T, Integer, String))
        Me.scanIdFunc = If(scanIdFunc, New Func(Of T, Integer, String)(AddressOf defaultScanId))
    End Sub

    Protected MustOverride Function defaultScanId(scaninfo As T, i As Integer) As String
    Protected MustOverride Function pullAllScans(skipEmptyScan As Boolean) As IEnumerable(Of T)
    Protected MustOverride Sub walkScan(scan As T)

    Public Function StreamTo(Optional skipEmptyScan As Boolean = True,
                             Optional println As Action(Of String) = Nothing) As mzPack

        Dim scan_times As New List(Of Double)
        Dim TIC As New List(Of Double)
        Dim BPC As New List(Of Double)
        Dim i As i32 = 0

        For Each scaninfo As T In pullAllScans(skipEmptyScan)
            Call walkScan(scaninfo)

            If scaninfo.MSLevel = 1 Then
                scan_times += scaninfo.ScanTime * 60
                TIC += scaninfo.TotalIonCurrent
                BPC += scaninfo.BasePeakIntensity

                If println IsNot Nothing AndAlso ++i Mod 7 = 0 Then
                    Call println($"Load " & scaninfo.ToString)
                End If
            End If
        Next

        If Not MS2.IsNullOrEmpty AndAlso Not MS1 Is Nothing Then
            MS1.products = MS2.PopAll
            MSscans += MS1
        End If

        If MSscans = 0 Then
            Call $"No MS scan data in raw data file: {rawFileName}".Warning
        End If

        Return New mzPack With {
            .MS = MSscans.PopAll,
            .Chromatogram = New Chromatogram With {
                .BPC = BPC.PopAll,
                .TIC = TIC.PopAll,
                .scan_time = scan_times.PopAll
            },
            .source = rawFileName.FileName
        }
    End Function
End Class
