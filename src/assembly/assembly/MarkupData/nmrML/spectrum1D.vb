Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Math

Namespace MarkupData.nmrML

    Public Class spectrum1D

        Public Property spectrumDataArray As fidData
        Public Property xAxis As xAxis

        Public Function ParseMatrix(SW As Double) As LibraryMatrix
            Dim spec1r As Double() = spectrumDataArray.DecodeBytes
            Dim ppm As Double() = xAxis.GetPPM(spec1r.Length, SW)

            Call Array.Reverse(ppm)

            Return New LibraryMatrix With {
                .ms2 = ppm _
                    .Select(Function(p, i)
                                Return New ms2 With {
                                    .mz = p,
                                    .intensity = spec1r(i)
                                }
                            End Function) _
                    .ToArray
            }
        End Function

    End Class

    Public Class xAxis

        <XmlAttribute> Public Property unitAccession As String
        <XmlAttribute> Public Property unitName As String
        <XmlAttribute> Public Property unitCvRef As String
        <XmlAttribute> Public Property startValue As Double
        <XmlAttribute> Public Property endValue As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SI">size of the spectrum</param>
        ''' <returns></returns>
        Public Function GetPPM(SI As Integer, SW As Double) As Double()
            Dim dppm As Double = SW / (SI - 1)
            Dim ppm_max = startValue
            Dim ppm_min = endValue
            Dim ppm As Double() = seq(from:=ppm_min, to:=ppm_max, by:=dppm).ToArray

            Return ppm
        End Function
    End Class

    Public Class spectrumList

        <XmlElement> Public Property spectrum1D As spectrum1D()

    End Class
End Namespace