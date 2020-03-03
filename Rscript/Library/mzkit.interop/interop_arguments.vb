Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1

Module interop_arguments

    Public Function GetTolerance(obj As Object, Optional default$ = "ppm:20") As Tolerance
        If obj Is Nothing Then
            Return Tolerance.ParseScript([default])
        End If

        Select Case obj.GetType
            Case GetType(String())
                Return Tolerance.ParseScript(DirectCast(obj, String())(Scan0))
            Case GetType(String)
                Return Tolerance.ParseScript(CStr(obj))
            Case GetType(PPMmethod), GetType(DAmethod)
                Return obj
            Case Else
                Return Tolerance.ParseScript([default])
        End Select
    End Function
End Module
