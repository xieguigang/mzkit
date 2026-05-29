#Region "Microsoft.VisualBasic::edd56a3bdfab360868c18d8d4ac77c49, Rscript\Library\mzkit_app\src\mzkit\comprehensive\z_assembler.vb"

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

    '   Total Lines: 162
    '    Code Lines: 110 (67.90%)
    ' Comment Lines: 34 (20.99%)
    '    - Xml Docs: 91.18%
    ' 
    '   Blank Lines: 18 (11.11%)
    '     File Size: 6.22 KB


    ' Module z_assembler
    ' 
    '     Function: z_assembler, z_header, z_volume
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.File
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' package for processing the ms-imaging 3D data assembly
''' </summary>
<Package("z_assembler")>
Public Module z_assembler

    ''' <summary>
    ''' create the volume file header data
    ''' </summary>
    ''' <param name="features">the ion features m/z vector</param>
    ''' <param name="mzdiff"></param>
    ''' <returns></returns>
    <ExportAPI("z_header")>
    Public Function z_header(<RRawVectorArgument> features As Object, Optional mzdiff As Double = 0.001) As MatrixHeader
        Return New MatrixHeader With {
            .matrixType = FileApplicationClass.MSImaging3D,
            .tolerance = mzdiff.ToString,
            .mz = CLRVector.asNumeric(features),
            .numSpots = 0
        }
    End Function

    ''' <summary>
    ''' Create mzpack object for ms-imaging in 3D
    ''' 
    ''' this function assembling a collection of the 2D layer in z-axis
    ''' order for construct a new 3D volume data.
    ''' </summary>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' a <see cref="MzMatrix"/> object will be packed as the 3D volumn result.
    ''' </remarks>
    <ExportAPI("z_assembler")>
    <RApiReturn(GetType(ZAssembler))>
    Public Function z_assembler(header As MatrixHeader, file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        Else
            Dim z As New ZAssembler(buf.TryCast(Of Stream))
            Call z.WriteHeader(header)
            Return New z_assembler_func With {
                .assembler = z
            }
        End If
    End Function

    ''' <summary>
    ''' create a simple 3d volume model for mzkit workbench
    ''' </summary>
    ''' <param name="layers">should be a collection of the <see cref="SingleIonLayer"/>. 
    ''' the layer elements in this collection should be already been re-ordered by 
    ''' the z-axis!</param>
    ''' <param name="dump"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function works for combine a collection of the 2D layer as the 3d volume <see cref="MzMatrix"/> data
    ''' </remarks>
    <ExportAPI("z_volume")>
    Public Function z_volume(<RRawVectorArgument> layers As Object, dump As String, Optional env As Environment = Nothing) As Object
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of SingleIonLayer)(layers, env)

        If pull.isError Then
            Return pull.getError
        End If

        Dim layers_2d As SingleIonLayer() = pull _
            .populates(Of SingleIonLayer)(env) _
            .ToArray
        ' check dimension size for all layers
        Dim assert_dims As Size = layers_2d(0).DimensionSize
        Dim check_mismatched = layers_2d _
            .Where(Function(l)
                       Return l.DimensionSize.Width <> assert_dims.Width OrElse
                           l.DimensionSize.Height <> assert_dims.Height
                   End Function) _
            .Select(Function(a) a.IonMz) _
            .ToArray

        If check_mismatched.Any Then
            Call env.AddMessage({
                $"there are {check_mismatched.Length} 2d layer dimension value is mis-matched!",
                $"layers: {check_mismatched.GetJson}"
            })
        End If

        Dim dimx As Integer = assert_dims.Width
        Dim dimy As Integer = assert_dims.Height
        Dim dimz As Integer = layers_2d.Length

        Call {dimx, dimy, dimz}.GetJson.SaveTo($"{dump}/dims.json")

        Dim s As Stream = $"{dump}/data.dat".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
        Dim bin As New BinaryDataWriter(s, Encoding.ASCII)
        Dim byter As New DoubleRange(0, 255)
        ' global intensity range
        Dim intensityr As DoubleRange = layers_2d _
            .AsParallel _
            .Select(Function(l) l.GetIntensity.Range) _
            .Select(Iterator Function(r) As IEnumerable(Of Double)
                        Yield r.Min
                        Yield r.Max
                    End Function) _
            .IteratesALL _
            .ToArray

        For Each layer As SingleIonLayer In TqdmWrapper.Wrap(layers_2d)
            Dim buf As Byte()() = RectangularArray.Matrix(Of Byte)(
                assert_dims.Height,
                assert_dims.Width
            )
            Dim v As Double

            For Each spot As MsImaging.PixelData In layer.MSILayer
                v = intensityr.ScaleMapping(spot.intensity, byter)

                If v < 0 Then
                    v = 0
                ElseIf v > 255 Then
                    v = 255
                End If

                buf(spot.y - 1)(spot.x - 1) = CByte(v)
            Next

            For Each line As Byte() In buf
                Call bin.Write(line)
            Next
        Next

        Call bin.Flush()
        Call s.Flush()
        Call s.Dispose()

        Return True
    End Function
End Module
