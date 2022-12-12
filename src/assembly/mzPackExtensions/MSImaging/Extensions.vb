#If NET48 Then
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.Math

Public Module Extensions

    Public Function CheckMatrixBaseIon(fileName As String) As (nscans As Integer, ion As Double)
        Dim n As Integer
        Dim allscans As mzPack

        Select Case fileName.ExtensionSuffix.ToLower
            Case "raw"
                Dim Xraw As New MSFileReader(fileName)

                n = Xraw.ScanMax
                allscans = New XRawStream(Xraw).StreamTo
            Case "wiff"
                Dim wiffRaw As New WiffScanFileReader(fileName)
                Dim println As Action(Of String) = AddressOf Console.WriteLine

                allscans = wiffRaw.LoadFromWiffRaw(checkNoise:=False, println:=println)
                n = allscans.size
            Case Else
                Throw New NotImplementedException(fileName.ExtensionSuffix)
        End Select

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