﻿#Region "Microsoft.VisualBasic::16452bc74559931395666b8be6e1db32, assembly\assembly\MarkupData\imzML\Writer\DataWriter.vb"

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

    '   Total Lines: 41
    '    Code Lines: 31 (75.61%)
    ' Comment Lines: 3 (7.32%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (17.07%)
    '     File Size: 1.28 KB


    '     Module DataWriter
    ' 
    '         Function: WriteArray, WriteMzPack
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Data.IO

Namespace MarkupData.imzML

    ''' <summary>
    ''' the ibd data writer
    ''' </summary>
    Module DataWriter

        <Extension>
        Public Function WriteMzPack(mzpack As ScanMS1, ibd As BinaryDataWriter, ionMode As IonModes) As ScanData
            Dim pixel As Point = mzpack.GetMSIPixel
            Dim scan As New ScanData With {
                .x = pixel.X,
                .y = pixel.Y,
                .totalIon = mzpack.into.Sum,
                .MzPtr = WriteArray(mzpack.mz, ibd),
                .IntPtr = WriteArray(mzpack.into, ibd),
                .polarity = ionMode
            }

            Return scan
        End Function

        Private Function WriteArray(array As Double(), ibd As BinaryDataWriter) As ibdPtr
            Dim offset As Long = ibd.Position
            Dim bytesize As Long

            ibd.Write(array)
            bytesize = ibd.Position - offset

            Return New ibdPtr With {
                .offset = offset,
                .arrayLength = array.Length,
                .encodedLength = bytesize
            }
        End Function
    End Module
End Namespace
