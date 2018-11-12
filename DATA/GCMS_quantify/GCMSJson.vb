Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Public Class GCMSJson

    Public Property name As String
    Public Property times As Double()
    Public Property tic As Double()
    Public Property ms As ms1_scan()()

    Public Function GetTIC() As NamedCollection(Of ChromatogramTick)

    End Function
End Class
