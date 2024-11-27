#Region "Microsoft.VisualBasic::d4510ffc910a46e79d854bf251177c6c, mzmath\GCxGC\ChromatogramMesh.vb"

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
    '    Code Lines: 48 (70.59%)
    ' Comment Lines: 7 (10.29%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 13 (19.12%)
    '     File Size: 2.13 KB


    ' Class ChromatogramMesh
    ' 
    '     Properties: scan2D
    ' 
    ' Class Chromatogram2DScan
    ' 
    '     Properties: chromatogram, intensity, scan_id, scan_time, size
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Function: times, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Scripting.Expressions

''' <summary>
''' A 2d chromatogram mesh grid data that consists with the GCxGC chromatogram data
''' </summary>
Public Class ChromatogramMesh

    Public Property scan2D As Chromatogram2DScan()

End Class

Public Class Chromatogram2DScan : Implements IReadOnlyId, INamedValue
    Public Property scan_time As Double
    Public Property intensity As Double
    Public Property scan_id As String Implements INamedValue.Key, IReadOnlyId.Identity

    ''' <summary>
    ''' chromatogram data 2d
    ''' </summary>
    ''' <returns></returns>
    Public Property chromatogram As ChromatogramTick()

    Default Public ReadOnly Property getTick(i As DoubleRange) As ChromatogramTick()
        Get
            Return chromatogram.Where(Function(a) i.IsInside(a.Time)).ToArray
        End Get
    End Property

    Default Public ReadOnly Property getTick(i As Integer) As ChromatogramTick
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return _chromatogram(i)
        End Get
    End Property

    Public ReadOnly Property size As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return chromatogram.Length
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(t1 As Double)
        scan_time = t1
    End Sub

    Sub New(t1 As Double, id As String)
        scan_id = id
        scan_time = t1
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function times() As Double()
        Return chromatogram.Select(Function(t) t.Time).ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{intensity.ToString("G3")}@{scan_time.ToString("F2")}"
    End Function
End Class
