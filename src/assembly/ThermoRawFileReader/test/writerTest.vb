Imports ThermoFisher.CommonCore.Data.Business
Imports ThermoFisher.CommonCore.Data.Interfaces

Module writerTest

    Sub Main()
        Dim worker As New RawFileFactory
        Dim rawfile As IRawFileWriter = worker.OpenFile("./aaa.raw")

        rawfile.UpdateCreationDate(Now)

        Pause()
    End Sub
End Module
