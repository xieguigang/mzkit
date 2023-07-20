Imports System.Reflection

Public Class IndexEmit

    ReadOnly no_arg As ConstructorInfo
    ReadOnly mass_arg As ConstructorInfo

    Sub New(schema As Type)
        no_arg = ParseNoArgument(schema)
        mass_arg = ParseMassArgument(schema)
    End Sub

    Private Shared Function ParseNoArgument(schema As Type) As ConstructorInfo
        Return schema _
            .GetConstructors _
            .Where(Function(c) c.GetParameters.Length = 0) _
            .FirstOrDefault
    End Function

    Private Shared Function ParseMassArgument(schema As Type) As ConstructorInfo
        Return schema _
            .GetConstructors _
            .Where(Function(c)
                       Dim args = c.GetParameters

                       If args.Length <> 1 Then
                           Return False
                       End If

                       Return args(Scan0).ParameterType Is GetType(Double)
                   End Function) _
            .FirstOrDefault
    End Function

    Private Function delegate_no_argument() As Object

    End Function

    Private Function delegate_mass_argument() As Object

    End Function

    Public Function CreateActivator(schema As Type) As Object
        If no_arg IsNot Nothing Then
            Return delegate_no_argument()
        ElseIf mass_arg IsNot Nothing Then
            Return delegate_mass_argument()
        Else
            Throw New InvalidProgramException("No suitable data interface could be found!")
        End If
    End Function
End Class
