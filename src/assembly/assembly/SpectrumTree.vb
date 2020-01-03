#Region "Microsoft.VisualBasic::ef92baf0edf6b50325731f04d77675d3, src\assembly\assembly\SpectrumTree.vb"

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

    ' Class SpectrumTree
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateTreeFromFile, doCluster
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.MassSpectrum.Assembly.MarkupData
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Spectra

''' <summary>
''' Clustering of the ms2 spectrum by SSM similarity.
''' </summary>
Public Class SpectrumTree : Inherits Spectra.SpectrumTreeCluster

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(Optional compares As Comparison(Of PeakMs2) = Nothing, Optional showReport As Boolean = True)
        Call MyBase.New(compares, showReport)
    End Sub

    ''' <summary>
    ''' Clustering of the raw ms2 spectrum data
    ''' </summary>
    ''' <param name="ms2list"></param>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overloads Function doCluster(ms2list As mzXML.scan(), fileName$) As SpectrumTree
        Return doCluster(ms2list _
            .Select(Function(scan) scan.ScanData(fileName)) _
            .OrderBy(Function(ms) ms.rt) _
            .ToArray
        )
    End Function

    Public Shared Function CreateTreeFromFile(file As String) As SpectrumTree
        Dim allMs2Scans = mzXML.XML _
            .LoadScans(file) _
            .Where(Function(s) s.msLevel = "2") _
            .OrderBy(Function(mz) mz.precursorMz) _
            .ToArray

        Return New SpectrumTree(showReport:=True).doCluster(allMs2Scans, file.FileName)
    End Function
End Class
