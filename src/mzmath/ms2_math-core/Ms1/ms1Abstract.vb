
Public Interface IMs1 : Inherits IRetentionTime

    Property mz As Double

End Interface

Public Interface IRetentionTime

    ''' <summary>
    ''' Rt in seconds
    ''' </summary>
    ''' <returns></returns>
    Property rt As Double

End Interface

''' <summary>
''' [mz, rt, intensity]
''' </summary>
Public Interface IMs1Scan : Inherits IMs1

    Property intensity As Double

End Interface
