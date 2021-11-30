Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal

Module Module1

    Sub Main()
        FrameworkInternal.ConfigMemory(MemoryLoads.Heavy)

        Dim demo As String = "F:\20211123_CDF\P210702366.netcdf"
        Dim gcxgc = netCDFReader.Open(demo).ToMzPack

        Using file As Stream = "F:\20211123_CDF\P210702366.mzpack".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call gcxgc.Write(file)
        End Using

        Pause()
    End Sub
End Module
