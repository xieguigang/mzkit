#Region "Microsoft.VisualBasic::1bc96b6d7cc80b768223adb2b5dbc763, src\metadb\Massbank\SDF\Struct\Enums.vb"

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

    '     Enum BoundTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum BoundStereos
    ' 
    '         Other
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
