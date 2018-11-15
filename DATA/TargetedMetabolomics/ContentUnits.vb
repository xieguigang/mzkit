Imports Microsoft.VisualBasic.ComponentModel.Ranges

Public Enum ContentUnits As Integer
    ppm = ppb * 1000
    ppb = ppt * 1000
    ppt = 1
End Enum

Public Module UnitExtensions

    Public Function ppm2ppb(ppm As Double) As Double
        Return ppm.Unit(ContentUnits.ppm).ScaleTo(ContentUnits.ppb).Value
    End Function
End Module