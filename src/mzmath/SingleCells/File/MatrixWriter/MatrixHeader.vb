﻿#Region "Microsoft.VisualBasic::303977a35870ad94a118407080d1150a, mzmath\SingleCells\File\MatrixWriter\MatrixHeader.vb"

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

    '   Total Lines: 59
    '    Code Lines: 21 (35.59%)
    ' Comment Lines: 29 (49.15%)
    '    - Xml Docs: 96.55%
    ' 
    '   Blank Lines: 9 (15.25%)
    '     File Size: 2.21 KB


    '     Class MatrixHeader
    ' 
    '         Properties: featureSize, matrixType, mz, mzmax, mzmin
    '                     numSpots, tolerance
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute

Namespace File

    Public Class MatrixHeader : Implements IMassSet

        ''' <summary>
        ''' m/z vector in numeric format of round to digit 4, this ion m/z 
        ''' feature list is generated under the current mass 
        ''' <see cref="tolerance"/>.
        ''' </summary>
        ''' <returns></returns>
        Public Property mz As Double() Implements IMassSet.mass
        Public Property mzmin As Double() Implements IMassSet.min
        Public Property mzmax As Double() Implements IMassSet.max

        ''' <summary>
        ''' the script string of the mz diff tolerance for <see cref="mz"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property tolerance As String

        ''' <summary>
        ''' get count of the ion feature size under current mass <see cref="tolerance"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property featureSize As Integer
            Get
                Return mz.TryCount
            End Get
        End Property

        ''' <summary>
        ''' number of the spots
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' number of the rows in <see cref="MzMatrix.matrix"/>
        ''' </remarks>
        Public Property numSpots As Integer

        ''' <summary>
        ''' the matrix data type of current object, value of this property could be one of the flag value:
        ''' 
        ''' 1. <see cref="FileApplicationClass.MSImaging"/> 2d spatial data
        ''' 2. <see cref="FileApplicationClass.MSImaging3D"/> 3d spatial data
        ''' 3. <see cref="FileApplicationClass.SingleCellsMetabolomics"/> single cell matrix data
        ''' </summary>
        ''' <returns></returns>
        Public Property matrixType As FileApplicationClass

        Public Overrides Function ToString() As String
            Return matrixType.ToString
        End Function

    End Class
End Namespace
