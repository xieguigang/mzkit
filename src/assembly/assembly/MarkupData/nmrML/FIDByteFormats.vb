Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Http

Namespace MarkupData.nmrML

    ''' <summary>
    ''' https://github.com/nmrML/nmrML/blob/99b854e7dc61ca85fcae16a068691dde160fa012/tools/Parser_and_Converters/R/nmRIO/R/readNMRMLFID.R#L31
    ''' </summary>
    Public Module FIDByteFormats

        <Extension>
        Public Function DecodeBytes(fidData As fidData) As Double()
            Select Case fidData.byteFormat
                Case "Complex128", "Complex64"
                    Return fidData.DecodeDouble
                Case "Integer32", "Complex32int", "class java.lang.Integer"
                    Return fidData.DecodeInteger
                Case Else
                    Throw New NotImplementedException(fidData.byteFormat)
            End Select
        End Function

        <Extension>
        Public Function DecodeDouble(fidData As fidData) As Double()
            Using bytes As MemoryStream = Convert.FromBase64String(fidData.base64).UnZipStream(noMagic:=True)
                Return bytes.ToArray _
                    .Split(8) _
                    .Select(Function(byts)
                                Return BitConverter.ToDouble(byts, Scan0)
                            End Function) _
                    .ToArray
            End Using
        End Function

        <Extension>
        Public Function DecodeInteger(fidData As fidData) As Double()
            Using bytes As MemoryStream = Convert.FromBase64String(fidData.base64).UnZipStream(noMagic:=True)
                Return bytes.ToArray _
                    .Split(4) _
                    .Select(Function(byts)
                                Return CDbl(BitConverter.ToInt32(byts, Scan0))
                            End Function) _
                    .ToArray
            End Using
        End Function
    End Module
End Namespace