Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace XMass

    Public Class pdata

        Public Property peaklist As pklist
        Public Property proc As NamedValue(Of String())()
        Public Property procs As NamedValue(Of String())()
        Public Property id As String

        Public Overrides Function ToString() As String
            Return id
        End Function

        Public Shared Function LoadFolder(dir As String) As pdata
            Dim pklist = $"{dir}/peaklist.xml".LoadXml(Of pklist)
            Dim proc = PropertyFileReader.ReadData($"{dir}/proc".OpenReader).ToArray
            Dim procs = PropertyFileReader.ReadData($"{dir}/procs".OpenReader).ToArray
            Dim id As String = dir.BaseName

            Return New pdata With {
                .peaklist = pklist,
                .proc = proc,
                .procs = procs,
                .id = id
            }
        End Function
    End Class
End Namespace