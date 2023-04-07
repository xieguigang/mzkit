Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports SMRUCC.Rsharp.RDataSet
Imports SMRUCC.Rsharp.RDataSet.Convertor
Imports SMRUCC.Rsharp.RDataSet.Struct.LinkedList
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class XcmsRData

    Public Property mz As Double()
    Public Property rt As Double()
    Public Property into As Double()
    Public Property ms2 As Dictionary(Of String, ms2())

    Private Shared Function readNumeric(root As RObject, node As String) As Double()
        Dim query = root.LinkVisitor(node)
        Dim car As RObject = query.value.CAR
        Dim vec As Double() = RStreamReader.ReadNumbers(car)

        Return vec
    End Function

    Public Shared Function ReadRData(buffer As Stream) As XcmsRData
        ' mz1, rt2, into, ms2
        Dim root = Reader.ParseData(buffer).object
        Dim mz = readNumeric(root, "mz1")
        Dim rt = readNumeric(root, "rt2")
        Dim into = readNumeric(root, "into")
        Dim matrix As New Dictionary(Of String, ms2())
        Dim raw As list = ConvertToR.ToRObject(root.LinkVisitor("ms2"))

        For Each name As String In raw.getNames
            Dim vec As Double() = CLRVector.asNumeric(raw(name))
            Dim msms As ms2() = New ms2(CInt(vec.Length / 2) - 1) {}
            Dim tuple = vec.Split(msms.Length)

            For i As Integer = 0 To msms.Length - 1
                msms(i) = New ms2 With {
                    .mz = tuple(0)(i),
                    .intensity = tuple(1)(i)
                }
            Next

            Call matrix.Add(name, msms)
        Next

        Return New XcmsRData With {
            .mz = mz, .rt = rt, .into = into,
            .ms2 = matrix
        }
    End Function

End Class
