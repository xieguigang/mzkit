Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Public Class LipidName

    Public Property className As String
    Public Property chains As Chain()

    Public Overrides Function ToString() As String
        Return ToSystematicName()
    End Function

    Public Function ToSystematicName() As String

    End Function

    Public Function ToOverviewName() As String

    End Function

    ''' <summary>
    ''' parse lipid name components
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Shared Function ParseLipidName(name As String) As LipidName

    End Function

End Class
