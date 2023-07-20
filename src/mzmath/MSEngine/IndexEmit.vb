Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations

Public Class IndexEmit

    ReadOnly no_arg As ConstructorInfo
    ReadOnly mass_arg As ConstructorInfo
    ReadOnly type As Type

    Sub New(schema As Type)
        no_arg = ParseNoArgument(schema)
        mass_arg = ParseMassArgument(schema)
        type = schema
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
        Dim writeMap As InterfaceMapping = type.GetInterfaceMap(GetType(IExactMassProvider))

        Return Function()
                   Dim obj As IExactMassProvider = Activator.CreateInstance(type)

               End Function
    End Function

    Private Function delegate_mass_argument() As Object
        Return Function(mass As Double)
                   Return Activator.CreateInstance(type, mass)
               End Function
    End Function

    Public Function CreateActivator() As Object
        If no_arg IsNot Nothing Then
            Return delegate_no_argument()
        ElseIf mass_arg IsNot Nothing Then
            Return delegate_mass_argument()
        Else
            Throw New InvalidProgramException("No suitable data interface could be found!")
        End If
    End Function
End Class
