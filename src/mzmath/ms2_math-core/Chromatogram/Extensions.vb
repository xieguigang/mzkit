#Region "Microsoft.VisualBasic::c1e3e1e9bc4c65329e7633a41183b48e, src\mzmath\ms2_math-core\Chromatogram\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: XIC
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Math

Namespace Chromatogram

    <HideModuleName>
    Public Module Extensions

        <Extension>
        Public Iterator Function XIC(ms1 As IEnumerable(Of ms1_scan), mzErr As Tolerance) As IEnumerable(Of (mz As Double, chromatogram As ChromatogramTick()))
            For Each mz As NamedCollection(Of ms1_scan) In ms1 _
                .GroupBy(Function(p) p.mz, mzErr) _
                .OrderBy(Function(mzi)
                             Return Val(mzi.name)
                         End Function)

                Dim scan = mz.OrderBy(Function(p) p.scan_time).ToArray
                Dim ticks = scan _
                    .Select(Function(p)
                                Return New ChromatogramTick With {
                                    .Intensity = p.intensity,
                                    .Time = p.scan_time
                                }
                            End Function) _
                    .ToArray

                Yield (Val(mz.name), ticks)
            Next
        End Function
    End Module
End Namespace
