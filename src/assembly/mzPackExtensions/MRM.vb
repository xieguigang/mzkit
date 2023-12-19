Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader.ChromatogramReader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports chromatogramRaw = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML.chromatogram

''' <summary>
''' convert mzML MRM data to mzPack
''' </summary>
Public Module MRM

    <Extension>
    Public Function ConvertMzMLFile(mzml As IEnumerable(Of chromatogramRaw),
                                    Optional source As String = Nothing,
                                    Optional size As Integer = -1) As mzPack

        Dim all_ions = mzml.DecodeRaw.ToArray
        Dim pack As New mzPack With {
            .Application = FileApplicationClass.LCMSMS,
            .source = source,
            .Annotations = New Dictionary(Of String, String),
            .Chromatogram = GetIonsChromatogram(
                tic:=all_ions.Where(Function(c) c.name.TextEquals("tic")).First.value,
                bpc:=all_ions.Where(Function(c) c.name.TextEquals("bpc")).First.value
            )
        }
        Dim ions = all_ions _
            .Where(Function(c) Not (c.name.TextEquals("tic") OrElse c.name.TextEquals("bpc"))) _
            .ToDictionary(Function(a) a.name,
                          Function(a)
                              Return Resampler.CreateSampler(
                                  x:=a.value.TimeArray,
                                  y:=a.value.IntensityArray
                              )
                          End Function)
        Dim time_scan As Double() = ions.Values _
            .Select(Function(r) r.enumerateMeasures) _
            .IteratesALL _
            .Range _
            .DoCall(Function(r)
                        If size > 0 Then
                            Return r.AsVector(counts:=size)
                        Else
                            Return r.AsVector(5000)
                        End If
                    End Function)
        Dim scans As New List(Of ScanMS1)
        Dim ion_tags As Double() = ions.Keys _
            .Select(Function(t)
                        Return t.Split("/"c) _
                            .Select(Function(d) CInt(d)) _
                            .JoinBy(".") _
                            .DoCall(AddressOf Double.Parse)
                    End Function) _
            .ToArray
        Dim mrm_tags As String() = ions.Keys.ToArray
        Dim scan_index As i32 = 1

        For i As Integer = 0 To ion_tags.Length - 1
            pack.Annotations(ion_tags(i).ToString("F4")) = mrm_tags(i)
        Next

        For Each rt As Double In time_scan
            Dim ints As Double() = mrm_tags _
                .Select(Function(ti) ions(ti).GetIntensity(rt)) _
                .ToArray

            scans += New ScanMS1 With {
                .rt = rt,
                .BPC = ints.Max,
                .TIC = ints.Sum,
                .into = ints,
                .mz = ion_tags,
                .scan_id = $"[MS1] scan={++scan_index} rt={rt}s; BPC={ .BPC} TIC={ .TIC} basepeak={mrm_tags(which.Max(ints))}"
            }
        Next

        pack.MS = scans.ToArray
        Return pack
    End Function

    <Extension>
    Private Iterator Function DecodeRaw(mzml As IEnumerable(Of chromatogram)) As IEnumerable(Of NamedCollection(Of ChromatogramTick))
        For Each c As chromatogram In mzml
            Dim id As String

            If c.id.TextEquals("TIC") OrElse c.id.TextEquals("BPC") Then
                id = c.id
            Else
                id = $"{c.precursor.GetMz}/{c.product.GetMz}"
            End If

            Yield New NamedCollection(Of ChromatogramTick)(id, c.Ticks)
        Next
    End Function
End Module
