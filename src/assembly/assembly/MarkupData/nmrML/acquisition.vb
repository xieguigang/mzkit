Imports System.IO
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Http

Namespace MarkupData.nmrML

    Public Class acquisition

        Public Property acquisitionMultiD As acquisitionMultiD
        Public Property acquisition1D As acquisitionMultiD

        Public Function ParseMatrix() As LibraryMatrix
            If acquisition1D Is Nothing Then
                Return acquisitionMultiD.ParseMatrix
            Else
                Return acquisition1D.ParseMatrix
            End If
        End Function

    End Class

    Public Class acquisitionMultiD

        ''' <summary>
        ''' Free Induction Decay
        ''' </summary>
        ''' <returns></returns>
        Public Property fidData As fidData

        Public Function ParseMatrix() As LibraryMatrix
            Dim FID As New List(Of ms2)

            Using bytes As New BinaryDataReader(Convert.FromBase64String(fidData.base64).UnZipStream(noMagic:=True))
                Dim n As Integer = bytes.Length / 16
                Dim values As Double()
                Dim freq As ms2

                If n * 16 <> bytes.Length Then
                    Throw New InvalidDataException
                End If

                bytes.ByteOrder = ByteOrder.LittleEndian
                bytes.Seek(Scan0, SeekOrigin.Begin)

                For i As Integer = 0 To n - 1
                    values = bytes.ReadDoubles(2)
                    freq = New ms2 With {
                        .mz = values(Scan0),
                        .intensity = values(1)
                    }

                    FID.Add(freq)
                Next
            End Using

            Return New LibraryMatrix With {
                .ms2 = FID.ToArray
            }
        End Function

    End Class

    ''' <summary>
    ''' The signal we detect is called a Free Induction Decay (FID). 
    ''' The FID is produced by the macroscopic magnetization after 
    ''' the pulse. The magnetization will undergo several processes 
    ''' as it returns to equilibrium.
    ''' </summary>
    Public Class fidData

        <XmlAttribute> Public Property byteFormat As String
        <XmlAttribute> Public Property compressed As String
        <XmlAttribute> Public Property encodedLength As Integer

        <XmlText>
        Public Property base64 As String

    End Class
End Namespace