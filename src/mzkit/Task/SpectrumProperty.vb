#Region "Microsoft.VisualBasic::08325352f4028d8530ee562d41ec2974, src\mzkit\Task\SpectrumProperty.vb"

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

    ' Class SpectrumProperty
    ' 
    '     Properties: centroided, collisionEnergy, msLevel, precursorMz, retentionTime
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.netCDF.Components

Public Class SpectrumProperty

    Public Property msLevel As Integer
    Public Property collisionEnergy As String
    Public Property centroided As String
    Public Property precursorMz As Double
    Public Property retentionTime As Double
    Public Property precursorCharge As Double
    Public Property polarity As String
    Public Property activationMethod As String

    Sub New(attrs As attribute())
        With attrs.ToDictionary(Function(a) a.name, Function(a) a.value)
            msLevel = .TryGetValue(NameOf(msLevel))
            collisionEnergy = .TryGetValue(NameOf(collisionEnergy))
            centroided = .TryGetValue(NameOf(centroided))
            precursorMz = Val(.TryGetValue(NameOf(precursorMz))).ToString("F4")
            retentionTime = Val(.TryGetValue(NameOf(retentionTime))).ToString("F2")
            precursorCharge = Val(.TryGetValue("precursorCharge"))
            polarity = .TryGetValue("polarity", [default]:="n/a")
            activationMethod = .TryGetValue("activationMethod", [default]:="n/a")
        End With
    End Sub
End Class

