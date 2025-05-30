﻿#Region "Microsoft.VisualBasic::558d04392a6deb1748c207e9acef0e6f, assembly\SignalReader\RamanSpectroscopy.vb"

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

    '   Total Lines: 31
    '    Code Lines: 28 (90.32%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (9.68%)
    '     File Size: 1.28 KB


    ' Module RamanSpectroscopy
    ' 
    '     Function: ToChromatogram, ToSignal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing

Public Module RamanSpectroscopy

    <Extension>
    Public Function ToSignal(raman As Raman.Spectroscopy) As GeneralSignal
        Return New GeneralSignal With {
            .description = "Raman Spectroscopy",
            .measureUnit = "Raman Shift [cm-1]",
            .reference = raman.Title,
            .Measures = raman.xyData.Select(Function(p) CDbl(p.X)).ToArray,
            .Strength = raman.xyData.Select(Function(p) CDbl(p.Y)).ToArray,
            .meta = raman.Comments _
                .JoinIterates(raman.DetailedInformation) _
                .JoinIterates(raman.MeasurementInformation) _
                .ToDictionary
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function ToChromatogram(raman As Raman.Spectroscopy) As ChromatogramTick()
        Return raman.xyData _
            .Select(Function(c) New ChromatogramTick With {.Time = c.X, .Intensity = c.Y}) _
            .ToArray
    End Function
End Module
