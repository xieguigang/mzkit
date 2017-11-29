Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Public Module ImageFly

    Public Function GetImage(cid$, Optional width% = 500, Optional height% = 500) As Bitmap
        Dim url$ = $"https://pubchem.ncbi.nlm.nih.gov/image/imagefly.cgi?cid={cid}&width={width}&height={height}"
        Dim tmp$ = App.GetAppSysTempFile(".png", sessionID:=App.PID)

        If Not url.DownloadFile(save:=tmp) Then
            Return Nothing
        End If

        Dim bitmap As Bitmap = New Bitmap(tmp) _
            .CorpBlank(margin:=5, blankColor:=Color.White) _
            .ColorReplace(Color.White, Color.Transparent)
        Return bitmap
    End Function
End Module
