
Namespace DataObjects

    ''' <summary>
    ''' Type for Mass Precision Information
    ''' </summary>
    <CLSCompliant(True)>
    Public Structure MassPrecisionInfoType
        ''' <summary>
        ''' Peak Intensity
        ''' </summary>
        Public Intensity As Double

        ''' <summary>
        ''' Peak Mass
        ''' </summary>
        Public Mass As Double

        ''' <summary>
        ''' Peak Accuracy (in MMU)
        ''' </summary>
        Public AccuracyMMU As Double

        ''' <summary>
        ''' Peak Accuracy (in PPM)
        ''' </summary>
        Public AccuracyPPM As Double

        ''' <summary>
        ''' Peak Resolution
        ''' </summary>
        Public Resolution As Double
    End Structure
End Namespace