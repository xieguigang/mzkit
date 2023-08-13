#Region "Microsoft.VisualBasic::19f36fd9813f6c3bf75e301d88858ae4, mzkit\src\visualize\MsImaging\Blender\Scaler\Scaler.vb"

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
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.73 KB


    '     Class Scaler
    ' 
    '         Function: [Then], (+2 Overloads) DoIntensityScale
    '         Interface LayerScaler
    ' 
    '             Function: DoIntensityScale
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Blender.Scaler

    Public MustInherit Class Scaler : Implements LayerScaler

        Public Interface LayerScaler
            Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer
            Function ToScript() As String
        End Interface

        Public Overridable Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer Implements LayerScaler.DoIntensityScale
            Dim pixels = layer.MSILayer
            Dim intensity As Double() = pixels.Select(Function(i) i.intensity).ToArray
            Dim maxScale As Double

            intensity = DoIntensityScale(intensity)
            maxScale = intensity.Max
            pixels = pixels _
                .Select(Function(p, i)
                            Return New PixelData With {
                                .intensity = intensity(i),
                                .level = .intensity / maxScale,
                                .mz = p.mz,
                                .sampleTag = p.sampleTag,
                                .x = p.x,
                                .y = p.y
                            }
                        End Function) _
                .ToArray

            Return New SingleIonLayer With {
                .DimensionSize = layer.DimensionSize,
                .IonMz = layer.IonMz,
                .MSILayer = pixels
            }
        End Function

        Public MustOverride Function ToScript() As String Implements LayerScaler.ToScript

        Public Overrides Function ToString() As String
            Return ToScript()
        End Function

        Protected Overridable Function DoIntensityScale(into As Double()) As Double()
            Throw New NotImplementedException
        End Function

        Public Function [Then]([next] As Scaler) As RasterPipeline
            Return New RasterPipeline From {Me, [next]}
        End Function

    End Class

End Namespace
