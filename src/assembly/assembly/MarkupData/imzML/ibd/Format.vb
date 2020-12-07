
Namespace MarkupData.imzML

    ''' <summary>
    ''' In order to insure efficient storage, two different 
    ''' formats of the binary data are defined: continuous 
    ''' and processed. 
    ''' </summary>
    Public Enum Format
        ''' <summary>
        ''' Continuous type means that each spectrum of an image has the 
        ''' same m/z values. As a result the m/z array is only saved once 
        ''' directly behind the UUID of the file and the intensity arrays 
        ''' of the spectra are following. 
        ''' </summary>
        Continuous
        ''' <summary>
        ''' At the processed type every spectrum has its own m/z array. 
        ''' So it is necessary to save both – the m/z array and the 
        ''' corresponding intensity array – per spectrum.
        ''' </summary>
        Processed
    End Enum
End Namespace