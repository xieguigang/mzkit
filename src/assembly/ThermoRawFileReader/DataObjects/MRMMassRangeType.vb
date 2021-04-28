
Namespace DataObjects

    ''' <summary>
    ''' Type for storing MRM Mass Ranges
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure MRMMassRangeType
        ''' <summary>
        ''' Start Mass
        ''' </summary>
        Public StartMass As Double

        ''' <summary>
        ''' End Mass
        ''' </summary>
        Public EndMass As Double

        ''' <summary>
        ''' Central Mass
        ''' </summary>
        Public CentralMass As Double      ' Useful for MRM/SRM experiments

        ''' <summary>
        ''' Return a summary of this object
        ''' </summary>
        Public Overrides Function ToString() As String
            Return StartMass.ToString("0.000") & "-" & EndMass.ToString("0.000")
        End Function
    End Structure
End Namespace