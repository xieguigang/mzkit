Imports System.Runtime.CompilerServices
Imports SMRUCC.MassSpectrum.Assembly.MarkupData
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Spectra

Public Class SpectrumTree : Inherits Spectra.SpectrumTreeCluster

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(Optional compares As Comparison(Of PeakMs2) = Nothing, Optional showReport As Boolean = True)
        Call MyBase.New(compares, showReport)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
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

        Return New SpectrumTree(showReport:=True).doCluster(allMs2Scans, file.FileName)
    End Function
End Class
