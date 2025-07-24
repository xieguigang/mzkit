﻿#Region "Microsoft.VisualBasic::7b89876807c897fed6d8a0b6d1c62513, Rscript\Library\mzkit_app\src\mzquant\xcms.vb"

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

    '   Total Lines: 149
    '    Code Lines: 93 (62.42%)
    ' Comment Lines: 39 (26.17%)
    '    - Xml Docs: 97.44%
    ' 
    '   Blank Lines: 17 (11.41%)
    '     File Size: 6.18 KB


    ' Module xcms
    ' 
    '     Function: cast_raw_dataframe, parse_xcms_samples, setAnnotations
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' the xcms interop and data handler
''' </summary>
<Package("xcms")>
Module xcms

    ''' <summary>
    ''' Parse the input file as the mzkit peakset object
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="group_features">
    ''' This function returns a xcms sample peaks collection for directly mapping of the input tabular file data. 
    ''' set this parameter to value true, will returns a tuple list object that contains the mzkit
    ''' peak feature groups, which is group by the sample names.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("parse_xcms_samples")>
    <RApiReturn(GetType(XcmsSamplePeak), GetType(PeakFeature))>
    Public Function parse_xcms_samples(<RRawVectorArgument> file As Object,
                                       Optional group_features As Boolean = False,
                                       Optional deli As Char = ","c,
                                       Optional normalizeID As Boolean = True,
                                       Optional env As Environment = Nothing) As Object

        Dim auto_close As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env,
                                            is_filepath:=auto_close)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim peaks As IEnumerable(Of XcmsSamplePeak) = XcmsSamplePeak.ParseFile(s.TryCast(Of Stream), deli, normalizeID)

        If group_features Then
            Dim samples = peaks.GroupBy(Function(si) si.sample).ToArray
            Dim samplelist As list = list.empty

            For Each sample As IGrouping(Of String, XcmsSamplePeak) In samples
                Call samplelist.add(sample.Key, From si In sample Select New PeakFeature(si))
            Next

            Return samplelist
        Else
            Return peaks.ToArray
        End If
    End Function

    ''' <summary>
    ''' cast the xcms find peaks result raw dataframe to mzkit peak feature data
    ''' </summary>
    ''' <param name="x">
    ''' A dataframe result of the xcms ``findPeaks``. data could be generated via ``as.data.frame(findPeaks(data));``.
    ''' this raw data frame output contains data fields:
    ''' 
    ''' 1. mz, mzmin, mzmax
    ''' 2. rt, rtmin, rtmax
    ''' 3. into, intf
    ''' 4. maxo, maxf
    ''' 5. i
    ''' 6. sn
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("cast_findpeaks_raw")>
    <RApiReturn(GetType(PeakFeature))>
    Public Function cast_raw_dataframe(x As dataframe, Optional sample_name As String = Nothing) As Object
        Dim mz As Double() = CLRVector.asNumeric(x!mz)
        Dim mzmin As Double() = CLRVector.asNumeric(x!mzmin)
        Dim mzmax As Double() = CLRVector.asNumeric(x!mzmax)
        Dim rt As Double() = CLRVector.asNumeric(x!rt)
        Dim rtmin As Double() = CLRVector.asNumeric(x!rtmin)
        Dim rtmax As Double() = CLRVector.asNumeric(x!rtmax)
        Dim into As Double() = CLRVector.asNumeric(x!into)
        Dim intf As Double() = CLRVector.asNumeric(x!intf)
        Dim maxo As Double() = CLRVector.asNumeric(x!maxo)
        Dim maxf As Double() = CLRVector.asNumeric(x!maxf)
        Dim i As String() = CLRVector.asCharacter(x!i)
        Dim sn As Double() = CLRVector.asNumeric(x!sn)

        Return mz _
            .Select(Function(mzi, offset)
                        Return New PeakFeature With {
                            .mz = mzi,
                            .area = into(offset),
                            .baseline = 1,
                            .integration = into(offset),
                            .maxInto = maxo(offset),
                            .noise = 1,
                            .nticks = 1,
                            .rawfile = sample_name,
                            .RI = 0,
                            .rt = rt(offset),
                            .rtmin = rtmin(offset),
                            .rtmax = rtmax(offset),
                            .xcms_id = i(offset)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' set annotation to the ion features
    ''' </summary>
    ''' <param name="peaktable"></param>
    ''' <param name="id">should be a character vector of the ion reference id</param>
    ''' <param name="annotation">should be a collection of the metabolite annotation model <see cref="MetID"/>, 
    ''' size of this collection should be equals to the size of the given id vector.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("set_annotations")>
    <RApiReturn(GetType(PeakSet))>
    Public Function setAnnotations(peaktable As PeakSet,
                                   <RRawVectorArgument> id As Object,
                                   <RRawVectorArgument> annotation As Object,
                                   Optional env As Environment = Nothing) As Object

        Dim xcms_id As String() = CLRVector.asCharacter(id)
        Dim metid As pipeline = pipeline.TryCreatePipeline(Of MetID)(annotation, env)
        Dim i As i32 = 0
        Dim list As New Dictionary(Of String, MetID)

        If metid.isError Then
            Return metid.getError
        End If

        For Each met As MetID In metid.populates(Of MetID)(env)
            Call list.Add(xcms_id(++i), met)
        Next

        peaktable.annotations = list

        Return peaktable
    End Function

    ''' <summary>
    ''' Create the expression dataframe for a specific ion peak data
    ''' </summary>
    ''' <param name="ion"></param>
    ''' <param name="sampleinfo"></param>
    ''' <returns></returns>
    <ExportAPI("expression_df")>
    Public Function expression_df(ion As xcms2, sampleinfo As SampleInfo()) As Object
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}

        Call df.add("id", sampleinfo.Select(Function(si) si.ID))
        Call df.add("name", sampleinfo.Select(Function(si) si.sample_name))
        Call df.add("group", sampleinfo.Select(Function(si) si.sample_info))
        Call df.add("expr", sampleinfo.Select(Function(si) ion(si.ID)))

        Return df
    End Function

End Module
