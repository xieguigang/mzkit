#Region "Microsoft.VisualBasic::, src\mzkit\ControlLibrary\MSI\MSIRenderTask.vb"

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

' /********************************************************************************/

#End Region

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices

Module GaussTask

    <DllImport("Gauss.CPP.dll", CallingConvention:=CallingConvention.Cdecl, EntryPoint:="ComputeGaussBlur")>
    Private Sub ComputeGaussBlurCpp(threadParameters As ThreadParameters)
    End Sub

    Private Function ComputeNumberOfLinesPerThread(ByVal threadId As Integer, ByVal numOfThreads As Integer, ByVal gaussMaskSize As Integer, ByVal imgHeight As Integer) As Integer
        Dim numOfLinesPerThread = imgHeight / numOfThreads

        If numOfThreads > 1 Then
            If threadId = 0 OrElse threadId = numOfThreads - 1 Then
                numOfLinesPerThread += gaussMaskSize / 2
            Else
                numOfLinesPerThread += gaussMaskSize - 1
            End If
        End If

        Return numOfLinesPerThread
    End Function

    Private Function ComputeThreadParams(ByVal threadId As Integer, ByVal generatorParams As GeneratorParameters, ByVal imageSizes As Size(Of Integer)) As ThreadParameters
        Dim rowPadded As Integer = imageSizes.Width * 3 + 3 And Not 3
        Dim currentThreadImgHeight = 0
        Dim sumOfOffsetLines = 0

        For i = 0 To threadId
            Dim numOfLinesOfCurrentThread = ComputeNumberOfLinesPerThread(threadId:=i, numOfThreads:=generatorParams.NumberOfThreads, gaussMaskSize:=generatorParams.GaussMaskSize, imgHeight:=imageSizes.Height)

            If i = threadId Then
                currentThreadImgHeight = numOfLinesOfCurrentThread
            Else
                sumOfOffsetLines += numOfLinesOfCurrentThread - (generatorParams.GaussMaskSize - 1)
            End If
        Next

        Return New ThreadParameters With {
            .CurrentImgOffset = sumOfOffsetLines * rowPadded,
            .GaussMaskSize = generatorParams.GaussMaskSize,
            .ImgWidth = imageSizes.Width,
            .ImgHeight = currentThreadImgHeight,
            .IdOfImgPart = threadId,
            .NumOfImgParts = generatorParams.NumberOfThreads
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="currentThreadParams"></param>
    Public Sub RunUnsafeImageGenerationCode(currentThreadParams As ThreadParameters, image As Image)
        Dim rowPadded = (currentThreadParams.ImgWidth * 3 + 3) And Not 3
        Dim tmpArray = New Byte(currentThreadParams.ImgHeight * rowPadded - 1) {}
        Dim buffer As New MemoryStream

        Call image.Save(buffer, ImageFormat.Bmp)

        Dim bytes As Byte() = buffer.ToArray
        Dim imgArrayPtr = Marshal.ReadIntPtr(bytes, 0)
        Dim tmpArrayPtr = Marshal.ReadIntPtr(tmpArray, 0)

        currentThreadParams.ImgByteArrayPtr = imgArrayPtr + 54
        currentThreadParams.TempImgByteArrayPtr = tmpArrayPtr

        System.Console.WriteLine("Start {0}", currentThreadParams)

        ComputeGaussBlurCpp(currentThreadParams)

    End Sub

End Module

Public Class GeneratorParameters

    Public Property NumberOfThreads As Integer
    Public Property BlurLevel As Integer
    Public Property GaussMaskSize As Integer

    Public Overrides Function ToString() As String
        Return String.Format("NumOfThreads: {0}, BlurLvl: {1}", NumberOfThreads, BlurLevel)
    End Function
End Class

<StructLayout(LayoutKind.Sequential)>
Public Structure ThreadParameters

    Public ProcessId As Integer
    Public GaussMaskSize As Integer
    Public CurrentImgOffset As Integer
    Public ImgWidth As Integer
    Public ImgHeight As Integer
    Public IdOfImgPart As Integer
    Public NumOfImgParts As Integer
    Public ImgByteArrayPtr As IntPtr
    Public TempImgByteArrayPtr As IntPtr

    Public Overrides Function ToString() As String
        Return {
            $"ProcessID: {IdOfImgPart}; ",
            $"ThreadID: {ImgWidth}; ",
            $"Width: {ImgHeight}; ",
            $"Height: {NumOfImgParts}; ",
            $"NumOfParts: {CurrentImgOffset}; ",
            $"ThreadOffset: {ImgByteArrayPtr.ToInt32()}; ",
            $"ImgPtr: {TempImgByteArrayPtr.ToInt32()}; ",
            $"TempImgPtr: {ProcessId}"
        }.JoinBy(" ")
    End Function
End Structure

Public Structure Size(Of T As {Structure, System.IConvertible})

    Public Property Width As T
    Public Property Height As T

    Public Sub New(width As T, height As T)
        Me.Width = width
        Me.Height = height
    End Sub

    Public Function GetLoadedImageSizes(buffer As Byte()) As Size(Of Integer)
        Dim width, height As Integer

        width = BitConverter.ToInt32(buffer, 18)
        height = BitConverter.ToInt32(buffer, 22)

        Return New Size(Of Integer)(width, height)
    End Function
End Structure