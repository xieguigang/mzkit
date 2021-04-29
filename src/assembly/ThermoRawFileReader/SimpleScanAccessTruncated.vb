Imports ThermoFisher.CommonCore.Data.Interfaces

Friend Class SimpleScanAccessTruncated
    Implements ISimpleScanAccess

    Public ReadOnly Property Masses As Double() Implements ISimpleScanAccess.Masses
    Public ReadOnly Property Intensities As Double() Implements ISimpleScanAccess.Intensities

    Public Sub New(masses As Double(), intensities As Double())
        Me.Masses = masses
        Me.Intensities = intensities
    End Sub
End Class