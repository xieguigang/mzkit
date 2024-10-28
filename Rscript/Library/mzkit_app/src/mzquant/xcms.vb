#Region "Microsoft.VisualBasic::4395e2ae6d5f6ff92fa144280858ef26, Rscript\Library\mzkit_app\src\mzquant\xcms.vb"

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

    '   Total Lines: 47
    '    Code Lines: 34 (72.34%)
    ' Comment Lines: 3 (6.38%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (21.28%)
    '     File Size: 1.63 KB


    ' Module xcms
    ' 
    '     Function: parse_xcms_samples
    ' 
    ' /********************************************************************************/

#End Region


Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

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

End Module

