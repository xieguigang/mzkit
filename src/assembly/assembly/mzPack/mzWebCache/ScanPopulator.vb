#Region "Microsoft.VisualBasic::2e4834a83ed615b4936bef0454e7eebd, src\assembly\assembly\mzPack\mzWebCache\ScanPopulator.vb"

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

    '     Interface IScanReader
    ' 
    '         Function: CreateScan
    ' 
    '     Class ScanPopulator
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: CreateScan, CreateScanGeneral, (+2 Overloads) Load, PopulateValidScans
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace mzData.mzWebCache

    Public Interface IScanReader

        Function CreateScan(scan As Object, Optional uniqueId As String = Nothing) As MSScan

    End Interface

    ''' <summary>
    ''' helper module for read a raw data file and populate 
    ''' a unify data scan model object
    ''' </summary>
    ''' <typeparam name="Scan"></typeparam>
    Public MustInherit Class ScanPopulator(Of Scan) : Implements IScanReader

        Protected ms1 As ScanMS1
        Protected products As New List(Of ScanMS2)
        Protected trim As LowAbundanceTrimming
        Protected ms1Err As Tolerance

        Protected ReadOnly reader As MsDataReader(Of Scan)
        Protected ReadOnly invalidScans As New List(Of Scan)

        Sub New(mzErr As String, intocutoff As Double)
            ms1Err = Tolerance.ParseScript(mzErr)
            reader = dataReader()
            trim = New RelativeIntensityCutoff(intocutoff)
        End Sub

        Protected MustOverride Function dataReader() As MsDataReader(Of Scan)
        Protected MustOverride Function loadScans(rawfile As String) As IEnumerable(Of Scan)

        Private Iterator Function PopulateValidScans(scans As IEnumerable(Of Scan)) As IEnumerable(Of Scan)
            For Each scan As Scan In scans
                If reader.IsEmpty(scan) Then
                    Call $"missing scan value of [{reader.GetScanId(scan)}]".Warning
                    Call invalidScans.Add(scan)
                Else
                    Yield scan
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function CreateScanGeneral(obj As Object, Optional uniqueId As String = Nothing) As MSScan Implements IScanReader.CreateScan
            Return CreateScan(scan:=DirectCast(obj, Scan), uniqueId)
        End Function

        Public Function CreateScan(scan As Scan, Optional uniqueId As String = Nothing) As MSScan
            Dim scan_time As Double = reader.GetScanTime(scan)
            Dim scan_id As String = If(uniqueId.StringEmpty, reader.GetScanId(scan), $"[{uniqueId}]{reader.GetScanId(scan)}")
            Dim msms As ms2() = reader.GetMsMs(scan).Centroid(ms1Err, trim).ToArray

            If reader.GetMsLevel(scan) = 1 Then
                Return New ScanMS1 With {
                    .BPC = reader.GetBPC(scan),
                    .TIC = reader.GetTIC(scan),
                    .rt = scan_time,
                    .scan_id = scan_id,
                    .mz = msms.Select(Function(a) a.mz).ToArray,
                    .into = msms.Select(Function(a) a.intensity).ToArray
                }
            Else
                Return New ScanMS2 With {
                    .rt = scan_time,
                    .parentMz = reader.GetParentMz(scan),
                    .scan_id = scan_id,
                    .intensity = reader.GetBPC(scan),
                    .mz = msms.Select(Function(a) a.mz).ToArray,
                    .into = msms.Select(Function(a) a.intensity).ToArray,
                    .polarity = PrecursorType.ParseIonMode(reader.GetPolarity(scan), True),
                    .activationMethod = reader.GetActivationMethod(scan),
                    .centroided = reader.GetCentroided(scan),
                    .charge = reader.GetCharge(scan),
                    .collisionEnergy = reader.GetCollisionEnergy(scan)
                }
            End If
        End Function

        Public Iterator Function Load(scans As IEnumerable(Of Scan), Optional progress As Action(Of String) = Nothing) As IEnumerable(Of ScanMS1)
            Dim i As i32 = 1

            For Each scan As Scan In PopulateValidScans(scans)
                Dim scanVal As MSScan = CreateScan(scan, ++i)

                If reader.GetMsLevel(scan) = 1 Then
                    If Not ms1 Is Nothing Then
                        ms1.products = products.ToArray
                        products.Clear()

                        Yield ms1
                    End If

                    ms1 = scanVal
                Else
                    Call products.Add(scanVal)
                End If

                If Not progress Is Nothing Then
                    Call progress(scanVal.scan_id)
                End If
            Next

            If Not ms1 Is Nothing Then
                ms1.products = products.ToArray
                products.Clear()

                Yield ms1
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(rawfile As String, Optional progress As Action(Of String) = Nothing) As IEnumerable(Of ScanMS1)
            Return Load(loadScans(rawfile), progress)
        End Function
    End Class
End Namespace
