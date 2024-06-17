#Region "Microsoft.VisualBasic::d93e99b82fc40da724d659448213a5b9, metadb\Massbank\MetaLib\RefMet.vb"

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

'   Total Lines: 54
'    Code Lines: 16 (29.63%)
' Comment Lines: 34 (62.96%)
'    - Xml Docs: 91.18%
' 
'   Blank Lines: 4 (7.41%)
'     File Size: 3.14 KB


'     Class RefMet
' 
'         Properties: exactmass, formula, inchi_key, main_class, pubchem_cid
'                     refmet_name, smiles, sub_class, super_class
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace MetaLib

    ''' <summary>
    ''' RefMet: A Reference set of Metabolite names
    ''' </summary>
    ''' <remarks>
    ''' RefMet Naming Conventions
    ''' 
    ''' The names used in RefMet are generally based on common, officially accepted terms and 
    ''' incorporate notations which are appropriate for the type of analytical technique used.
    ''' In general, high-throughput untargeted MS experiments are not capable of deducing 
    ''' stereochemistry, double bond position/geometry and sn position (for glycerolipids/
    ''' lycerophospholipids). Secondly, the type of MS technique employed, as well as the mass
    ''' accuacy of the instrument will produce identifications at different levels of detail. 
    ''' For example, MS/MS methods are capable of identifying acyl chain substituents in lipids 
    ''' (e.g. PC 16:0/20:4) whereas MS methods only using precursor ion information might report 
    ''' these ions as "sum-composition" species (e.g. PC 36:4). RefMet covers both types of 
    ''' notations in an effort to enable data-sharing and comparative analysis of metabolomics 
    ''' data, using an analytical chemistry-centric approach.
    ''' 
    ''' The "sum-composition" lipid species indicate the number Of carbons And number Of "double
    ''' bond equivalents" (DBE), but Not chain positions Or Double bond regiochemistry And geometry.
    ''' The concept Of a Double bond equivalent unites a range Of chemical functionality which 
    ''' gives rise To isobaric features by mass spectometry. For example a chain containing a ring 
    ''' results In loss Of 2 hydrogen atoms (compared To a linear Structure) And thus has 1 DBE 
    ''' since the mass And molecular formula Is identical To a linear Structure With one Double bond.
    ''' Similarly, conversion Of a hydroxyl group To ketone results In loss Of 2 hydrogen atoms,
    ''' therefore the ketone Is assigned 1 DBE. Where applicable, the number Of oxygen atoms Is added 
    ''' To the abbreviation, separated by a semi-colon. Oxygen atoms In the Class-specific functional
    ''' group (e.g. the carboxylic acid group For fatty acids Or the phospholcholine group For PC) 
    ''' are excluded. In the Case Of sphingolipids, all oxygen atoms apart from the amide oxygen are
    ''' included, In order To discrminate, For example, between 1-deoxyceramides (;O), ceramides (;O2) 
    ''' And phytoceramides (;O3).
    ''' 
    ''' Some notes pertaining To different metabolite classes are outlined below.
    ''' </remarks>
    Public Class RefMet : Implements IReadOnlyId, IExactMassProvider, ICompoundNameProvider, IFormulaProvider, INamedValue

        Public Property refmet_name As String Implements IReadOnlyId.Identity, ICompoundNameProvider.CommonName, INamedValue.Key
        Public Property super_class As String
        Public Property main_class As String
        Public Property sub_class As String
        Public Property formula As String Implements IFormulaProvider.Formula
        Public Property exactmass As Double Implements IExactMassProvider.ExactMass
        Public Property inchi_key As String
        Public Property smiles As String
        Public Property pubchem_cid As String

        Public Overrides Function ToString() As String
            Return refmet_name
        End Function

    End Class
End Namespace
