Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text

Namespace Deconvolute

    ''' <summary>
    ''' a data matrix object in format of row is pixel and 
    ''' column is mz intensity value across different 
    ''' pixels
    ''' </summary>
    Public Class MzMatrix

        ''' <summary>
        ''' m/z vector in numeric format of round to digit 4 
        ''' </summary>
        ''' <returns></returns>
        Public Property mz As Double()

        ''' <summary>
        ''' the script string of the mz diff tolerance for <see cref="mz"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property tolerance As String

        ''' <summary>
        ''' MS-imaging pixel data matrix or the 
        ''' cells data matrix in a single cells raw data 
        ''' </summary>
        ''' <returns></returns>
        Public Property matrix As PixelData()

        Public Function ExportCsvSheet(file As Stream) As Boolean
            Using text As New StreamWriter(file, Encodings.ASCII.CodePage) With {
                .NewLine = vbLf
            }
                Call text.WriteLine("Pixels," & mz.JoinBy(","))

                For Each pixelLine As String In matrix _
                    .AsParallel _
                    .Select(Function(pixel)
                                Return $"""{pixel.X},{pixel.Y}"",{pixel.intensity.JoinBy(",")}"
                            End Function)

                    Call text.WriteLine(pixelLine)
                Next

                Call text.Flush()
            End Using

            Return True
        End Function
    End Class
End Namespace