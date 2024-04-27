#Region "Microsoft.VisualBasic::cf6cd2e829e8abd3c60c7a3b6b342054, G:/mzkit/src/visualize/MsImaging//Blender/Scaler/DenoiseScaler.vb"

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

    '   Total Lines: 43
    '    Code Lines: 28
    ' Comment Lines: 7
    '   Blank Lines: 8
    '     File Size: 1.27 KB


    '     Class DenoiseScaler
    ' 
    '         Properties: q
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: DoIntensityScale, ToScript
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace Blender.Scaler

    ''' <summary>
    ''' removes the pixel points by the average density cutoff
    ''' </summary>
    Public Class DenoiseScaler : Inherits Scaler

        ''' <summary>
        ''' The density cutoff that treated the pixel as noise pixels
        ''' </summary>
        ''' <returns></returns>
        <Description("The density cutoff that treate some pixel as the noise pixels.")>
        <XmlAttribute>
        Public Property q As Double

        Sub New(q As Double)
            Me.q = q
        End Sub

        Sub New()
            Call Me.New(q:=0.01)
        End Sub

        Public Overrides Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Dim denoise As PixelData() = layer.MSILayer _
                .DensityCut(q) _
                .ToArray

            Return New SingleIonLayer With {
                .MSILayer = denoise,
                .DimensionSize = layer.DimensionSize,
                .IonMz = layer.IonMz
            }
        End Function

        Public Overrides Function ToScript() As String
            Return $"denoise({q})"
        End Function
    End Class
End Namespace
