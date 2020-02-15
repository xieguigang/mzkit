Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("mzkit.visual")>
Module Visual

    ''' <summary>
    ''' Plot of the mass spectrum
    ''' </summary>
    ''' <param name="spectrum"></param>
    ''' <param name="alignment"></param>
    ''' <param name="title$"></param>
    ''' <returns></returns>
    <ExportAPI("mass_spectrum.plot")>
    Public Function SpectrumPlot(spectrum As Object, Optional alignment As Object = Nothing, Optional title$ = "Mass Spectrum Plot") As GraphicsData
        If alignment Is Nothing Then
            Return MassSpectra.MirrorPlot(getSpectrum(spectrum), plotTitle:=title)
        Else
            Return MassSpectra.AlignMirrorPlot(getSpectrum(spectrum), getSpectrum(alignment), title:=title)
        End If
    End Function

    Private Function getSpectrum(data As Object) As LibraryMatrix
        Dim type As Type = data.GetType

        Select Case type
            Case GetType(ms2())
                Return New LibraryMatrix With {.ms2 = data, .name = "Mass Spectrum"}
            Case GetType(LibraryMatrix)
                Return data
            Case GetType(MGF.Ions)
                Return DirectCast(data, MGF.Ions).GetLibrary
            Case GetType(dataframe)
                Dim matrix As dataframe = DirectCast(data, dataframe)
                Dim mz As Double() = REnv.asVector(Of Double)(matrix.GetColumnVector("mz"))
                Dim into As Double() = REnv.asVector(Of Double)(matrix.GetColumnVector("into"))
                Dim ms2 As ms2() = mz _
                    .Select(Function(m, i)
                                Return New ms2 With {
                                    .mz = m,
                                    .intensity = into(i),
                                    .quantity = into(i)
                                }
                            End Function) _
                    .ToArray

                Return New LibraryMatrix With {.ms2 = ms2, .name = "Mass Spectrum"}
            Case GetType(PeakMs2)
                Return DirectCast(data, PeakMs2).mzInto
            Case Else
                Throw New NotImplementedException(type.FullName)
        End Select
    End Function
End Module
