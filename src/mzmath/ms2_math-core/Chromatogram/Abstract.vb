Namespace Chromatogram

    Public Interface IChromXs
        Property RT As RetentionTime
        Property RI As RetentionIndex
        Property Drift As DriftTime
        Property Mz As MzValue
        Property MainType As ChromXType
    End Interface

    Public Interface IChromX
        ReadOnly Property Value As Double
        ReadOnly Property Type As ChromXType
        ReadOnly Property Unit As ChromXUnit
    End Interface

End Namespace