#Region "Microsoft.VisualBasic::d88abdb3cbfaf98a59d1a9f05babba4c, ms2_math-core\Spectra\Models\ms2AlignMatrix.vb"

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

    '     Class SSM2MatrixFragment
    ' 
    '         Properties: da, mz, query, ref
    ' 
    '         Function: createFragment, FromXml, ToString
    ' 
    '     Class Ms2AlignMatrix
    ' 
    '         Properties: SSM
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml
Imports Microsoft.VisualBasic.Math.Scripting
Imports MathCore = Microsoft.VisualBasic.Math

Namespace Spectra

    Public Class SSM2MatrixFragment

        ''' <summary>
        ''' The m/z value of the query fragment
        ''' </summary>
        ''' <returns></returns>
        Public Property mz As Double

#Region "Fragment intensity"
        Public Property query As Double
        Public Property ref As Double
#End Region

        ''' <summary>
        ''' Mass delta between the query and reference fragment in unit ``da``
        ''' </summary>
        ''' <returns></returns>
        Public Property da As String

        Public Shared Function FromXml(node As XmlNode, nodeName$) As SSM2MatrixFragment()
            Return (From child As XmlNode
                    In node.ChildNodes
                    Where child.Name = nodeName) _
 _
                .Select(AddressOf createFragment) _
                .ToArray
        End Function

        Private Shared Function createFragment(feature As XmlNode) As SSM2MatrixFragment
            Dim data = feature.Attributes
            Dim mz, query, ref As Double
            Dim da As String

            With data
                mz = !mz.Value
                query = !query.Value.ParseDouble
                ref = !ref.Value.ParseDouble
                da = !da.Value
            End With

            Return New SSM2MatrixFragment With {
                .mz = mz,
                .query = query,
                .ref = ref,
                .da = da
            }
        End Function

        Public Overrides Function ToString() As String
            Return mz
        End Function
    End Class

    Public Class Ms2AlignMatrix : Inherits IVector(Of SSM2MatrixFragment)

        ''' <summary>
        ''' 计算两个色谱矩阵之间的余弦相似度
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SSM As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                With Me
                    Return MathCore.SSM(!query, !ref)
                End With
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(source As IEnumerable(Of SSM2MatrixFragment))
            Call MyBase.New(source)
        End Sub
    End Class
End Namespace
