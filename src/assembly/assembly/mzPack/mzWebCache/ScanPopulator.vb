#Region "Microsoft.VisualBasic::3808f53b0ca7c03e052abbb39d32e75a, assembly\assembly\mzPack\mzWebCache\ScanPopulator.vb"

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

'   Total Lines: 186
'    Code Lines: 140 (75.27%)
' Comment Lines: 17 (9.14%)
'    - Xml Docs: 88.24%
' 
'   Blank Lines: 29 (15.59%)
'     File Size: 7.83 KB


'     Interface IScanReader
' 
'         Function: CreateScan
' 
'     Class ScanPopulator
' 
'         Properties: verbose
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: CreateScan, CreateScanGeneral, (+2 Overloads) Load, PopulateValidScans, yieldFakeMs1
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
Imports std = System.Math

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

        ''' <summary>
        ''' cache of the current MSn products list, this cache pool will be clear when move to next MS1 scan data.
        ''' </summary>
        Protected products As New List(Of ScanMS2)
        Protected trim As LowAbundanceTrimming
        Protected ms1Err As Tolerance
        Protected rawName As String

        Protected ReadOnly reader As MsDataReader(Of Scan)
        Protected ReadOnly invalidScans As New List(Of Scan)

        Public Property verbose As Boolean = False

        Sub New(mzErr As String, intocutoff As Double)
            ms1Err = Tolerance.ParseScript(mzErr)
            reader = dataReader()
            trim = New RelativeIntensityCutoff(intocutoff)
        End Sub

        Protected MustOverride Function msManufacturer(rawfile As String) As String
        Protected MustOverride Function dataReader() As MsDataReader(Of Scan)
        Protected MustOverride Function loadScans(rawfile As String) As IEnumerable(Of Scan)

        ''' <summary>
        ''' populate of the valids ms scans data
        ''' </summary>
        ''' <param name="scans"></param>
        ''' <returns></returns>
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
            Dim scan_name As String = reader.GetScanId(scan)

            If scan_name.StringEmpty Then
                scan_name = $"[MS1] {rawName}"
            End If

            Dim scan_id As String = If(uniqueId.StringEmpty, scan_name, $"[{uniqueId}]{scan_name}")
            Dim msms As ms2() = reader.GetMsMs(scan).Centroid(ms1Err, trim).ToArray
            Dim msLevel As Integer = reader.GetMsLevel(scan)

            If msLevel = 1 OrElse msLevel = 0 Then
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="scans">load scans data list with unknown data length</param>
        ''' <param name="progress"></param>
        ''' <returns></returns>
        Public Iterator Function Load(scans As IEnumerable(Of Scan), Optional progress As Action(Of String) = Nothing) As IEnumerable(Of ScanMS1)
            Dim i As i32 = 1
            Dim ms1Yields As Integer = 0
            Dim lastProduct As New Dictionary(Of String, ScanMS2)

            For Each scan As Scan In PopulateValidScans(scans)
                Dim scanVal As MSScan = CreateScan(scan, ++i)
                Dim isMs1 As Boolean = TypeOf scanVal Is ScanMS1

                If isMs1 Then
                    If Not ms1 Is Nothing Then
                        ms1.products = products.ToArray
                        ms1Yields += 1
                        products.Clear()

                        Yield ms1
                    End If

                    ms1 = scanVal
                Else
                    ' 20250401 the scan model is the mzkit mzpack data model
                    ' scan id is not the raw id string that parsed from the
                    ' xml file
                    ' MS level prefix has been added into this scan id data
                    ' MS1 - for mslevel = 1
                    ' MS/MS - for mslevel = 2
                    ' MSx - for mslevel > 2
                    Dim isMS2 As Boolean = InStr(scanVal.scan_id, "MS/MS") > 0
                    Dim parent_id As String = reader.GetParentScanNumber(scan)

                    If parent_id Is Nothing AndAlso products.Any AndAlso Not isMS2 Then
                        ' 20250401
                        ' is MSn scan but missing parent scan id,
                        ' this happends in mzML rawdata file, but
                        ' the mzXML file does not.
                        ' needs fix this problem for mzML file.
                        Dim parentMz As Double = DirectCast(scanVal, ScanMS2).parentMz
                        Dim check As ScanMS2 = products _
                            .AsParallel _
                            .Where(Function(ms2)
                                       If ms2.mz Is Nothing Then
                                           Return False
                                       Else
                                           For Each mzi As Double In ms2.mz
                                               If std.Abs(mzi - parentMz) < 0.1 Then
                                                   Return True
                                               End If
                                           Next

                                           Return False
                                       End If
                                   End Function) _
                            .OrderByDescending(Function(a) a.rt) _
                            .FirstOrDefault

                        If Not check Is Nothing Then
                            parent_id = check.scan_id
                        Else
                            parent_id = products.Last.scan_id
                        End If
                    End If

                    If isMS2 OrElse parent_id Is Nothing Then
                        Call products.Add(scanVal)
                    ElseIf lastProduct.ContainsKey(parent_id) Then
                        lastProduct(parent_id).product = scanVal
                    Else
                        Call products.Add(scanVal)
                        Call $"missing precursor scan of number({parent_id}) for {scanVal.scan_id}".Warning
                    End If

                    ' add current product scan to the hash index
                    ' for build next tree node
                    Call lastProduct.Add(reader.GetScanNumber(scan), scanVal)
                    Call lastProduct.Add(scanVal.scan_id, scanVal)
                End If

                ' adjust to 17 for make progress less verbose
                If isMs1 AndAlso progress IsNot Nothing AndAlso CInt(i) Mod 111 = 0 Then
                    Call progress(scanVal.scan_id)
                End If
            Next

            If Not ms1 Is Nothing Then
                ms1.products = products.ToArray
                products.Clear()

                Yield ms1
            ElseIf ms1Yields = 0 Then
                For Each fake As ScanMS1 In yieldFakeMs1(products)
                    Yield fake
                Next
            End If

            If progress IsNot Nothing Then
                Call progress("* read finished!")
            End If
        End Function

        Private Iterator Function yieldFakeMs1(products As IEnumerable(Of ScanMS2)) As IEnumerable(Of ScanMS1)
            For Each ms2 As ScanMS2 In products
                Yield New ScanMS1 With {
                    .BPC = ms2.intensity,
                    .into = {ms2.intensity},
                    .products = {ms2},
                    .mz = {ms2.parentMz},
                    .rt = ms2.rt,
                    .scan_id = "[MS1]" & ms2.scan_id,
                    .TIC = ms2.intensity
                }
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Load(rawfile As String, Optional progress As Action(Of String) = Nothing) As IEnumerable(Of ScanMS1)
            Return Load(loadScans(rawfile), progress)
        End Function
    End Class
End Namespace
