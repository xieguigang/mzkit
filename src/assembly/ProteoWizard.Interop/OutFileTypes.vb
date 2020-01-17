
Imports System.ComponentModel

Public Enum OutFileTypes
    ''' <summary>
    ''' write mzXML format
    ''' </summary>
    <Description("--mzXML")> mzXML
    ''' <summary>
    ''' write mzML format [default]
    ''' </summary>
    <Description("--mzML")> mzML
    ''' <summary>
    ''' write mz5 format
    ''' </summary>
    <Description("--mz5")> mz5
    ''' <summary>
    ''' write Mascot generic format
    ''' </summary>
    <Description("--mgf")> mgf
    ''' <summary>
    ''' write ProteoWizard internal text format
    ''' </summary>
    <Description("--text")> text
    ''' <summary>
    ''' write MS1 format
    ''' </summary>
    <Description("--ms1")> ms1
    ''' <summary>
    ''' write CMS1 format
    ''' </summary>
    <Description("--cms1")> cms1
    ''' <summary>
    ''' write MS2 format
    ''' </summary>
    <Description("--ms2")> ms2
    ''' <summary>
    ''' write CMS2 format
    ''' </summary>
    <Description("--cms2")> cms2
End Enum
