Imports Microsoft.VisualBasic.Data.IO.netCDF.Components

Public Class SpectrumProperty

    Public Property msLevel As Integer
    Public Property collisionEnergy As String
    Public Property centroided As String
    Public Property precursorMz As Double
    Public Property retentionTime As Double

    Sub New(attrs As attribute())
        With attrs.ToDictionary(Function(a) a.name, Function(a) a.value)
            msLevel = .TryGetValue(NameOf(msLevel))
            collisionEnergy = .TryGetValue(NameOf(collisionEnergy))
            centroided = .TryGetValue(NameOf(centroided))
            precursorMz = Val(.TryGetValue(NameOf(precursorMz))).ToString("F4")
            retentionTime = Val(.TryGetValue(NameOf(retentionTime))).ToString("F2")
        End With
    End Sub
End Class
