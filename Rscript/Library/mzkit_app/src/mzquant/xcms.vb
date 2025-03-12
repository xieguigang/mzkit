#Region "Microsoft.VisualBasic::0063de283b55458d089c8eeba2431b5b, Rscript\Library\mzkit_app\src\mzquant\xcms.vb"

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

    '   Total Lines: 136
    '    Code Lines: 91 (66.91%)
    ' Comment Lines: 28 (20.59%)
    '    - Xml Docs: 96.43%
    ' 
    '   Blank Lines: 17 (12.50%)
    '     File Size: 5.43 KB


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

    <ExportAPI("parse_xcms_samples")>
    <RApiReturn(GetType(XcmsSamplePeak), GetType(PeakFeature))>
    Public Function parse_xcms_samples(<RRawVectorArgument> file As Object,
                                       Optional group_features As Boolean = False,
                                       Optional env As Environment = Nothing) As Object

        Dim auto_close As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env,
                                            is_filepath:=auto_close)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim peaks As IEnumerable(Of XcmsSamplePeak) = XcmsSamplePeak.ParseCsv(s.TryCast(Of Stream))

        If group_features Then
            Dim samples = peaks.GroupBy(Function(si) si.sample).ToArray
            Dim samplelist As list = list.empty

            For Each sample In samples
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

End Module
