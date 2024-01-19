Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute

Public Class MatrixHeader

    ''' <summary>
    ''' m/z vector in numeric format of round to digit 4, this ion m/z 
    ''' feature list is generated under the current mass 
    ''' <see cref="tolerance"/>.
    ''' </summary>
    ''' <returns></returns>
    Public Property mz As Double()

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

End Class