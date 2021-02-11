#Region "Microsoft.VisualBasic::7dd65482c7a593dd7eee6e945a2d910a, Massbank\Public\NCBI\PubChem\PCXml\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: LoadFromXml, TrimXml
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text

Namespace NCBI.PubChem.PCCompound

    <HideModuleName>
    Public Module Extensions

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadFromXml(xml As String) As Compound
            Return Compound.LoadFromXml(xml)
        End Function

        Friend Function TrimXml(xml As String) As String
            With New StringBuilder(xml)
                Call .Replace("PC-Compound_", "")
                Call .Replace("PC-Count_", "")
                Call .Replace("PC-CompoundType_", "")
                Call .Replace("PC-Atoms_", "")
                Call .Replace("PC-AtomInt_", "")
                Call .Replace("PC-Bonds_", "")
                Call .Replace("PC-StereoTetrahedral_", "")
                Call .Replace("PC-Coordinates_", "")
                Call .Replace("PC-Conformer_", "")
                Call .Replace("PC-DrawAnnotations_", "")

                Return .ToString
            End With
        End Function
    End Module
End Namespace
