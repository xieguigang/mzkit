#Region "Microsoft.VisualBasic::efe7f18c0e08054e42236b867837bfa8, G:/mzkit/src/visualize/MsImaging//Blender/Scaler/PowerScaler.vb"

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

    '   Total Lines: 31
    '    Code Lines: 21
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 842 B


    '     Class PowerScaler
    ' 
    '         Properties: pow
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: DoIntensityScale, ToScript
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace Blender.Scaler

    ''' <summary>
    ''' power for increase the data difference
    ''' </summary>
    Public Class PowerScaler : Inherits Scaler

        <XmlAttribute> Public Property pow As Double = 2

        Sub New()
            Call Me.New(p:=2)
        End Sub

        Sub New(p As Double)
            pow = p
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            Return New Vector(into) ^ pow
        End Function

        Public Overrides Function ToScript() As String
            Return $"power({pow})"
        End Function
    End Class
End Namespace
