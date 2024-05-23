#Region "Microsoft.VisualBasic::25a64716a7f5fe1965d12d7e0553c16b, assembly\SpectrumTree\Pack\MassIndex.vb"

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

    '   Total Lines: 44
    '    Code Lines: 19 (43.18%)
    ' Comment Lines: 19 (43.18%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (13.64%)
    '     File Size: 1.52 KB


    '     Class MassIndex
    ' 
    '         Properties: exactMass, formula, name, size, spectrum
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.Annotations
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PackLib

    ''' <summary>
    ''' A metabolite its spectrum data index
    ''' </summary>
    ''' <remarks>
    ''' A tree node of a specific metabolite reference to multiple spectrum data
    ''' </remarks>
    Public Class MassIndex : Implements INamedValue, IExactMassProvider

        ''' <summary>
        ''' the unique reference of current metabolite spectrum cluster
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String Implements INamedValue.Key
        Public Property exactMass As Double Implements IExactMassProvider.ExactMass
        Public Property formula As String
        ''' <summary>
        ''' the pointer to the spectrum data in the library file
        ''' </summary>
        ''' <returns></returns>
        Public Property spectrum As New List(Of Integer)

        ''' <summary>
        ''' the number of the spectrum that associated with current
        ''' metabolite <see cref="name"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property size As Integer
            Get
                Return spectrum.Count
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
