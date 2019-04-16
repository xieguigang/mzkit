Namespace File

    Public Enum BoundTypes As Integer
        ''' <summary>
        ''' 非碳原子的化学键连接可能会存在其他数量的键
        ''' </summary>
        [Other] = 0
        ''' <summary>
        ''' 单键
        ''' </summary>
        [Single] = 1
        ''' <summary>
        ''' 双键
        ''' </summary>
        [Double] = 2
        ''' <summary>
        ''' 三键
        ''' </summary>
        [Triple] = 3
        ''' <summary>
        ''' 四键
        ''' </summary>
        [Aromatic] = 4
    End Enum

    ''' <summary>
    ''' 空间立体结构的类型
    ''' </summary>
    Public Enum BoundStereos As Integer
        NotStereo = 0
        Up = 1
        Down = 6
        Other
    End Enum
End Namespace