Imports System.Runtime.CompilerServices

Namespace Formula

    Module HeteroatomRatioCheck

        <Extension>
        Public Function HeteroatomRatioCheck(formula As Formula) As Boolean
            Dim C As Integer = formula!C

            If C = 0 Then
                ' 不支持这种检查规则，则跳过
                Return True
            End If

            If formula!H / C > 6 Then Return False
            If formula!F / C > 6 Then Return False
            If formula!Cl / C > 2 Then Return False
            If formula!Br / C > 2 Then Return False
            If formula!N / C > 4 Then Return False
            If formula!O / C > 3 Then Return False
            If formula!P / C > 2 Then Return False
            If formula!S / C > 3 Then Return False
            If formula!Si / C > 1 Then Return False

            Return True
        End Function
    End Module
End Namespace