#Region "Microsoft.VisualBasic::b5f0aba52a5a2110af929e7a022cebe1, SpectrumTree\BlockNode.vb"

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

    ' Class BlockNode
    ' 
    '     Properties: left, right, scan0
    ' 
    '     Function: ReadNode
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO

Public Class BlockNode

    Public Property scan0 As Long
    Public Property left As BlockNode
    Public Property right As BlockNode

    Public Iterator Function ReadNode(file As Stream) As IEnumerable(Of PeakMs2)
        Dim infile As New BinaryDataReader(file)
        Dim size As Long
        Dim nspectra As Integer

        infile.Seek(scan0, SeekOrigin.Begin)
        size = infile.ReadInt64
        infile.ReadString(BinaryStringFormat.ZeroTerminated)
        nspectra = infile.ReadInt32

        For i As Integer = 0 To nspectra - 1
            Yield Reader.ReadSpectra(infile)
        Next
    End Function

End Class
