Namespace MarkupData.imzML

    ''' <summary>
    ''' TIC/BPC/Average
    ''' </summary>
    Public Enum IntensitySummary As Integer
        ''' <summary>
        ''' sum all intensity signal value in a pixel
        ''' </summary>
        Total
        ''' <summary>
        ''' get a max intensity signal value in a pixel
        ''' </summary>
        BasePeak
        ''' <summary>
        ''' get the average intensity signal value in a pixel
        ''' </summary>
        Average
        Median
    End Enum
End Namespace