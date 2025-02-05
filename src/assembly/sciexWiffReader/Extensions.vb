﻿#Region "Microsoft.VisualBasic::ab0dfb5b092dcc8e84b238b61ca589e5, mzkit\src\assembly\sciexWiffReader\Extensions.vb"

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

    '   Total Lines: 16
    '    Code Lines: 14
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 467 B


    ' Module Extensions
    ' 
    '     Function: SubArray
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports vec = System.Array

Module Extensions

    <Extension>
    Public Function SubArray(Of T)(data As T(), length As Integer) As T()
        If data.Length <= length Then
            Return CType(data.Clone(), T())
        Else
            Dim array As T() = New T(length - 1) {}
            Call vec.Copy(data, Scan0, array, Scan0, length)
            Return array
        End If
    End Function
End Module
