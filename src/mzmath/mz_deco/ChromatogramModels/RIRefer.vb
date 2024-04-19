Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' the retention index reference
''' </summary>
Public Class RIRefer : Implements INamedValue, IReadOnlyId, IRetentionIndex, IRetentionTime, IMs1

    Public Property name As String Implements INamedValue.Key, IReadOnlyId.Identity
    Public Property mz As Double Implements IMs1.mz
    Public Property rt As Double Implements IMs1.rt

    ''' <summary>
    ''' the reference retention index value
    ''' </summary>
    ''' <returns></returns>
    Public Property RI As Double Implements IRetentionIndex.RI

    Public Overrides Function ToString() As String
        Return $"m/z {mz}, {rt} sec, " & name
    End Function



End Class