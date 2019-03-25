Imports SMRUCC.MassSpectrum.Assembly.MarkupData
Imports SMRUCC.MassSpectrum.Math

Public Class SpectrumTree : Inherits Spectra.SpectrumTreeCluster

    Sub New(showReport As Boolean)
        Call MyBase.New(showReport)
    End Sub

    Public Overloads Function doCluster(ms2list As mzXML.scan(), fileName$) As SpectrumTree
        Return doCluster(ms2list _
            .Select(Function(scan) scan.ScanData(fileName)) _
            .OrderBy(Function(ms) ms.rt) _
            .ToArray
        )
    End Function

    Public Shared Function CreateTreeFromFile(file As String) As SpectrumTree
        Dim allMs2Scans = mzXML.XML _
            .LoadScans(file) _
            .Where(Function(s) s.msLevel = "2") _
            .OrderBy(Function(mz) mz.precursorMz) _
            .ToArray

        Return New SpectrumTree(True).doCluster(allMs2Scans, file.FileName)
    End Function
End Class
