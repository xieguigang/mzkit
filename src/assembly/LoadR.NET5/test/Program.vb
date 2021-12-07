Imports System
Imports System.IO
Imports SMRUCC.Rsharp.RDataSet

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")

        Using buffer As Stream = "F:\PlantMAT.rda".Open
            Dim data = Reader.ParseData(buffer)

            Pause()
        End Using
    End Sub
End Module
