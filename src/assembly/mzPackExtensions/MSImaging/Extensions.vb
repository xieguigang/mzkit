#If NET48 Then
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.Math

Public Module Extensions

    Public Function CheckMatrixBaseIon(fileName As String) As (nscans As Integer, ion As Double)
        Dim Xraw As New MSFileReader(fileName)
        Dim n As Integer = Xraw.ScanMax
        Dim allscans = New XRawStream(Xraw).StreamTo
        Dim basePeak As Double = allscans.MS _
            .Select(Function(a) a.GetMs.OrderByDescending(Function(i) i.intensity).FirstOrDefault) _
            .Where(Function(mzi) Not mzi Is Nothing) _
            .GroupBy(Function(x) x.mz, offsets:=0.3) _
            .OrderByDescending(Function(ni) ni.Length) _
            .First _
            .name _
            .ParseDouble

        Return (n, basePeak)
    End Function
End Module
#End If  