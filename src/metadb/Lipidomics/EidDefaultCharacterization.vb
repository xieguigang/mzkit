#Region "Microsoft.VisualBasic::3b8acbf706d0384426e6ff34ff9f1264, G:/mzkit/src/metadb/Lipidomics//EidDefaultCharacterization.vb"

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


    ' Code Statistics:

    '   Total Lines: 45
    '    Code Lines: 30
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 3.00 KB


    ' Class EidDefaultCharacterization
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Characterize4AlkylAcylGlycerols, Characterize4Ceramides, Characterize4DiacylGlycerols, Characterize4MonoacylGlycerols, Characterize4SingleAcylChain
    '               Characterize4TriacylGlycerols
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine


Public NotInheritable Class EidDefaultCharacterization
    Private Sub New()
    End Sub

    Public Shared Function Characterize4AlkylAcylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
        Return GetDefaultCharacterizationResultForAlkylAcylGlycerols(molecule, defaultResult)
    End Function

    Public Shared Function Characterize4Ceramides(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
        Return GetDefaultCharacterizationResultForCeramides(molecule, defaultResult)
    End Function

    Public Shared Function Characterize4DiacylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
        Return GetDefaultCharacterizationResultForGlycerophospholipid(molecule, defaultResult)
    End Function

    Public Shared Function Characterize4MonoacylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
        Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
    End Function

    Public Shared Function Characterize4SingleAcylChain(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 0, 0.5)
        Return GetDefaultCharacterizationResultForSingleAcylChainLipid(molecule, defaultResult)
    End Function


    Public Shared Function Characterize4TriacylGlycerols(scan As IMSScanProperty, molecule As ILipid, reference As MoleculeMsReference, tolerance As Single, mzBegin As Single, mzEnd As Single) As (ILipid, Double())

        Dim defaultResult = EieioMsCharacterizationUtility.GetDefaultScore(scan, reference, tolerance, mzBegin, mzEnd, 1, 0, 1, 0.5)
        Return GetDefaultCharacterizationResultForTriacylGlycerols(molecule, defaultResult)
    End Function
End Class
