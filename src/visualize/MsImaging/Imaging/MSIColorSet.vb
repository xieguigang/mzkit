Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Namespace Imaging

    Public Module MSIColorSet

        ReadOnly scales As String()

        Sub New()
            scales = {
                "#070711",
                "#110071",
                "#1202a2",
                "#185eff",
                "#11d6ea",
                "#20fdda",
                "#11f9a6",
                "#10f97a",
                "#3df31b",
                "#bbf50c",
                "#e7f116",
                "#f9bb1b",
                "#f17408",
                "#e23f00",
                "#f50961",
                "#f410a3",
                "#e333d1",
                "#f87bfd",
                "#ffffff"
            }
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetColors() As Color()
            Return (From str As String In scales Select str.TranslateColor).ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub DoRegister()
            Call Designer.Register("MSImaging", GetColors)
        End Sub
    End Module
End Namespace