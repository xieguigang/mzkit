#Region "Microsoft.VisualBasic::752a288180cc9c1645ee1951ed402b31, Library\mzkit\assembly\MzWeb.vb"

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

    ' Module MzWeb
    ' 
    '     Function: GetChromatogram, loadStream, writeStream
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' biodeep mzweb data viewer raw data file helper
''' </summary>
<Package("mzweb")>
Module MzWeb

    <ExportAPI("load.chromatogram")>
    <RApiReturn(GetType(BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader.Chromatogram))>
    Public Function GetChromatogram(scans As pipeline, Optional env As Environment = Nothing) As Object
        If scans.elementType Like GetType(mzXML.scan) Then
            Return BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader.Chromatogram.GetChromatogram(scans.populates(Of scan)(env))
        ElseIf scans.elementType Like GetType(mzML.spectrum) Then
            Return BioNovoGene.Analytical.MassSpectrometry.Assembly.DataReader.Chromatogram.GetChromatogram(scans.populates(Of mzML.spectrum)(env))
        Else
            Return Message.InCompatibleType(GetType(mzXML.scan), scans.elementType, env)
        End If
    End Function

    ''' <summary>
    ''' load the unify mzweb scan stream data from the mzml/mzxml raw scan data stream.
    ''' </summary>
    ''' <param name="scans"></param>
    ''' <param name="mzErr$"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("load.stream")>
    <RApiReturn(GetType(ScanMS1))>
    Public Function loadStream(scans As pipeline,
                               Optional mzErr$ = "da:0.1",
                               Optional env As Environment = Nothing) As pipeline

        If scans.elementType Like GetType(mzXML.scan) Then
            Return mzWebCache _
                .Load(scans.populates(Of scan)(env), mzErr) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        ElseIf scans.elementType Like GetType(mzML.spectrum) Then
            Return mzWebCache _
                .Load(scans.populates(Of mzML.spectrum)(env), mzErr) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        Else
            Return Message.InCompatibleType(GetType(mzXML.scan), scans.elementType, env)
        End If
    End Function

    <ExportAPI("write.cache")>
    Public Function writeStream(scans As pipeline,
                                Optional file As Object = Nothing,
                                Optional env As Environment = Nothing) As Object
        Dim stream As Stream

        If file Is Nothing Then
            stream = Console.OpenStandardOutput
        ElseIf TypeOf file Is String Then
            stream = DirectCast(file, String).Open(doClear:=True)
        ElseIf TypeOf file Is Stream Then
            stream = DirectCast(file, Stream)
        Else
            Return Message.InCompatibleType(GetType(Stream), file.GetType, env)
        End If

        Call scans.populates(Of ScanMS1)(env).Write(stream)

        Return True
    End Function
End Module
