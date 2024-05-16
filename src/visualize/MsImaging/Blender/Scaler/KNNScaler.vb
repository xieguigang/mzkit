#Region "Microsoft.VisualBasic::c5c77f39f5e9fb7b8f452be2121aaaa7, visualize\MsImaging\Blender\Scaler\KNNScaler.vb"

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

    '   Total Lines: 34
    '    Code Lines: 24
    ' Comment Lines: 3
    '   Blank Lines: 7
    '     File Size: 1.08 KB


    '     Class KNNScaler
    ' 
    '         Properties: k, q, random
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: DoIntensityScale, ToScript
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace Blender.Scaler

    ''' <summary>
    ''' fill the missing data with knn search method
    ''' </summary>
    Public Class KNNScaler : Inherits Scaler

        <XmlAttribute> Public Property k As Integer
        <XmlAttribute> Public Property q As Double
        <XmlAttribute> Public Property random As Boolean

        Public Sub New(k As Integer, q As Double, random As Boolean)
            Me.k = k
            Me.q = q
            Me.random = random
        End Sub

        Sub New()
            Call Me.New(k:=3, q:=0.65, random:=False)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Return layer.KnnFill(k, k, q, random)
        End Function

        Public Overrides Function ToScript() As String
            Return $"knn_fill({k},{q},random={random.ToString.ToLower})"
        End Function
    End Class
End Namespace
