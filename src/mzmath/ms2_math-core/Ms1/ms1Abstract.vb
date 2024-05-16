#Region "Microsoft.VisualBasic::513c4bfe298730f9578de27b31024aca, mzmath\ms2_math-core\Ms1\ms1Abstract.vb"

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

    '   Total Lines: 63
    '    Code Lines: 16
    ' Comment Lines: 32
    '   Blank Lines: 15
    '     File Size: 1.46 KB


    ' Interface IMs1
    ' 
    '     Properties: mz
    ' 
    ' Interface IRetentionTime
    ' 
    '     Properties: rt
    ' 
    ' Interface IRetentionIndex
    ' 
    '     Properties: RI
    ' 
    ' Interface IMs1Scan
    ' 
    '     Properties: intensity
    ' 
    ' Interface IMS1Annotation
    ' 
    '     Properties: precursor_type
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

''' <summary>
''' [mz, rt] tuple
''' </summary>
Public Interface IMs1 : Inherits IRetentionTime

    Property mz As Double

End Interface

''' <summary>
''' A time point related data
''' </summary>
Public Interface IRetentionTime

    ''' <summary>
    ''' Rt in seconds
    ''' </summary>
    ''' <returns></returns>
    Property rt As Double

End Interface

''' <summary>
''' A time point related data
''' </summary>
Public Interface IRetentionIndex

    ''' <summary>
    ''' the retention index
    ''' </summary>
    ''' <returns></returns>
    Property RI As Double

End Interface

''' <summary>
''' [mz, rt, intensity]
''' </summary>
Public Interface IMs1Scan : Inherits IMs1

    Property intensity As Double

End Interface

''' <summary>
''' the abstract annotation result model of a ion, this abstract interface model 
''' contains data properties:
''' 
''' 1. the entity key: is the unique reference id of the targetd annotated metabolite in the source database
''' 2. the ion adducts precursor type: the adducts type of the target annotated metabolite convert to the 
'''    given ion mz data
''' </summary>
Public Interface IMS1Annotation : Inherits IMs1Scan, INamedValue

    ''' <summary>
    ''' the ion annotated result
    ''' </summary>
    ''' <returns></returns>
    Property precursor_type As String

End Interface
