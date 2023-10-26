Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace XMass

    ''' <summary>
    ''' XMASS, Bruker-Daltonics project reader
    ''' </summary>
    Public Class Project

        Public Property sptype As String
        Public Property pdata As pdata()
        Public Property acqu As NamedValue(Of String())()
        Public Property acqus As NamedValue(Of String())()
        Public Property AnalysisParameter As AnnotationParameter
        Public Property source As String

        Public Shared Function FromResultFolder(dir As String) As Project
            Dim acqu = PropertyFileReader.ReadData($"{dir}/acqu".OpenReader).ToArray
            Dim acqus = PropertyFileReader.ReadData($"{dir}/acqus".OpenReader).ToArray
            Dim method As AnalysisMethod = $"{dir}/Analysis.FAmethod".LoadXml(Of AnalysisMethod)
            Dim parms As AnnotationParameter = ($"{dir}/AnalysisParameter.xml") _
                .ReadAllText _
                .CreateObjectFromXmlFragment(Of AnnotationParameter)
            Dim pdataList As New List(Of pdata)

            For Each pdatadir As String In $"{dir}/pdata".ListDirectory
                Call pdataList.Add(XMass.pdata.LoadFolder(pdatadir))
            Next

            Return New Project With {
                .sptype = $"{dir}/sptype".ReadAllText,
                .acqu = acqu,
                .acqus = acqus,
                .AnalysisParameter = parms,
                .pdata = pdataList.ToArray,
                .source = dir
            }
        End Function
    End Class
End Namespace