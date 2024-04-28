#Region "Microsoft.VisualBasic::a880e4879943a57f0f369f39139a29ff, E:/mzkit/src/visualize/MsImaging//Blender/Scaler/LogScaler.vb"

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
    '    Code Lines: 22
    ' Comment Lines: 3
    '   Blank Lines: 6
    '     File Size: 901 B


    '     Class LogScaler
    ' 
    '         Properties: base
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
Imports std = System.Math

Namespace Blender.Scaler

    ''' <summary>
    ''' log for reduce the data difference
    ''' </summary>
    Public Class LogScaler : Inherits Scaler

        <XmlAttribute> Public Property base As Double = std.E

        Sub New(base As Double)
            Me.base = base
        End Sub
        Sub New()
            Call Me.New(base:=std.E)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function DoIntensityScale(into() As Double) As Double()
            Return New Vector(into).Log(base)
        End Function

        Public Overrides Function ToScript() As String
            Return $"log({base.ToString("F4")})"
        End Function
    End Class
End Namespace
