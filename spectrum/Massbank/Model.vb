Imports System.Runtime.CompilerServices
Imports SMRUCC.proteomics.MS_Spectrum.DATA.Massbank.DATA

Public Module Model

    <Extension>
    Public Function DBLinks(record As Record) As Dictionary(Of String, String())
        If record.CH Is Nothing OrElse record.CH.LINK.IsNullOrEmpty Then
            Return Nothing
        Else
            Dim links$() = record.CH.LINK
            Dim data = links.Select(Function(l) l.GetTagValue(" ", trim:=True)).ToArray
            Dim table As Dictionary(Of String, String()) = data _
                .ToDictionary(Function(l) l.Name,
                              Function(l) l.Value.StringSplit("\s+"))
            Return table
        End If
    End Function
End Module
