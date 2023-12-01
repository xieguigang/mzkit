Namespace MarkupData.imzML

    Public Interface IMSIPixel

        ReadOnly Property x As Integer
        ReadOnly Property y As Integer
        ReadOnly Property intensity As Double

    End Interface

    ''' <summary>
    ''' a unify model of imzml image pixel scan data
    ''' </summary>
    Public Interface ImageScan

        Property MzPtr As ibdPtr
        Property IntPtr As ibdPtr

    End Interface
End Namespace