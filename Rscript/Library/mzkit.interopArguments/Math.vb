Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime.Internal.Object

Module Math

    Public Function getTolerance(val As Object, env As Environment) As [Variant](Of Tolerance, Message)
        If val Is Nothing Then
            Return Tolerance.DefaultTolerance.DefaultValue
        ElseIf val.GetType.IsInheritsFrom(GetType(Tolerance)) Then
            Return val
        ElseIf val.GetType Is GetType(String) Then
            Return Tolerance.ParseScript(val)
        ElseIf val.GetType Is GetType(String()) Then
            Return Tolerance.ParseScript(DirectCast(val, String())(Scan0))
        ElseIf val.GetType Is GetType(vector) Then
            Return Tolerance.ParseScript(DirectCast(val, vector).data(Scan0))
        Else
            Return debug.stop(New NotImplementedException(val.GetType.FullName), env)
        End If
    End Function
End Module
