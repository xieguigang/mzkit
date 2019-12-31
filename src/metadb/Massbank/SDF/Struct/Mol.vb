#Region "Microsoft.VisualBasic::454aa71120c85d25d015b1ed34da361b, DATA\Massbank\SDF\Struct\Mol.vb"

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

    '     Class [Structure]
    ' 
    '         Properties: Atoms, Bounds
    ' 
    '         Function: Parse, parseCounter, ParseStream, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace File

    ''' <summary>
    ''' The molecular structure.
    ''' </summary>
    Public Class [Structure]

        Public Property Atoms As Atom()
        Public Property Bounds As Bound()

        Public Overrides Function ToString() As String
            Return $"{Atoms.Length} atoms with {Bounds.Length} bounds"
        End Function

        ''' <summary>
        ''' Next comes the so-called "counts" line. This line is made up of twelve fixed-length fields 
        ''' 
        ''' + the first eleven are three characters long, 
        ''' + and the last six characters long. 
        ''' 
        ''' The first two fields are the most critical, and give the number of atoms and bonds 
        ''' described in the compound.
        ''' 
        ''' ```
        '''   9  8  0     0  0  0  0  0  0999 V2000
        ''' ```
        ''' 
        ''' So this compound will have 9 atoms And 8 bonds described. Often, hydrogens - especially 
        ''' those attached To elements such As carbon Or oxygen - are left implicit (And will be 
        ''' included based On the available valences) rather than being included In the file.
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Private Shared Function parseCounter(line As String) As (counts As String(), version$)
            Dim version$ = Mid(line, line.Length - 6 + 1).Trim
            Dim t$()

            line = Mid(line, 1, line.Length - 6)
            t = line _
                .Split(partitionSize:=3) _
                .Select(Function(b) New String(b).Trim) _
                .ToArray

            Return (t, version)
        End Function

        ''' <summary>
        ''' 从分子结构文本数据之中解析出分子的结构模型数据
        ''' </summary>
        ''' <param name="mol">这个参数同时兼容文本内容或者文本文件的路径</param>
        ''' <returns></returns>
        Public Shared Function Parse(mol As String) As [Structure]
            Dim lines$() = mol _
                .SolveStream _
                .Trim(ASCII.CR, ASCII.LF) _
                .LineTokens

            Return ParseStream(lines)
        End Function

        Friend Shared Function ParseStream(lines As String()) As [Structure]
            Dim countLine = parseCounter(lines(Scan0))
            Dim [dim] = (
                atoms:=CInt(countLine.counts(0)),
                bounds:=CInt(countLine.counts(1))
            )
            Dim atoms = lines _
                .Skip(1) _
                .Take([dim].atoms) _
                .Select(AddressOf Trim) _
                .Select(AddressOf Atom.Parse) _
                .ToArray
            Dim bounds = lines _
                .Skip(1 + [dim].atoms) _
                .Take([dim].bounds) _
                .Select(AddressOf Trim) _
                .Select(AddressOf Bound.Parse) _
                .ToArray

            Return New [Structure] With {
                .Atoms = atoms,
                .Bounds = bounds
            }
        End Function
    End Class
End Namespace
