#Region "Microsoft.VisualBasic::84a36f3444f1e07f94d2cdc76e4749d4, mzkit\src\visualize\MsImaging\Blender\Scaler\RasterPipeline.vb"

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

    '   Total Lines: 57
    '    Code Lines: 44
    ' Comment Lines: 0
    '   Blank Lines: 13
    '     File Size: 2.05 KB


    '     Class RasterPipeline
    ' 
    '         Function: [Then], (+2 Overloads) DoIntensityScale, GetDefaultPipeline, GetEnumerator, IEnumerable_GetEnumerator
    '                   ToString
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Blender.Scaler

    ''' <summary>
    ''' a collection of the raster data filter: <see cref="Scaler"/>
    ''' </summary>
    Public Class RasterPipeline : Implements Scaler.LayerScaler, IEnumerable(Of Scaler), Enumeration(Of Scaler)

        ReadOnly pipeline As New List(Of Scaler)

        Default Public ReadOnly Property Run(layer As SingleIonLayer) As SingleIonLayer
            Get
                Return DoIntensityScale(layer)
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Add(scaler As Scaler)
            Call pipeline.Add(scaler)
        End Sub

        Public Function [Then](scaler As Scaler) As RasterPipeline
            Call pipeline.Add(scaler)
            Return Me
        End Function

        Public Function DoIntensityScale(into As Double()) As Double()
            For Each shader As Scaler In pipeline
                into = shader.DoIntensityScale(into)
            Next

            Return into
        End Function

        Public Function DoIntensityScale(pixels As IEnumerable(Of PixelData), dimSize As Size) As PixelData()
            Return DoIntensityScale(New SingleIonLayer With {.DimensionSize = dimSize, .IonMz = -1, .MSILayer = pixels.ToArray})
        End Function

        Public Function DoIntensityScale(layer As SingleIonLayer) As SingleIonLayer Implements Scaler.LayerScaler.DoIntensityScale
            For Each shader As Scaler In pipeline
                layer = shader.DoIntensityScale(layer)
            Next

            Return layer
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function GetDefaultPipeline() As RasterPipeline
            Return New LogScaler().Then(New TrIQScaler)
        End Function

        Public Overrides Function ToString() As String
            Return ToScript()
        End Function

        Public Function ToScript() As String Implements Scaler.LayerScaler.ToScript
            Return pipeline.JoinBy(" -> ")
        End Function

        Public Shared Function Parse(configs As IEnumerable(Of String)) As RasterPipeline
            Dim filter As New RasterPipeline

            For Each line As String In configs
                Call filter.Add(Scaler.Parse(line))
            Next

            Return filter
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Scaler) Implements IEnumerable(Of Scaler).GetEnumerator, Enumeration(Of Scaler).GenericEnumerator
            For Each scaler As Scaler In pipeline
                Yield scaler
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
