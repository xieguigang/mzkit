Imports System.Drawing
Imports ControlLibrary
Imports Microsoft.VisualBasic.Imaging

Module Module1

    Sub Main()
        Dim image As Image = "E:\mzkit\src\mzkit\splash.PNG".LoadImage()
        Dim blur = image.RunUnsafeImageGenerationCode(BlurLevel:=50)

        Call blur.SaveAs("./blur.png")

        Pause()
    End Sub

End Module
