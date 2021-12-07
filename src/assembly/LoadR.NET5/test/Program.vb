Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry
Imports Microsoft.VisualBasic.My
Imports SMRUCC.Rsharp.RDataSet
Imports SMRUCC.Rsharp.RDataSet.Convertor
Imports SMRUCC.Rsharp.RDataSet.Struct.LinkedList

Module Program
    Sub Main(args As String())
        Console.WriteLine("Hello World!")

        FrameworkInternal.ConfigMemory(FrameworkInternal.MemoryLoads.Heavy)

        Using buffer As Stream = "D:\mzkit\src\assembly\LoadR.NET5\PeaksMs2.rda".Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim data = Reader.ParseData(buffer).object.LinkVisitor("x")
            Dim list As RObject() = data.value.CAR.value.data
            Dim peaksData = list.Select(AddressOf LoadR.LoadPeakMs2).ToArray

            Pause()
        End Using
    End Sub
End Module
