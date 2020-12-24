Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO

Public Class BlockNode

    Public Property scan0 As Long
    Public Property left As BlockNode
    Public Property right As BlockNode

    Public Iterator Function ReadNode(file As Stream) As IEnumerable(Of PeakMs2)
        Dim infile As New BinaryDataReader(file)
        Dim size As Long
        Dim nspectra As Integer

        infile.Seek(scan0, SeekOrigin.Begin)
        size = infile.ReadInt64
        infile.ReadString(BinaryStringFormat.ZeroTerminated)
        nspectra = infile.ReadInt32

        For i As Integer = 0 To nspectra - 1
            Yield Reader.ReadSpectra(infile)
        Next
    End Function

End Class
