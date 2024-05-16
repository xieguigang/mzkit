#Region "Microsoft.VisualBasic::45319e30a7051e0aa8f5b773d28ab3a9, assembly\assembly\mzPack\Binary\ScanModel.vb"

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

    '   Total Lines: 58
    '    Code Lines: 48
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 2.06 KB


    '     Module ScanModel
    ' 
    '         Function: Scan1, Scan2
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace mzData.mzWebCache

    ''' <summary>
    ''' Helper module for cast the clr object model
    ''' </summary>
    Public Module ScanModel

        <Extension>
        Public Function Scan2(i As PeakMs2) As ScanMS2
            Dim ionMode As Integer = 1

            If Not i.precursor_type.StringEmpty Then
                ionMode = Provider.ParseIonMode(i.precursor_type.Last)
            End If

            Return New ScanMS2 With {
                .centroided = True,
                .mz = i.mzInto.Select(Function(mzi) mzi.mz).ToArray,
                .into = i.mzInto.Select(Function(mzi) mzi.intensity).ToArray,
                .parentMz = i.mz,
                .intensity = i.intensity,
                .rt = i.rt,
                .scan_id = $"{i.file}#{i.lib_guid}",
                .collisionEnergy = i.collisionEnergy,
                .polarity = ionMode
            }
        End Function

        <Extension>
        Public Function Scan1(list As NamedCollection(Of PeakMs2)) As ScanMS1
            Dim scan2 As ScanMS2() = list _
                .Select(Function(i)
                            Return i.Scan2
                        End Function) _
                .ToArray

            Return New ScanMS1 With {
               .into = scan2 _
                   .Select(Function(i) i.intensity) _
                   .ToArray,
               .mz = scan2 _
                   .Select(Function(i) i.parentMz) _
                   .ToArray,
               .products = scan2,
               .rt = Val(list.name),
               .scan_id = list.name,
               .TIC = .into.Sum,
               .BPC = .into.Max
            }
        End Function
    End Module
End Namespace
