#Region "Microsoft.VisualBasic::51ddb66d0e0a6bfb7a2d853a282ceffa, mzmath\SingleCells\SingleCellIonStat.vb"

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

    '   Total Lines: 109
    '    Code Lines: 74 (67.89%)
    ' Comment Lines: 24 (22.02%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 11 (10.09%)
    '     File Size: 4.09 KB


    ' Class SingleCellIonStat
    ' 
    '     Properties: baseCell, cells, maxIntensity, mz, Q1Intensity
    '                 Q2Intensity, Q3Intensity, RSD
    ' 
    '     Function: DoIonStats, DoStatInternal, DoStatSingleIon, fillVector
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' A ion feature statistics information from a single cell data
''' </summary>
Public Class SingleCellIonStat

    ''' <summary>
    ''' the ion feature m/z value
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double
    Public Property mzmin As Double
    Public Property mzmax As Double
    Public Property mz_error As String

    ''' <summary>
    ''' the cell numbers that contains this ion feature.
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As Integer
    ''' <summary>
    ''' the max intensity value of current ion feature.
    ''' </summary>
    ''' <returns></returns>
    Public Property maxIntensity As Double
    ''' <summary>
    ''' the cell label id which has the max intensity of current ion feature.
    ''' </summary>
    ''' <returns></returns>
    Public Property baseCell As String
    Public Property Q1Intensity As Double
    Public Property Q2Intensity As Double
    Public Property Q3Intensity As Double
    ''' <summary>
    ''' RSD value of the intensity value of current ion feature across multiple cells
    ''' </summary>
    ''' <returns></returns>
    Public Property RSD As Double
    Public Property entropy As Double
    Public Property sparsity As Double

    Public Shared Function DoIonStats(raw As IMZPack, Optional da As Double = 0.01, Optional parallel As Boolean = True) As IEnumerable(Of SingleCellIonStat)
        Return raw.MS _
            .Select(Function(scan)
                        Return scan.GetMs.Select(Function(ms1) (scan.scan_id, ms1))
                    End Function) _
            .IteratesALL _
            .DoCall(Function(allIons)
                        Return DoStatInternal(allIons, da, parallel)
                    End Function)
    End Function

    ''' <summary>
    ''' do single cell ion feature statistics analysis
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="parallel"></param>
    ''' <returns></returns>
    Public Shared Iterator Function DoIonStats(mat As MzMatrix, Optional parallel As Boolean = True) As IEnumerable(Of SingleCellIonStat)
        If parallel Then
            For Each stat In mat.mz _
                .SeqIterator _
                .AsParallel _
                .Select(Function(i)
                            Dim offset As Integer = i
                            Dim cells = mat.matrix _
                                .Select(Function(cell) (cell.label, cell(i))) _
                                .ToArray

                            Return DoStatSingleIon(i.value, cells)
                        End Function)

                Yield stat
            Next
        Else
            Dim offset As Integer
            Dim cells As (String, Double)()

            For i As Integer = 0 To mat.featureSize - 1
                offset = i
                cells = mat.matrix _
                    .Select(Function(cell) (cell.label, cell(offset))) _
                    .ToArray

                Yield DoStatSingleIon(mat.mz(i), cells)
            Next
        End If
    End Function


End Class
