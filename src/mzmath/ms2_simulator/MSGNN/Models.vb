' ============================================================================
' Models.vb
' ============================================================================
' 数据结构定义：质谱峰、谱图、图节点、图边、图数据
' ============================================================================

''' <summary>
''' 图节点类型枚举
''' </summary>
Public Enum NodeType
    ParentIon = 0   ' 母离子根节点
    Fragment = 1    ' 碎片峰节点
End Enum

''' <summary>
''' 图节点
''' </summary>
Public Class GraphNode
    Public Property Id As Integer
    Public Property Type As NodeType
    Public Property Mz As Double
    Public Property Intensity As Double
    Public Property NormalizedIntensity As Double
    Public Property FeatureIonName As String = Nothing  ' 匹配到的特征离子名称 (Nothing表示无匹配)
    Public Property FeatureIonIndex As Integer = -1     ' 特征离子在One-hot向量中的索引 (-1表示无匹配)

    ''' <summary>
    ''' 获取节点的特征向量
    ''' [0] = m/z
    ''' [1] = 归一化强度
    ''' [2..N] = 特征离子One-hot编码
    ''' </summary>
    Public Function GetFeatureVector(numFeatureIons As Integer) As Double()
        Dim features As New List(Of Double)()
        features.Add(Mz)
        features.Add(NormalizedIntensity)
        ' One-hot编码
        For i = 0 To numFeatureIons - 1
            If i = FeatureIonIndex Then
                features.Add(1.0)
            Else
                features.Add(0.0)
            End If
        Next
        Return features.ToArray()
    End Function

    Public Overrides Function ToString() As String
        Return $"Node[{Id}] {Type} m/z={Mz:F4} I={NormalizedIntensity:F4} FI={If(FeatureIonName, "none")}"
    End Function
End Class

''' <summary>
''' 图边
''' </summary>
Public Class GraphEdge
    Public Property Source As Integer
    Public Property Target As Integer
    Public Property EdgeTypeName As String       ' 边类型名称 (如 "NL_H2O", "AD_Na", "ISO_C13")
    Public Property EdgeTypeIndex As Integer     ' 边类型在One-hot向量中的索引
    Public Property MassDiff As Double           ' 实际质量差 |Δm/z|
    Public Property SignedMassDiff As Double     ' 带符号质量差 (Source - Target)
    Public Property IntensityRatio As Double     ' log(IA/IB)
    Public Property MassError As Double          ' 质量匹配误差

    ''' <summary>
    ''' 获取边的特征向量
    ''' [0..N-1] = 边类型One-hot编码
    ''' [N] = 质量差值 |Δm/z|
    ''' [N+1] = 强度比值 log(IA/IB)
    ''' </summary>
    Public Function GetFeatureVector(numEdgeTypes As Integer) As Double()
        Dim features As New List(Of Double)()
        ' One-hot编码
        For i = 0 To numEdgeTypes - 1
            If i = EdgeTypeIndex Then
                features.Add(1.0)
            Else
                features.Add(0.0)
            End If
        Next
        ' 质量差
        features.Add(MassDiff)
        ' 强度比
        features.Add(IntensityRatio)
        Return features.ToArray()
    End Function

    Public Overrides Function ToString() As String
        Return $"Edge[{Source}->{Target}] {EdgeTypeName} Δm/z={MassDiff:F4} log(I)={IntensityRatio:F3}"
    End Function
End Class

''' <summary>
''' 完整的图数据
''' </summary>
Public Class GraphData
    Public Property SpectrumTitle As String = ""
    Public Property Nodes As New List(Of GraphNode)()
    Public Property Edges As New List(Of GraphEdge)()
    Public Property Metadata As New Dictionary(Of String, Object)()

    Public ReadOnly Property NumNodes As Integer
        Get
            Return Nodes.Count
        End Get
    End Property

    Public ReadOnly Property NumEdges As Integer
        Get
            Return Edges.Count
        End Get
    End Property

    Public ReadOnly Property NumNodeFeatures As Integer
        Get
            Return 2 + KnownConstants.FeatureIonCategories.Count
        End Get
    End Property

    Public ReadOnly Property NumEdgeFeatures As Integer
        Get
            Return KnownConstants.EdgeTypeCategories.Count + 2
        End Get
    End Property

    ''' <summary>
    ''' 获取节点特征矩阵 (num_nodes x num_features)
    ''' </summary>
    Public Function GetNodeFeatureMatrix() As Double(,)
        Dim numNodes = Nodes.Count
        Dim numFeatures = NumNodeFeatures
        If numNodes = 0 Then
            Return New Double(-1, -1) {}
        End If
        Dim matrix(numNodes - 1, numFeatures - 1) As Double
        For i = 0 To numNodes - 1
            Dim features = Nodes(i).GetFeatureVector(numFeatures - 2)
            For j = 0 To numFeatures - 1
                matrix(i, j) = features(j)
            Next
        Next
        Return matrix
    End Function

    ''' <summary>
    ''' 获取边索引矩阵 (2 x num_edges)
    ''' 第0行是源节点，第1行是目标节点
    ''' </summary>
    Public Function GetEdgeIndexMatrix() As Integer(,)
        Dim numEdges = Edges.Count
        If numEdges = 0 Then
            Return New Integer(1, -1) {}
        End If
        Dim matrix(1, numEdges - 1) As Integer
        For i = 0 To numEdges - 1
            matrix(0, i) = Edges(i).Source
            matrix(1, i) = Edges(i).Target
        Next
        Return matrix
    End Function

    ''' <summary>
    ''' 获取边特征矩阵 (num_edges x num_edge_features)
    ''' </summary>
    Public Function GetEdgeFeatureMatrix() As Double(,)
        Dim numEdges = Edges.Count
        Dim numFeatures = NumEdgeFeatures
        If numEdges = 0 Then
            Return New Double(-1, -1) {}
        End If
        Dim matrix(numEdges - 1, numFeatures - 1) As Double
        For i = 0 To numEdges - 1
            Dim features = Edges(i).GetFeatureVector(numFeatures - 2)
            For j = 0 To numFeatures - 1
                matrix(i, j) = features(j)
            Next
        Next
        Return matrix
    End Function

End Class
