#Region "Microsoft.VisualBasic::c10bed4f83203be07bd3b646b99d97e9, mzkit\src\visualize\MsImaging\Blender\Scaler\LogScaler.vb"

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

'   Total Lines: 24
'    Code Lines: 18
' Comment Lines: 0
'   Blank Lines: 6
'     File Size: 717 B


'     Class LogScaler
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: DoIntensityScale, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports std = System.Math

Namespace Blender.Scaler

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
