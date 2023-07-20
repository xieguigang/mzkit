Imports System.Reflection
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations

Public Class IndexEmit

    ReadOnly no_arg As ConstructorInfo
    ReadOnly mass_arg As ConstructorInfo
    ReadOnly type As Type

    Public ReadOnly Property [delegate] As Type

    Sub New(schema As Type)
        no_arg = ParseNoArgument(schema)
        mass_arg = ParseMassArgument(schema)
        type = schema
        [delegate] = GetType(Func(Of,  )).MakeGenericType(GetType(Double), schema)
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
        Dim target = writeMap.TargetMethods(0)
        ' get_xxx
        Dim delp As PropertyInfo = type.GetProperty(target.Name.Substring(4))
        Dim del As Func(Of Double, Object) =
            Function(m As Double)
                Dim obj As IExactMassProvider = Activator.CreateInstance(type)
                delp.SetValue(obj, m)
                Return delp
            End Function

        Return del
    End Function

    Private Function delegate_mass_argument() As Object
        Dim del As Func(Of Double, Object) =
            Function(mass As Double)
                Return Activator.CreateInstance(type, mass)
            End Function

        Return del
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
