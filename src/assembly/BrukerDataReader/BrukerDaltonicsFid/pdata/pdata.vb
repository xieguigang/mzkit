Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class pdata

    Public Property peaklist As pklist
    Public Property proc As NamedValue(Of String())()
    Public Property procs As NamedValue(Of String())()

    Public Shared Function LoadFolder(dir As String) As pdata
        Dim pklist = $"{dir}/peaklist.xml".LoadXml(Of pklist)
        Dim proc = PropertyFileReader.ReadData($"{dir}/proc".OpenReader).ToArray
        Dim procs = PropertyFileReader.ReadData($"{dir}/procs".OpenReader).ToArray

        Return New pdata With {
            .peaklist = pklist,
            .proc = proc,
            .procs = procs
        }
    End Function

End Class
