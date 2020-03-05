Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace MRM

    ''' <summary>
    ''' pre-processing for the rt shift
    ''' </summary>
    Module RTAlignmentProcessor

        <Extension>
        Public Function AcquireRT(samples As IEnumerable(Of String), ions As IonPair(), args As MRMArguments) As RTAlignment()
            Dim tolerance As Tolerance = args.tolerance
            Dim isomerism As IsomerismIonPairs() = IonPair.GetIsomerism(ions, Tolerance).ToArray
            Dim raw = samples _
                .Select(Function(file)
                            Return MRMSamples.ExtractIonData(
                                ion_pairs:=isomerism,
                                mzML:=file,
                                assignName:=Function(i) i.accession,
                                tolerance:=tolerance
                            ).Select(Function(ion)
                                         Return (file.BaseName, ion)
                                     End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(tuple) tuple.ion.name) _
                .ToArray
            Dim rt As RTAlignment() = raw _
                .Select(Function(ion) ion.processingRT(args)) _
                .ToArray

            Return rt
        End Function

        <Extension>
        Private Function processingRT(samples As IEnumerable(Of (sampleName$, raw As IonChromatogramData)), args As MRMArguments) As RTAlignment
            Dim data = samples.ToArray
            Dim ionPair As IsomerismIonPairs = data(Scan0).raw.ion
            Dim samplePeaks = data _
                .Select(Function(sample)
                            Dim ROI = sample.raw.chromatogram _
                                .Shadows _
                                .PopulateROI(
                                    baselineQuantile:=args.baselineQuantile,
                                    angleThreshold:=args.angleThreshold
                                )
                            Dim rt As Double = ROI.getRt(numOfIsomerism:=ionPair.ions.Length)

                            Return New NamedValue(Of Double) With {
                                .Name = sample.sampleName,
                                .Value = rt
                            }
                        End Function) _
                .ToArray

            Return New RTAlignment(ionPair.target, samplePeaks)
        End Function

        <Extension>
        Private Function getRt(ROI As IEnumerable(Of ROI), numOfIsomerism As Integer) As Double
            Dim peak As ROI

            If numOfIsomerism = 0 Then
                peak = ROI _
                    .OrderByDescending(Function(r) r.Integration) _
                    .First
            Else
                peak = ROI _
                    .OrderByDescending(Function(r) r.Integration) _
                    .Take(numOfIsomerism + 1) _
                    .OrderBy(Function(r) r.rt) _
                    .First
            End If

            Return peak.rt
        End Function

    End Module

    Public Class RTAlignment

        Dim samples As NamedValue(Of Double)()
        Dim ion As IonPair

        Sub New(ion As IonPair, sampleValues As NamedValue(Of Double)())
            Me.ion = ion
            Me.samples = sampleValues
        End Sub

        Public Iterator Function CalcRtShifts() As IEnumerable(Of NamedValue(Of Double))

        End Function

        Public Overrides Function ToString() As String
            Return ion.ToString
        End Function
    End Class
End Namespace