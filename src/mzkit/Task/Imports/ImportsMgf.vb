Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF

Public Class ImportsMgf : Inherits ImportsTask(Of MGF.Ions)

    Protected Overrides Function getScans(source As String) As IEnumerable(Of Ions)
        Return MGF.ReadIons(source)
    End Function
End Class
