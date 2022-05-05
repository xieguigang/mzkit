Imports System.Text.RegularExpressions
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
        Static namePattern1 As New Regex("[a-zA-Z0-9]+\(.+\)")
        Static namePattern2 As New Regex("[a-zA-Z0-9]+\s+.+")

        Dim className As String
        Dim components As String

        If name.IsPattern(namePattern1) Then
            Dim tokens = name.GetTagValue("(", trim:=True)

            className = tokens.Name
            components = tokens.Value
            components = components.Substring(0, components.Length - 1)
        Else
            Dim tokens = name.GetTagValue(" ", trim:=True)

            className = tokens.Name
            components = tokens.Value.Trim
        End If

        Return New LipidName With {
            .className = className,
            .chains = ChainParser(components).ToArray
        }
    End Function

    Private Shared Iterator Function ChainParser(components As String) As IEnumerable(Of Chain)

    End Function

End Class
