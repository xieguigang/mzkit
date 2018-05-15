Imports System.Runtime.CompilerServices
Imports System.Xml
Imports Microsoft.VisualBasic.Math.Scripting
Imports MathCore = Microsoft.VisualBasic.Math

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
            .Select(Function(feature)
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
                    End Function) _
            .ToArray
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