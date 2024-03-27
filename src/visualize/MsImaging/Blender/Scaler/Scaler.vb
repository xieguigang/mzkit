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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

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

        Public Overridable Function DoIntensityScale(into As Double()) As Double()
            Return into
        End Function

        Public Function [Then]([next] As Scaler) As RasterPipeline
            Return New RasterPipeline From {Me, [next]}
        End Function

        Public Shared Iterator Function GetFilters() As IEnumerable(Of Type)
            Yield GetType(DenoiseScaler)      ' $"denoise({q})"
            Yield GetType(IntensityCutScaler) ' $"cut({cutoff}, {percentage})"
            Yield GetType(KNNScaler)          ' $"knn_fill({k},{q})"
            Yield GetType(LogScaler)          ' $"log({base.ToString("F4")})"
            Yield GetType(PowerScaler)        ' $"power({pow})"
            Yield GetType(QuantileScaler)     ' $"Q({q.ToString("F4")},percentail:={percentail.ToString.ToLower})"
            Yield GetType(SoftenScaler)       ' $"soften()"
            Yield GetType(TrIQClip)           ' $"TrIQ_clip({threshold},{N})"
            Yield GetType(TrIQScaler)         ' $"TrIQ({threshold.ToString("F4")})"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(line As String) As Scaler
            Return ParserInternal(line:=Strings.Trim(line).ToLower)
        End Function

        Private Shared Function ParserInternal(line As String) As Scaler
            Dim config = line.GetTagValue("(", trim:=True)
            Dim par_str As String = config.Value.Trim(")"c)
            Dim pars As New ParameterSet(par_str.Split(","c))

            Select Case config.Name
                Case "soften" : Return New SoftenScaler
                Case "denoise" : Return New DenoiseScaler(pars.next(0.01))
                Case "triq" : Return New TrIQScaler(pars.next(0.65))
                Case "q" : Return New QuantileScaler(pars.next(0.5), pars.next(False))
                Case "knn_fill" : Return New KNNScaler(pars.next(3), pars.next(0.65), pars.next(False))
                Case "log" : Return New LogScaler(pars.next(System.Math.E))
                Case "power" : Return New PowerScaler(pars.next(2.0))
                Case "triq_clip" : Return New TrIQClip(pars.next(0.8), pars.next(100))
                Case "cut" : Return New IntensityCutScaler(pars.next(0.05), pars.next(False))
                Case Else
                    Throw New NotImplementedException(line & ": " & config.Name)
            End Select
        End Function
    End Class

    Friend Class ParameterSet

        Dim pars As String()
        Dim offset As i32 = 0

        Sub New(pars As String())
            Me.pars = pars
        End Sub

        Private Function getValueParseString([default] As String) As String
            Dim val_str As String = pars.ElementAtOrDefault(++offset, [default])
            Dim tokens = val_str.Split(":"c, "="c)

            If tokens.Length = 1 Then
                Return tokens.First
            Else
                Return tokens.Last
            End If
        End Function

        Public Function [next](default#) As Double
            Return Val(getValueParseString([default].ToString))
        End Function

        Public Function [next]([default] As Boolean) As Boolean
            Return getValueParseString([default].ToString).ParseBoolean
        End Function

        Public Function [next](default%) As Integer
            Return getValueParseString([default].ToString).ParseInteger
        End Function

        Public Overrides Function ToString() As String
            Return pars.GetJson
        End Function

    End Class

End Namespace
