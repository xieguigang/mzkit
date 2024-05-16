#Region "Microsoft.VisualBasic::c80f8340d893527506ab6d29d957a9ab, assembly\SpectrumTree\ClusterTypes.vb"

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

    '   Total Lines: 29
    '    Code Lines: 6
    ' Comment Lines: 21
    '   Blank Lines: 2
    '     File Size: 866 B


    ' Enum ClusterTypes
    ' 
    '     [Default], Binary, Pack
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' the constant value for enumerate the reference library data types
''' </summary>
Public Enum ClusterTypes
    ''' <summary>
    ''' default library data type is the spectrum cluster tree,
    ''' this enumeration value is equaliviant to the 
    ''' <see cref="Tree"/>
    ''' </summary>
    [Default]

    ' 20230406
    ' ArgumentException: An item with the same key has already been added. Key: Tree

    ''' <summary>
    ''' default library data type is the spectrum cluster tree, 
    ''' this enumeration value is equaliviant to the 
    ''' <see cref="[Default]"/>
    ''' </summary>
    Tree ' = [Default]
    ''' <summary>
    ''' spectrum clustering in binary tree format
    ''' </summary>
    Binary
    ''' <summary>
    ''' spectrum packed with the same metabolite id 
    ''' </summary>
    Pack
End Enum
