#Region "Microsoft.VisualBasic::85fad33928e52b05991f7d16ee02bf7f, Rscript\Library\mzkit_app\src\mzplot\ChemicalDraw.vb"

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

    '   Total Lines: 30
    '    Code Lines: 27
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 1.17 KB


    ' Module ChemicalDraw
    ' 
    '     Function: AsKCF, DrawKCF
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Drawing
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("ChemicalDraw")>
Module ChemicalDraw

    <ExportAPI("KCFDraw")>
    Public Function DrawKCF(molecule As KCF) As GraphicsData
        Return molecule.Draw
    End Function

    <ExportAPI("as.kcf")>
    <RApiReturn(GetType(KCF))>
    Public Function AsKCF(chemical As Object, Optional env As Environment = Nothing) As Object
        If chemical Is Nothing Then
            Return Internal.debug.stop("the given chemical data is nothing!", env)
        ElseIf TypeOf chemical Is ChemicalFormula Then
            Return DirectCast(chemical, ChemicalFormula).ToKCF
        Else
            Return Internal.debug.stop(New NotImplementedException(chemical.GetType.FullName), env)
        End If
    End Function
End Module
