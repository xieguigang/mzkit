#Region "Microsoft.VisualBasic::c0355a8f48249a90cf4d01ca52b8747e, mzmath\ms2_math-core\Chromatogram\Abstract.vb"

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

    '   Total Lines: 17
    '    Code Lines: 14 (82.35%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (17.65%)
    '     File Size: 464 B


    '     Interface IChromXs
    ' 
    '         Properties: Drift, MainType, Mz, RI, RT
    ' 
    '     Interface IChromX
    ' 
    '         Properties: Type, Unit, Value
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Chromatogram

    Public Interface IChromXs
        Property RT As RetentionTime
        Property RI As RetentionIndex
        Property Drift As DriftTime
        Property Mz As MzValue
        Property MainType As ChromXType
    End Interface

    Public Interface IChromX
        ReadOnly Property Value As Double
        ReadOnly Property Type As ChromXType
        ReadOnly Property Unit As ChromXUnit
    End Interface

End Namespace
