Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components

Public Class ImportsMgf : Inherits ImportsTask(Of MGF.Ions)

    Public Sub New(source As String)
        MyBase.New(source)
    End Sub

    Protected Overrides Function getScans(source As String) As IEnumerable(Of Ions)
        Return MGF.ReadIons(source)
    End Function

    Protected Overrides Function getAttributes(scan As Ions) As attribute()
        Throw New NotImplementedException()
    End Function

    Protected Overrides Function getPeaks(scan As Ions) As ms2()
        Throw New NotImplementedException()
    End Function
End Class
