Imports Microsoft.VisualBasic.MIME.application.netCDF.Components
Imports SMRUCC.MassSpectrum.Math
Imports SMRUCC.MassSpectrum.Math.Chromatogram

Public Class GCMSJson

    Public Property times As Double()
    Public Property tic As Double()
    Public Property ms As ms1_scan()()

    Public Function GetTIC() As IEnumerable(Of ChromatogramTick)

    End Function
End Class
