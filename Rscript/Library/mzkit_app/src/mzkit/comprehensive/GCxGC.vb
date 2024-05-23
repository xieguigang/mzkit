#Region "Microsoft.VisualBasic::249478e4bc9bd2e341d598c82496a0e1, Rscript\Library\mzkit_app\src\mzkit\comprehensive\GCxGC.vb"

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

    '   Total Lines: 191
    '    Code Lines: 88 (46.07%)
    ' Comment Lines: 86 (45.03%)
    '    - Xml Docs: 91.86%
    ' 
    '   Blank Lines: 17 (8.90%)
    '     File Size: 8.12 KB


    ' Module GCxGC
    ' 
    '     Function: create2DPeaks, Demodulate2D, readCDF, saveCDF, TIC1D
    '               TIC2D
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
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Comprehensive two-dimensional gas chromatography
''' 
''' Processing GCxGC comprehensive chromatogram data: Comprehensive Two-dimensional gas chromatography, 
''' or GC×GC is a multidimensional gas chromatography technique that was originally described in 1984 
''' by J. Calvin Giddings and first successfully implemented in 1991 by Professor Phillips and his 
''' student Zaiyou Liu.
'''
''' GC×GC utilizes two different columns With two different stationary phases. In GC×GC, all Of the 
''' effluent from the first dimension column Is diverted To the second dimension column via a modulator. 
''' The modulator quickly traps, Then "injects" the effluent from the first dimension column onto the second 
''' dimension. This process creates a retention plane Of the 1St dimension separation x 2nd dimension 
''' separation.
'''
''' The Oil And Gas Industry were early adopters Of the technology For the complex oil samples To determine
''' the many different types Of Hydrocarbons And its isomers. Nowadays In these types Of samples it has been 
''' reported that over 30000 different compounds could be identified In a crude oil With this Comprehensive 
''' Chromatography Technology (CCT).
'''
''' The CCT evolved from a technology only used In academic R&amp;D laboratories, into a more robust technology 
''' used In many different industrial labs. Comprehensive Chromatography Is used In forensics, food And flavor, 
''' environmental, metabolomics, biomarkers And clinical applications. Some Of the most well-established 
''' research groups In the world that are found In Australia, Italy, the Netherlands, Canada, United States,
''' And Brazil use this analytical technique.
''' </summary>
<Package("GCxGC")>
Module GCxGC

    ''' <summary>
    ''' Demodulate the 1D TIC to 2D data
    ''' </summary>
    ''' <param name="TIC"></param>
    ''' <param name="modtime">
    ''' The time required to complete a cycle is called the period of modulation (modulation time)
    ''' and is actually the time in between two hot pulses, which typically lasts between 2 and 10 
    ''' seconds is related to the time needed for the compounds to eluted in 2D.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("TIC2D")>
    <RApiReturn(GetType(D2Chromatogram))>
    Public Function TIC2D(TIC As ChromatogramTick(), modtime As Double) As Object
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
    ''' Demodulate the 1D rawdata input as 2D data
    ''' </summary>
    ''' <param name="rawdata"></param>
    ''' <param name="modtime"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <example>
    ''' require(mzkit);
    ''' 
    ''' imports "GCxGC" from "mzkit";
    ''' 
    ''' let rawdata = open.mzpack(file = "/path/to/leco_gcms.cdf");
    ''' let gcxgc = GCxGC::demodulate_2D(rawdata, modtime = 4);
    ''' 
    ''' write.mzPack(gcxgc, file = "/file/to/save/gcxgc_rawdata.mzPack");
    ''' </example>
    <ExportAPI("demodulate_2D")>
    <RApiReturn(GetType(mzPack))>
    Public Function Demodulate2D(rawdata As Object, modtime As Double, Optional env As Environment = Nothing) As Object
        If rawdata Is Nothing Then
            Return Nothing
        End If

        If TypeOf rawdata Is netCDFReader Then
            Return GC2Dimensional.ToMzPack(agilentGC:=rawdata, modtime:=modtime)
        ElseIf TypeOf rawdata Is mzPack Then
            Return DirectCast(rawdata, mzPack).Demodulate2D(modtime)
        Else
            Return Internal.debug.stop("invalid rawdata type!", env)
        End If
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
            extract = ExtractXIC(mz, mzdiff:=test)
        Else
            extract = AddressOf ExtractTIC
        End If

        Return raw.MS _
            .Select(Function(d)
                        Return extract(d)
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' save GCxGC 2D Chromatogram data as a new netcdf file.
    ''' </summary>
    ''' <param name="TIC"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("save.cdf")>
    <RApiReturn(TypeCodes.boolean)>
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
