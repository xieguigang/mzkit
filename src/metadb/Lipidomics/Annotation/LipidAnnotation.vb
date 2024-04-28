#Region "Microsoft.VisualBasic::2c73ac6e27ffd6dc72411c0fb7b78260, E:/mzkit/src/metadb/Lipidomics//Annotation/LipidAnnotation.vb"

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

    '   Total Lines: 25
    '    Code Lines: 23
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 1.17 KB


    ' Class LipoqualityAnnotation
    ' 
    '     Properties: Adduct, AveragedIntensity, Formula, Inchikey, Intensities
    '                 IonMode, LipidClass, LipidSuperClass, Mz, Name
    '                 Rt, Smiles, Sn1AcylChain, Sn2AcylChain, Sn3AcylChain
    '                 Sn4AcylChain, SpotID, StandardDeviation, TotalChain
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS


Public Class LipoqualityAnnotation
    Public Property Mz As Single
    Public Property Rt As Single
    Public Property AveragedIntensity As Single
    Public Property Name As String = String.Empty
    Public Property LipidClass As String = String.Empty
    Public Property LipidSuperClass As String = String.Empty
    Public Property TotalChain As String = String.Empty
    Public Property Sn1AcylChain As String = String.Empty
    Public Property Sn2AcylChain As String = String.Empty
    Public Property Sn3AcylChain As String = String.Empty
    Public Property Sn4AcylChain As String = String.Empty
    Public Property Adduct As AdductIon
    Public Property IonMode As IonModes
    Public Property Smiles As String = String.Empty
    Public Property Inchikey As String = String.Empty
    Public Property StandardDeviation As Single
    Public Property SpotID As Integer
    Public Property Formula As String = String.Empty
    Public Property Intensities As List(Of Double) = New List(Of Double)()
End Class
