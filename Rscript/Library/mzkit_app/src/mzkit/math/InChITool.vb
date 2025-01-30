#Region "Microsoft.VisualBasic::8ad24b7b06fe41956decff761e4208b5, Rscript\Library\mzkit_app\src\mzkit\math\InChITool.vb"

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

'   Total Lines: 75
'    Code Lines: 24 (32.00%)
' Comment Lines: 44 (58.67%)
'    - Xml Docs: 88.64%
' 
'   Blank Lines: 7 (9.33%)
'     File Size: 3.71 KB


' Module InChITool
' 
'     Function: get_formula, get_struct, inchikey, parse_inchi
' 
' /********************************************************************************/

#End Region


Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.IUPAC.InChI
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF.Models
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' InChI (International Chemical Identifier) string parser
''' </summary>
<Package("InChI")>
Module InChITool

    ''' <summary>
    ''' parse the inchi string 
    ''' </summary>
    ''' <param name="inchi"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Parsing an InChI (International Chemical Identifier) string can be a complex task due to the detailed and structured format of the InChI. 
    ''' However, the InChI format is designed to be readable by both humans and machines, and it can be broken down into several layers that 
    ''' represent different aspects of the chemical substance.
    ''' 
    ''' Here's a basic guide on how to manually parse an InChI string:
    ''' 
    ''' 1. **InChI Version**: The InChI string starts with the version number, followed by a slash. For example, `InChI=1S/`.
    ''' 2. **Formula Layer**: This is the chemical formula of the compound. It starts after the version and is enclosed in parentheses. For example, 
    '''    `InChI=1S/C6H12O6/` indicates that the compound is composed of 6 carbon, 12 hydrogen, and 6 oxygen atoms.
    ''' 3. **Main Layer**: This layer describes the connectivity of atoms in the molecule and is followed by a slash. It can include:
    '''    - Atom connectivity (`c`)
    '''    - Hydrogen atoms (`h`)
    '''    - Charge (`q`)
    '''    - Stereochemistry (`b`, `t`, `m`)
    ''' 4. **Stereochemical Layer**: This layer provides information about the stereochemistry of the compound if applicable. It is also followed by a slash.
    ''' 5. **Isotopic Layer**: If the compound contains isotopes, this layer will provide that information, followed by a slash.
    ''' 6. **Fixed-H Layer**: This layer indicates the positions of non-exchangeable hydrogen atoms and is followed by a slash.
    ''' 7. **Reconnected Layer**: In cases where the molecule can be disconnected into multiple subunits, this layer will describe the reconnected
    '''    form of the molecule.
    ''' 8. **Charge Layer**: This layer indicates the net electric charge of the compound.
    ''' 9. **Auxiliary Information**: Sometimes additional information is provided in parentheses at the end of the InChI string.
    ''' 
    ''' Here's an example of an InChI string broken down into its components:
    ''' 
    ''' ```
    ''' InChI=1S/C6H12O6/c7-1-2-3(8)4(9)5(10)6(11)12-1/h1-11H/t2-6+/m1/s1
    ''' ```
    ''' 
    ''' - `InChI=1S/` indicates the InChI version.
    ''' - `C6H12O6/` is the formula layer.
    ''' - `c7-1-2-3(8)4(9)5(10)6(11)` is the main layer with atom connectivity.
    ''' - `h1-11H` indicates the presence of hydrogen atoms.
    ''' - `t2-6+` indicates the stereochemistry.
    ''' - `/m1/s1` provides additional stereochemical information.
    ''' </remarks>
    <ExportAPI("parse")>
    Public Function parse_inchi(inchi As String) As InChI
        Return New InChI(inchi)
    End Function

    <ExportAPI("get_formula")>
    Public Function get_formula(inchi As InChI) As Formula
        Return FormulaScanner.ScanFormula(inchi.Main.Formula)
    End Function

    <ExportAPI("get_struct")>
    Public Function get_struct(inchi As InChI) As [Structure]
        Return inchi.GetStruct
    End Function

    ''' <summary>
    ''' get inchi hash key object from the given inchi object
    ''' </summary>
    ''' <param name="inchi"></param>
    ''' <returns></returns>
    <ExportAPI("inchikey")>
    Public Function inchikey(inchi As InChI) As InChIKey
        Return inchi.InChIKey
    End Function

    ''' <summary>
    ''' generates the inchikey hashcode based on the given inchi data
    ''' </summary>
    ''' <param name="inchi"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("eval_inchikey")>
    <RApiReturn(GetType(InChIKey))>
    Public Function inchikey(<RRawVectorArgument> inchi As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of String, InChIKey)(inchi, eval:=AddressOf IUPAC.InChI.MakeHashCode)
    End Function

End Module

