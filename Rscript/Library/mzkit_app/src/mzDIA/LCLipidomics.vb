#Region "Microsoft.VisualBasic::c73d0511a8a72cd04b9218cc604197be, G:/mzkit/Rscript/Library/mzkit_app/src/mzDIA//LCLipidomics.vb"

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

    '   Total Lines: 50
    '    Code Lines: 27
    ' Comment Lines: 18
    '   Blank Lines: 5
    '     File Size: 2.10 KB


    ' Module LCLipidomics
    ' 
    '     Function: adductIon, GetLipidIons
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Lipidomics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Lipidomics annotation based on MS-DIAL
''' </summary>
<Package("lipidomics")>
Module LCLipidomics

    ''' <summary>
    ''' meansrue lipid ions
    ''' </summary>
    ''' <param name="lipidclass">configs of the target lipid <see cref="LbmClass"/> for run spectrum peaks generation</param>
    ''' <param name="adduct">a precursor adducts data which could be generates via the ``adduct`` function.</param>
    ''' <param name="minCarbonCount"></param>
    ''' <param name="maxCarbonCount"></param>
    ''' <param name="minDoubleBond"></param>
    ''' <param name="maxDoubleBond"></param>
    ''' <param name="maxOxygen"></param>
    ''' <returns>a collection of the <see cref="LipidIon"/> data</returns>
    <ExportAPI("lipid_ions")>
    <RApiReturn(GetType(LipidIon))>
    Public Function GetLipidIons(lipidclass As LbmClass,
                                 adduct As AdductIon,
                                 minCarbonCount As Integer,
                                 maxCarbonCount As Integer,
                                 minDoubleBond As Integer,
                                 maxDoubleBond As Integer,
                                 maxOxygen As Integer) As Object

        Dim precursor_type As AdductIon

        Return LipidMassLibraryGenerator _
            .GetIons(lipidclass, adduct, minCarbonCount, maxCarbonCount, minDoubleBond, maxDoubleBond, maxOxygen) _
            .ToArray
    End Function

    ''' <summary>
    ''' create the adduct ion data model
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("adduct")>
    <RApiReturn(GetType(AdductIon))>
    Public Function adductIon(<RRawVectorArgument> adduct As Object, Optional env As Environment = Nothing) As Object
        Throw New NotImplementedException
    End Function
End Module
