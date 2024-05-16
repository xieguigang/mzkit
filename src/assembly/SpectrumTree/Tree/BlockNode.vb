#Region "Microsoft.VisualBasic::1d5676c36c25261ae954df4f51f72a9b, assembly\SpectrumTree\Tree\BlockNode.vb"

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

    '   Total Lines: 115
    '    Code Lines: 61
    ' Comment Lines: 46
    '   Blank Lines: 8
    '     File Size: 3.88 KB


    '     Class BlockNode
    ' 
    '         Properties: Block, centroid, childs, Id, isBlank
    '                     isLeaf, Members, mz, rt
    ' 
    '         Function: GetBinaryIndex, GetIndex, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Tree

    ''' <summary>
    ''' A spectrum tree node object
    ''' </summary>
    Public Class BlockNode

        ''' <summary>
        ''' the unique id of current reference spectrum
        ''' </summary>
        ''' <returns></returns>
        Public Property Id As String
        ''' <summary>
        ''' the file stream buffer pointer
        ''' </summary>
        ''' <returns></returns>
        Public Property Block As BufferRegion
        ''' <summary>
        ''' 得分0.9以上的都算作为当前节点的等价谱图
        ''' </summary>
        ''' <returns></returns>
        Public Property Members As List(Of Integer)
        ''' <summary>
        ''' A set of the precursor m/z of the member spectrum in current cluster
        ''' </summary>
        ''' <returns></returns>
        Public Property mz As List(Of Double)
        ''' <summary>
        ''' the average RT
        ''' </summary>
        ''' <returns></returns>
        Public Property rt As Double
        ''' <summary>
        ''' 总共10个元素，分别表示[0,1]区间内的10个阈值等级
        ''' 0,0.1,0.2,0.3,0.4,0.5,0.6,0.7,0.8,0.9
        ''' </summary>
        ''' <returns></returns>
        Public Property childs As Integer()
        ''' <summary>
        ''' the reference spectrum of current spectrum cluster family
        ''' </summary>
        ''' <returns></returns>
        Public Property centroid As ms2()

        Public ReadOnly Property isLeaf As Boolean
            Get
                Return childs.IsNullOrEmpty
            End Get
        End Property

        Public ReadOnly Property isBlank As Boolean
            Get
                ' ZERO always is root
                ' and root can not be a child
                ' so zero means no data at here
                Return childs.All(Function(c) c <= 0)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Id}: {mz.GetJson}@{rt}; MS/MS {centroid.Select(Function(m) $"{m.mz.ToString("F3")}:{m.intensity.ToString("G4")}").JoinBy("_")}"
        End Function

        ''' <summary>
        ''' the entire reference database must be rebuild after the
        ''' cutoff value in this function has been modified.
        ''' </summary>
        ''' <param name="score"></param>
        ''' <returns></returns>
        Friend Shared Function GetIndex(score As Double) As Integer
            If score > 0.85 Then
                ' min score greater than 0.85 means equals
                ' to current spectrum
                Return -1
            ElseIf score > 0.8 Then
                Return 0
            ElseIf score > 0.7 Then
                Return 1
            ElseIf score > 0.6 Then
                Return 2
            ElseIf score > 0.5 Then
                Return 3
            ElseIf score > 0.4 Then
                Return 4
            ElseIf score > 0.3 Then
                Return 5
            ElseIf score > 0.2 Then
                Return 6
            ElseIf score > 0.1 Then
                Return 7
            ElseIf score > 0 Then
                Return 8
            Else
                Return 9
            End If
        End Function

        Friend Shared Function GetBinaryIndex(score As Double) As Integer
            If score < 0.6 Then
                ' index = 0
                Return -1
            ElseIf score < 0.85 Then
                ' index = 1
                Return 1
            Else
                ' add to current member list
                Return 0
            End If
        End Function
    End Class
End Namespace
