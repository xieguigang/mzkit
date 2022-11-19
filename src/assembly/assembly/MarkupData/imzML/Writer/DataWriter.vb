Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.Data.IO

Namespace MarkupData.imzML

    ''' <summary>
    ''' the ibd data writer
    ''' </summary>
    Module DataWriter

        <Extension>
        Public Function WriteMzPack(mzpack As ScanMS1, ibd As BinaryDataWriter) As ScanData
            Dim pixel As Point = mzpack.GetMSIPixel
            Dim scan As New ScanData With {
                .x = pixel.X,
                .y = pixel.Y,
                .totalIon = mzpack.into.Sum,
                .MzPtr = WriteArray(mzpack.mz, ibd),
                .IntPtr = WriteArray(mzpack.into, ibd)
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