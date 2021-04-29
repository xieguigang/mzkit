Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects

Public Module RawStream

    <Extension>
    Public Function LoadFromXRaw(raw As MSFileReader) As mzPack
        Dim pack As New mzPack

        Call raw.LoadFile()

        For Each scan As RawLabelData In raw.GetLabelData

        Next

        Return pack
    End Function
End Module
