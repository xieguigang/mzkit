#Region "Microsoft.VisualBasic::aaac8c1ccea0017d5165871ba5e67d65, G:/mzkit/src/visualize/MsImaging//Blender/MzLayerColorSet.vb"

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

    '   Total Lines: 36
    '    Code Lines: 28
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.14 KB


    '     Class MzLayerColorSet
    ' 
    '         Properties: colorSet, mz, tolerance
    ' 
    '         Function: FindColor, SelectGroup
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Namespace Blender

    Public Class MzLayerColorSet

        Public Property mz As Double()
        Public Property colorSet As Color()
        Public Property tolerance As Tolerance

        Default Public ReadOnly Property GetColor(i As Integer) As Color
            Get
                Return colorSet(i)
            End Get
        End Property

        Public Function FindColor(mz As Double) As Color
            Dim i As Integer = which(Me.mz.Select(Function(mzi) tolerance(mz, mzi))).FirstOrDefault(-1)

            If i = -1 Then
                Return Color.Transparent
            Else
                Return colorSet(i)
            End If
        End Function

        Public Function SelectGroup(pixels As PixelData()) As IEnumerable(Of NamedCollection(Of PixelData))
            Return pixels.GroupBy(Function(p) p.mz, tolerance)
        End Function

    End Class
End Namespace
