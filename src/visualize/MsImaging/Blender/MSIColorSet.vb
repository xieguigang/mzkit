#Region "Microsoft.VisualBasic::b82b1b9677f0f0f4829f3c43288a1ffe, visualize\MsImaging\Blender\MSIColorSet.vb"

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

    '   Total Lines: 46
    '    Code Lines: 40
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.28 KB


    '     Module MSIColorSet
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetColors
    ' 
    '         Sub: DoRegister
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Namespace Blender

    Public Module MSIColorSet

        ReadOnly scales As String()

        Sub New()
            scales = {
                "#070711",
                "#110071",
                "#1202a2",
                "#185eff",
                "#11d6ea",
                "#20fdda",
                "#11f9a6",
                "#10f97a",
                "#3df31b",
                "#bbf50c",
                "#e7f116",
                "#f9bb1b",
                "#f17408",
                "#e23f00",
                "#f50961",
                "#f410a3",
                "#e333d1",
                "#f87bfd",
                "#ffffff"
            }
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetColors() As Color()
            Return (From str As String In scales Select str.TranslateColor).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub DoRegister()
            Call Designer.Register("MSImaging", GetColors)
        End Sub
    End Module
End Namespace
