#Region "Microsoft.VisualBasic::d4a145e28a21339550277e23eb19b319, mzkit\src\mzkit\ControlLibrary\MSI\Editor\Constants.vb"

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

    '   Total Lines: 26
    '    Code Lines: 23
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 480.00 B


    '     Enum MenuOption
    ' 
    '         AddRelation, AddVertex, DeleteVertex, HalveEdge, MoveComponent
    '         MovePolygon, RemovePolygon, RemoveRelation
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum Relation
    ' 
    '         Equality, None, Perpendicular
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum Algorithm
    ' 
    '         Antialiasing, Bresenham, BresenhamSymmetric, Library
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace PolygonEditor

    Friend Enum MenuOption
        MoveComponent
        AddVertex
        DeleteVertex
        MovePolygon
        RemovePolygon
        HalveEdge
        AddRelation
        RemoveRelation
    End Enum

    Friend Enum Relation
        None
        Equality
        Perpendicular
    End Enum

    Friend Enum Algorithm
        Bresenham
        Library
        Antialiasing
        BresenhamSymmetric
    End Enum
End Namespace
