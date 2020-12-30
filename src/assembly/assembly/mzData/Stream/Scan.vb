Imports BioNovoGene.Analytical.MassSpectrometry.Math

Namespace mzData.mzWebCache

    Public Class ScanMS2 : Inherits MSScan
        Implements IMs1

        Public Property parentMz As Double Implements IMs1.mz
        Public Property intensity As Double
        Public Property polarity As Integer
        Public Overrides Property rt As Double Implements IRetentionTime.rt

    End Class

    Public Class ScanMS1 : Inherits MSScan

        Public Property TIC As Double
        Public Property BPC As Double
        Public Property products As ScanMS2()

    End Class
End Namespace