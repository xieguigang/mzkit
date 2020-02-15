#Region "Microsoft.VisualBasic::5a2a2f146594d35f7caba2fc0c32ffc4, Rscript\Library\mzkit\Assembly.vb"

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

' Module Assembly
' 
'     Function: ReadMgfIons
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports mzXMLAssembly = BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' The mass spectrum assembly file read/write library module.
''' </summary>
<Package("mzkit.assembly", Category:=APICategories.UtilityTools)>
Module Assembly

    <ExportAPI("read.mgf")>
    Public Function ReadMgfIons(file As String) As Ions()
        Return MgfReader.StreamParser(path:=file).ToArray
    End Function

    <ExportAPI("mgf.ion_peaks")>
    Public Function IonPeaks(ions As Object, Optional env As Environment = Nothing) As Object
        Dim inputType As Type = ions.GetType

        Select Case inputType
            Case GetType(Ions)
                Return {DirectCast(ions, Ions)}.IonPeaks.ToArray
            Case GetType(Ions())
                Return DirectCast(ions, Ions()).IonPeaks.ToArray
            Case GetType(pipeline)
                Return New pipeline(DirectCast(ions, pipeline).populates(Of Ions).IonPeaks, GetType(PeakMs2))
            Case Else
                Return Internal.debug.stop(New InvalidCastException(inputType.FullName), env)
        End Select
    End Function

    <ExportAPI("write.mgf")>
    Public Function writeMgfIons(ions As Object, file$, Optional relativeInto As Boolean = False) As Boolean
        If ions.GetType() Is GetType(pipeline) Then
            Using mgfWriter As StreamWriter = file.OpenWriter(Encodings.ASCII, append:=False)
                For Each ionPeak As PeakMs2 In DirectCast(ions, pipeline).populates(Of PeakMs2)
                    Call ionPeak _
                        .MgfIon _
                        .WriteAsciiMgf(mgfWriter, relativeInto)
                Next
            End Using
        ElseIf ions.GetType Is GetType(LibraryMatrix) Then
            Using mgf As StreamWriter = file.OpenWriter
                Call DirectCast(ions, LibraryMatrix) _
                    .MgfIon _
                    .WriteAsciiMgf(mgf)
            End Using
        End If

        Return True
    End Function

    ''' <summary>
    ''' Converts profiles peak data to peak data in centroid mode.
    ''' 
    ''' profile and centroid in Mass Spectrometry?
    ''' 
    ''' 1. Profile means the continuous wave form in a mass spectrum.
    '''   + Number of data points Is large.
    ''' 2. Centroid means the peaks in a profile data Is changed to bars.
    '''   + location of the bar Is center of the profile peak.
    '''   + height of the bar Is area of the profile peak.
    '''   
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <returns>
    ''' Peaks data in centroid mode.
    ''' </returns>
    <ExportAPI("centroid")>
    Public Function centroid(ions As Object, Optional intoCutoff As Double = 0.05, Optional parallel As Boolean = False, Optional env As Environment = Nothing) As Object
        Dim inputType As Type = ions.GetType

        If inputType Is GetType(pipeline) OrElse inputType Is GetType(PeakMs2()) Then
            Dim source As IEnumerable(Of PeakMs2) = If(inputType Is GetType(pipeline), DirectCast(ions, pipeline).populates(Of PeakMs2), DirectCast(ions, PeakMs2()))
            Dim converter = Iterator Function() As IEnumerable(Of PeakMs2)
                                For Each peak As PeakMs2 In source
                                    If Not peak.mzInto.centroid Then
                                        peak.mzInto.ms2 = peak.mzInto.ms2 _
                                            .Centroid(intoCutoff) _
                                            .ToArray
                                        ' peak.mzInto = peak.mzInto.Shrink(tolerance:=Tolerance.DeltaMass(0.3))
                                    End If

                                    Yield peak
                                Next
                            End Function

            If parallel Then
                Return New pipeline(converter().AsParallel, GetType(PeakMs2))
            Else
                Return New pipeline(converter(), GetType(PeakMs2))
            End If
        ElseIf inputType Is GetType(PeakMs2) Then
            Dim ms2Peak As PeakMs2 = DirectCast(ions, PeakMs2)

            If Not ms2Peak.mzInto.centroid Then
                ms2Peak.mzInto.ms2 = ms2Peak.mzInto.ms2 _
                    .Centroid(intoCutoff) _
                    .ToArray
            End If

            Return ms2Peak
        ElseIf inputType Is GetType(LibraryMatrix) Then
            Dim ms2 As LibraryMatrix = DirectCast(ions, LibraryMatrix)

            If Not ms2.centroid Then
                ms2 = ms2.CentroidMode(intoCutoff)
            End If

            Return ms2
        Else
            Return Internal.debug.stop(New InvalidCastException(inputType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' Convert mzxml file as mgf ions.
    ''' </summary>
    ''' <param name="mzXML"></param>
    ''' <returns></returns>
    <ExportAPI("mzxml.mgf")>
    Public Function mzXML2Mgf(mzXML$, Optional relativeInto As Boolean = False, Optional includesMs1 As Boolean = False) As pipeline
        Return mzXML _
            .scanLoader(relativeInto, includesMs1) _
            .Where(Function(peak) peak.mzInto.Length > 0) _
            .DoCall(Function(scans)
                        Return New pipeline(scans, GetType(PeakMs2))
                    End Function)
    End Function

    <Extension>
    Private Iterator Function scanLoader(mzXML$, relativeInto As Boolean, includesMs1 As Boolean) As IEnumerable(Of PeakMs2)
        Dim basename$ = mzXML.FileName

        For Each ms2Scan As scan In mzXMLAssembly.XML _
            .LoadScans(mzXML) _
            .Where(Function(s)
                       If includesMs1 Then
                           Return True
                       Else
                           Return s.msLevel = 2
                       End If
                   End Function)

            If ms2Scan.msLevel = 1 Then
                ' ms1的数据总是使用raw intensity值
                Yield ms2Scan.ScanData(basename, raw:=True)
            Else
                Yield ms2Scan.ScanData(basename, raw:=Not relativeInto)
            End If
        Next
    End Function

    <ExportAPI("ms1.scans")>
    Public Function getMs1Scans(<RRawVectorArgument> raw As Object) As ms1_scan()
        Dim files As String() = REnv.asVector(Of String)(raw)
        Dim ms1 As New List(Of ms1_scan)
        Dim peakScans As PeakMs2

        For Each file As String In files
            Select Case file.ExtensionSuffix.ToLower
                Case "mzxml"
                    Dim basename$ = file.FileName

                    For Each scan As scan In mzXMLAssembly.XML _
                        .LoadScans(file) _
                        .Where(Function(s)
                                   Return s.msLevel = 1
                               End Function)

                        ' ms1的数据总是使用raw intensity值
                        peakScans = scan.ScanData(basename, raw:=True)
                        ms1 += peakScans.mzInto.ms2 _
                            .Select(Function(frag)
                                        Return New ms1_scan With {
                                            .intensity = frag.intensity,
                                            .mz = frag.mz,
                                            .scan_time = peakScans.rt
                                        }
                                    End Function)
                    Next
                Case Else
                    Throw New NotImplementedException
            End Select
        Next

        Return ms1.ToArray
    End Function
End Module
