
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("MsImaging")>
Module MsImaging

    ''' <summary>
    ''' load imzML data into the ms-imaging render
    ''' </summary>
    ''' <param name="imzML"></param>
    ''' <returns></returns>
    <ExportAPI("viewer")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function viewer(imzML As String) As Drawer
        Return New Drawer(imzML)
    End Function

    ''' <summary>
    ''' load the raw pixels data from imzML file 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="skip_zero"></param>
    ''' <returns></returns>
    <ExportAPI("pixels")>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <RApiReturn(GetType(PixelData))>
    Public Function LoadPixels(imzML As Drawer, mz As Double(),
                               Optional tolerance As Object = "da:0.1",
                               Optional skip_zero As Boolean = True,
                               Optional env As Environment = Nothing) As pipeline

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        Return imzML _
            .LoadPixels(mz, errors, skip_zero) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    ''' <summary>
    ''' render a ms-imaging layer by a specific ``m/z`` scan.
    ''' </summary>
    ''' <param name="viewer"></param>
    ''' <param name="mz"></param>
    ''' <param name="threshold"></param>
    ''' <param name="pixelSize"></param>
    ''' <param name="tolerance"></param>
    ''' <param name="color$"></param>
    ''' <param name="levels%"></param>
    ''' <returns></returns>
    <ExportAPI("layer")>
    <RApiReturn(GetType(Bitmap))>
    Public Function layer(viewer As Drawer, mz As Double(),
                          Optional threshold As Double = 0.05,
                          <RRawVectorArgument>
                          Optional pixelSize As Object = "5,5",
                          Optional tolerance As Object = "da:0.1",
                          Optional color$ = "YlGnBu:c8",
                          Optional levels% = 30,
                          Optional env As Environment = Nothing) As Object

        Dim errors As [Variant](Of Tolerance, Message) = Math.getTolerance(tolerance, env)

        If errors Like GetType(Message) Then
            Return errors.TryCast(Of Message)
        End If

        If mz.IsNullOrEmpty Then
            Return Nothing
        ElseIf mz.Length = 1 Then
            Return viewer.DrawLayer(
                mz:=mz(Scan0),
                threshold:=threshold,
                pixelSize:=InteropArgumentHelper.getSize(pixelSize, "5,5"),
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels
            )
        Else
            Return viewer.DrawLayer(
                mz:=mz,
                threshold:=threshold,
                pixelSize:=InteropArgumentHelper.getSize(pixelSize, "5,5"),
                toleranceErr:=errors.TryCast(Of Tolerance).GetScript,
                colorSet:=color,
                mapLevels:=levels
            )
        End If
    End Function

    ''' <summary>
    ''' flatten image layers
    ''' </summary>
    ''' <param name="layers">
    ''' layer bitmaps should be all in equal size
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("flatten")>
    Public Function flatten(layers As Bitmap(), Optional bg$ = "white") As Bitmap
        Using g As Graphics2D = New Bitmap(layers(Scan0).Width, layers(Scan0).Height)
            If Not bg.StringEmpty Then
                Call g.Clear(bg.GetBrush)
            End If

            ' 在这里是反向叠加图层的
            ' 向量中最开始的图层表示为最上层的图层，即最后进行绘制的图层
            For Each layer As Bitmap In layers.Reverse
                Call g.DrawImageUnscaled(layer, New Point)
            Next

            Return g.ImageResource
        End Using
    End Function
End Module
