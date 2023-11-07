#Region "Microsoft.VisualBasic::959cfd9b418f2ae97cfa033953c3b96a, mzkit\Rscript\Library\mzkit.quantify\zzz.vb"

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

'   Total Lines: 10
'    Code Lines: 7
' Comment Lines: 0
'   Blank Lines: 3
'     File Size: 174 B


' Class zzz
' 
'     Sub: onLoad
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime

<Assembly: RPackageModule>

Public Class zzz

    Public Shared Sub onLoad()
        Call mzDeco.Main()
        Call Linears.Main()

        REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(ROI()), AddressOf ROISummary)
    End Sub

    Private Shared Function ROISummary(peaks As ROI(), args As List, env As Environment) As Rdataframe
        Dim rt As Array = peaks.Select(Function(r) r.rt).ToArray
        Dim rtmin As Double() = peaks.Select(Function(r) r.time.Min).ToArray
        Dim rtmax As Double() = peaks.Select(Function(r) r.time.Max).ToArray
        Dim maxinto As Array = peaks.Select(Function(r) r.maxInto).ToArray
        Dim nticks As Array = peaks.Select(Function(r) r.ticks.Length).ToArray
        Dim baseline As Array = peaks.Select(Function(r) r.baseline).ToArray
        Dim area As Array = peaks.Select(Function(r) r.integration).ToArray
        Dim noise As Array = peaks.Select(Function(r) r.noise).ToArray
        Dim sn_ratio As Array = peaks.Select(Function(r) r.snRatio).ToArray

        Return New Rdataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {NameOf(rt), rt},
                {NameOf(rtmin), rtmin},
                {"rt(min)", rtmin.Select(Function(d) d / 60).ToArray},
                {NameOf(rtmax), rtmax},
                {"peak_width", (rtmax.AsVector - rtmin.AsVector).ToArray},
                {NameOf(maxinto), maxinto},
                {NameOf(nticks), nticks},
                {NameOf(baseline), baseline},
                {NameOf(area), area},
                {NameOf(noise), noise},
                {NameOf(sn_ratio), sn_ratio}
            }
        }
    End Function
End Class
