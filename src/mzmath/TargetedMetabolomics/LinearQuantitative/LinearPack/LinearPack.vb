#Region "Microsoft.VisualBasic::f59d4f98fd5dfc4f86e3d9b9b4dc8749, TargetedMetabolomics\LinearQuantitative\LinearPack\LinearPack.vb"

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

    '     Class LinearPack
    ' 
    '         Properties: [IS], linears, peakSamples, reference, time
    '                     title
    ' 
    '         Function: GetLevelKeys, GetLinear, OpenFile, ToString, Write
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports Microsoft.VisualBasic.Linq

Namespace LinearQuantitative.Data

    ''' <summary>
    ''' 以CDF文件格式保存线性方程以及原始峰面积数据
    ''' </summary>
    Public Class LinearPack

        Public Property linears As StandardCurve()
        Public Property peakSamples As TargetPeakPoint()
        Public Property title As String
        Public Property time As Date
        Public Property reference As Dictionary(Of String, SampleContentLevels)
        Public Property [IS] As [IS]()

        Public Function GetLinear(id As String) As StandardCurve
            Return linears.Where(Function(line) line.name = id).FirstOrDefault
        End Function

        Public Function GetLevelKeys() As String()
            Return reference.Values _
                .Select(Function(ref) ref.levels.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        Public Overrides Function ToString() As String
            Return title
        End Function

        Public Function Write(file As String) As Boolean
            Using outfile As Stream = file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Return CDFWriter.Write(Me, outfile)
            End Using
        End Function

        Public Shared Function OpenFile(file As String) As LinearPack
            Using infile As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Return CDFReader.Load(infile)
            End Using
        End Function
    End Class
End Namespace
