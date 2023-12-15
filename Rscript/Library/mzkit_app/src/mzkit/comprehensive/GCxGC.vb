#Region "Microsoft.VisualBasic::d1d79b3c7b489b65e6afae5aeb41b56c, mzkit\Rscript\Library\mzkit\comprehensive\GCxGC.vb"

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

    '   Total Lines: 151
    '    Code Lines: 101
    ' Comment Lines: 33
    '   Blank Lines: 17
    '     File Size: 5.85 KB


    ' Module GCxGC
    ' 
    '     Function: create2DPeaks, extractTIC, extractXIC, readCDF, saveCDF
    '               TIC1D, TIC2D
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Processing GCxGC comprehensive chromatogram data
''' </summary>
<Package("GCxGC")>
Module GCxGC

    <ExportAPI("TIC2D")>
    Public Function TIC2D(TIC As ChromatogramTick(), modtime As Double) As D2Chromatogram()
        Return TIC.Demodulate2D(modtime)
    End Function

    <ExportAPI("TIC1D")>
    Public Function TIC1D(matrix As D2Chromatogram()) As ChromatogramTick()
        Return matrix _
            .Select(Function(i)
                        Return New ChromatogramTick With {
                            .Time = i.scan_time,
                            .Intensity = i.intensity
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' extract GCxGC 2d peaks from the mzpack raw data file
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="mz">
    ''' target mz value for extract XIC data. NA means extract 
    ''' TIC data by default.
    ''' </param>
    ''' <param name="mzdiff">
    ''' the mz tolerance error for match the intensity data for
    ''' extract XIC data if the <paramref name="mz"/> is not 
    ''' ``NA`` value.
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function will extract the TIC data by default.
    ''' </remarks>
    <ExportAPI("extract_2D_peaks")>
    <RApiReturn(GetType(D2Chromatogram))>
    Public Function create2DPeaks(raw As mzPack,
                                  Optional mz As Double = Double.NaN,
                                  Optional mzdiff As Object = "ppm:30",
                                  Optional env As Environment = Nothing) As Object

        Dim extract_XIC As Boolean = Not mz.IsNaNImaginary AndAlso mz > 0
        Dim mzErr = Math.getTolerance(mzdiff, env)

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim test As Tolerance = mzErr.TryCast(Of Tolerance)
        Dim extract As Func(Of ScanMS1, D2Chromatogram)

        If extract_XIC Then
            extract = extractXIC(mz, mzdiff:=test)
        Else
            extract = AddressOf extractTIC
        End If

        Return raw.MS _
            .Select(Function(d)
                        Return extract(d)
                    End Function) _
            .ToArray
    End Function

    Private Function extractXIC(mz As Double, mzdiff As Tolerance) As Func(Of ScanMS1, D2Chromatogram)
        Return Function(d)
                   Return New D2Chromatogram With {
                       .scan_time = d.rt,
                       .intensity = d.GetIntensity(mz, mzdiff),
                       .chromatogram = d.products _
                            .Select(Function(t)
                                        Return New ChromatogramTick With {
                                            .Time = t.rt,
                                            .Intensity = t.GetIntensity(mz, mzdiff)
                                        }
                                    End Function) _
                            .ToArray
                   }
               End Function
    End Function

    Private Function extractTIC(d As ScanMS1) As D2Chromatogram
        Return New D2Chromatogram With {
            .intensity = d.TIC,
            .scan_time = d.rt,
            .chromatogram = d.products _
                .Select(Function(t)
                            Return New ChromatogramTick With {
                                .Intensity = t.into.Sum,
                                .Time = t.rt
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' save GCxGC 2D Chromatogram data as a new netcdf file.
    ''' </summary>
    ''' <param name="TIC"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("save.cdf")>
    Public Function saveCDF(TIC As D2Chromatogram(), <RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim filestream As [Variant](Of Stream, Message) = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If filestream Like GetType(Message) Then
            Return filestream.TryCast(Of Message)
        End If

        Return D2Chromatogram.EncodeCDF(TIC, filestream.TryCast(Of Stream))
    End Function

    ''' <summary>
    ''' read GCxGC 2D Chromatogram data from a given netcdf file.
    ''' </summary>
    ''' <param name="file">
    ''' this function used for parse the cdf file format for both mzkit format or LECO format
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' A data model for GCxGC 2d chromatogram
    ''' </returns>
    <ExportAPI("read.cdf")>
    <RApiReturn(GetType(D2Chromatogram))>
    Public Function readCDF(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim filestream As [Variant](Of Stream, Message) = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If filestream Like GetType(Message) Then
            Return filestream.TryCast(Of Message)
        End If

        Return D2Chromatogram.DecodeCDF(filestream.TryCast(Of Stream)).ToArray
    End Function
End Module
