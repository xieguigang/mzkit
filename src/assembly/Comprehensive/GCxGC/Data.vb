#Region "Microsoft.VisualBasic::a2bd97234c07bed9380fe0052eb00cfd, assembly\Comprehensive\GCxGC\Data.vb"

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

    '   Total Lines: 68
    '    Code Lines: 58 (85.29%)
    ' Comment Lines: 5 (7.35%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 5 (7.35%)
    '     File Size: 2.67 KB


    ' Module Data
    ' 
    '     Function: Create2DData, (+2 Overloads) ExtractTIC, ExtractXIC
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.GCxGC
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Public Module Data

    <Extension>
    Public Function ExtractXIC(mz As Double, mzdiff As Tolerance) As Func(Of ScanMS1, Chromatogram2DScan)
        Return Function(d)
                   Return New Chromatogram2DScan With {
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

    <Extension>
    Public Function ExtractTIC(rawdata As mzPack) As IEnumerable(Of Chromatogram2DScan)
        Return rawdata.MS.Select(Function(d1) d1.ExtractTIC)
    End Function

    <Extension>
    Public Function ExtractTIC(d As ScanMS1) As Chromatogram2DScan
        Return New Chromatogram2DScan With {
            .intensity = d.TIC,
            .scan_time = d.rt,
            .chromatogram = d.products _
                .Select(Function(t)
                            Return New ChromatogramTick With {
                                .Intensity = t.into.Sum,
                                .Time = t.rt
                            }
                        End Function) _
                .ToArray,
            .scan_id = d.scan_id
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gcxgc">The converted GCxGC rawdata object</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Create2DData(gcxgc As mzPack) As IEnumerable(Of DimensionalSpectrum)
        For Each d1 As ScanMS1 In gcxgc.MS
            Yield New DimensionalSpectrum With {
                .rt1 = d1.rt,
                .baseIntensity = d1.BPC,
                .totalIon = d1.TIC,
                .ms2 = d1.products _
                    .Select(Function(si) si.GetSpectrum2) _
                    .OrderBy(Function(a) a.rt) _
                    .ToArray
            }
        Next
    End Function
End Module
