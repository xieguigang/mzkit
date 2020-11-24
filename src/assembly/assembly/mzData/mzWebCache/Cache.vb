Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http

Namespace mzData.mzWebCache

    Public Module Cache

        Public Iterator Function Load(raw As IEnumerable(Of scan), Optional mzErr$ = "da:0.1") As IEnumerable(Of ScanMS1)
            Dim ms1 As ScanMS1 = Nothing
            Dim ms2 As New List(Of ScanMS2)
            Dim scan_time As Double
            Dim msms As ms2()
            Dim trim As LowAbundanceTrimming = New RelativeIntensityCutoff(0.03)
            Dim ms1Err As Tolerance = Tolerance.ParseScript(mzErr)
            Dim scan_id As String

            For Each scan As scan In raw
                scan_time = PeakMs2.RtInSecond(scan.retentionTime)
                scan_id = scan.getName
                msms = scan.peaks _
                    .ExtractMzI _
                    .Where(Function(p) p.intensity > 0) _
                    .Select(Function(p)
                                Return New ms2 With {
                                    .mz = p.mz,
                                    .quantity = p.intensity,
                                    .intensity = p.intensity
                                }
                            End Function) _
                    .ToArray _
                    .Centroid(ms1Err, trim) _
                    .ToArray

                If scan.msLevel = 1 Then
                    If Not ms1 Is Nothing Then
                        ms1.products = ms2.ToArray
                        ms2.Clear()

                        Yield ms1
                    End If

                    ms1 = New ScanMS1 With {
                        .BPC = scan.basePeakIntensity,
                        .TIC = scan.totIonCurrent,
                        .rt = scan_time,
                        .scan_id = scan_id,
                        .mz = msms.Select(Function(a) a.mz).ToArray,
                        .into = msms.Select(Function(a) a.intensity).ToArray
                    }
                Else
                    Call New ScanMS2 With {
                        .rt = scan_time,
                        .parentMz = scan.precursorMz.value,
                        .scan_id = scan_id,
                        .intensity = scan.basePeakIntensity,
                        .mz = msms.Select(Function(a) a.mz).ToArray,
                        .into = msms.Select(Function(a) a.intensity).ToArray
                    }.DoCall(AddressOf ms2.Add)
                End If
            Next

            ms1.products = ms2.ToArray
            ms2.Clear()

            Yield ms1
        End Function

        <Extension>
        Public Sub Write(scans As IEnumerable(Of ScanMS1), file As Stream)
            Using writer As New StreamWriter(file)
                For Each scan As ScanMS1 In scans
                    Call writer.WriteLine(scan.scan_id)
                    Call writer.WriteLine({scan.rt, scan.BPC, scan.TIC}.JoinBy(","))
                    Call writer.WriteLine(scan.mz.vectorBase64)
                    Call writer.WriteLine(scan.into.vectorBase64)

                    For Each product As ScanMS2 In scan.products
                        Call writer.WriteLine(product.scan_id)
                        Call writer.WriteLine({product.parentMz, product.rt, product.intensity}.JoinBy(","))
                        Call writer.WriteLine(product.mz.vectorBase64)
                        Call writer.WriteLine(product.into.vectorBase64)
                    Next

                    Call writer.WriteLine("-----")
                Next

                Call writer.Flush()
            End Using
        End Sub

        <Extension>
        Private Function vectorBase64(vec As Double()) As String
            Using buffer As New MemoryStream
                Dim data As Byte()

                For Each x As Double In vec
                    data = BitConverter.GetBytes(x)
                    buffer.Write(data, Scan0, data.Length)
                Next

                Return buffer.ToArray.ToBase64String
            End Using
        End Function
    End Module
End Namespace
