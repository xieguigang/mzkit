Module Includes

    ReadOnly _defaultPaths = {
        App.HOME & "\XRawfile2_x64.dll",                           ' Theoretical Bundled 64 bit MSFileReader DLL
        App.CurrentDirectory & "\XRawfile2_x64.dll",
        "C:\Program Files\Thermo\MSFileReader\XRawfile2_x64.dll",  ' Default Installation Path on 64 bit systems
        "C:\Program Files (x86)\Thermo\MSFileReader\XRawfile2.dll",
        "C:\Program Files\Thermo\MSFileReader\XRawfile2.dll"       ' Default Installation Path on 32 bit systems
    }

    Public Declare Function Open Lib "XRawfile2_x64.dll" (szFileName As String) As Integer
End Module
