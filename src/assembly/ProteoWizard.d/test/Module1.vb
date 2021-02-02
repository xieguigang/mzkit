#Region "Microsoft.VisualBasic::6be537031ea316a6b840f04cea5c25e5, ProteoWizard.d\test\Module1.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:

    ' Module Module1
    ' 
    '     Sub: Main, testExtractZip
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip

Module Module1

    Sub Main()

        Call testExtractZip()


    End Sub

    Sub testExtractZip()
        Dim zip1 = "C:\Users\administrator\Desktop\95-1.D.zip"
        Dim test1 = ZipLib.IsSourceFolderZip(zip1)

        Dim zip2 = "C:\Users\administrator\Desktop\95-1.D-flat.zip"
        Dim test2 = ZipLib.IsSourceFolderZip(zip2)

        Call UnZip.ImprovedExtractToDirectory(zip1, "./ddddddd", extractToFlat:=True)

        Call UnZip.ImprovedExtractToDirectory(zip1, "./eeeeee/", extractToFlat:=False)


        Call UnZip.ImprovedExtractToDirectory(zip2, "./xxxxxx", extractToFlat:=True)
        Call UnZip.ImprovedExtractToDirectory(zip2, "./ZZZZZZZZZ", extractToFlat:=False)

        Pause()
    End Sub

End Module
