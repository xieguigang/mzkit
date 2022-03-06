Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.InteropServices

Namespace BrukerDataReader
    ' TODO: add apodization ability
    ' TODO: remove all dependence on DeconEngine (FFT, apodization, etc).

    Public Class DataReader
        Implements IDisposable
        ' Ignore Spelling: acqu, acqus, apodization, Bruker, fid, ser

        Private _numMSScans As Integer = -1
        Private _lastScanIndexOpened As Integer
        Private _reader As BinaryReader
        Private _previousStartPosition As Long
        Private _bytesAdvanced As Long
        Private ReadOnly _fourierTransform As FourierTransform = New FourierTransform()

        ''' <summary>
        ''' Constructor for the DataReader class
        ''' </summary>
        ''' <paramname="fileName">Refers to the binary file containing the mass spectra data. For Bruker data,
        ''' this is a 'ser' or a 'fid' file</param>
        ''' <paramname="settingsFilePath">Path to the acqus or apexAcquisition.method file that should be used for reading parameters</param>
        Public Sub New(ByVal fileName As String, ByVal Optional settingsFilePath As String = "")
            If File.Exists(fileName) Then
                Me.FileName = fileName
            Else
                Throw New FileNotFoundException("Dataset could not be opened. File not found.")
            End If

            ' Assure that the file can be opened
            Using New BinaryReader(File.Open(Me.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            End Using

            If String.IsNullOrEmpty(settingsFilePath) Then
                Parameters = New GlobalParameters()
            Else
                LoadParameters(settingsFilePath)
            End If
        End Sub

        Public Property Parameters As GlobalParameters
        Public ReadOnly Property FileName As String

        ''' <summary>
        ''' Load the parameters from an acqus file or apexAcquisition.method file
        ''' </summary>
        ''' <paramname="settingsFilePath"></param>
        Public Sub LoadParameters(ByVal settingsFilePath As String)
            Dim fiSettingsFile = New FileInfo(settingsFilePath)
            If Not fiSettingsFile.Exists Then Throw New FileNotFoundException("Settings file not found")
            Dim filenameLower = fiSettingsFile.Name.ToLower()
            Dim reader = New BrukerSettingsFileReader()

            Select Case filenameLower
                Case "acqu", "acqus"
                    Parameters = reader.LoadApexAcqusParameters(fiSettingsFile)
                Case "apexacquisition.method"
                    Parameters = reader.LoadApexAcqParameters(fiSettingsFile)
                Case Else
                    Throw New Exception("Unrecognized settings file (" & fiSettingsFile.Name & "); should be acqus or apexAcquisition.method")
            End Select
        End Sub

        Public Sub SetParameters(ByVal calA As Double, ByVal calB As Double, ByVal sampleRate As Double, ByVal numValuesInScan As Integer)
            Parameters = New GlobalParameters With {
                .ML1 = calA,
                .ML2 = calB,
                .SampleRate = sampleRate,
                .NumValuesInScan = numValuesInScan
            }
        End Sub

        Public Sub SetParameters(ByVal gp As GlobalParameters)
            Parameters = gp
        End Sub

        Public Function GetNumMSScans() As Integer
            ' If numMSScans was already stored, then return it
            If _numMSScans <> -1 Then
                Return _numMSScans
            End If

            ' Determine the number of scans using the file length

            Require(Parameters?.NumValuesInScan > 0, "Cannot determine number of MS Scans. Parameter for number of points in Scan has not been set.")

            Using reader = New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                Dim fileLength = reader.BaseStream.Length
                                ''' Cannot convert LocalDeclarationStatementSyntax, System.InvalidCastException: Unable to cast object of type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax' to type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax'.
'''    at ICSharpCode.CodeConverter.VB.CommonConversions.RemodelVariableDeclaration(VariableDeclarationSyntax declaration)
'''    at ICSharpCode.CodeConverter.VB.MethodBodyExecutableStatementVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
'''    at Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor`1.Visit(SyntaxNode node)
'''    at ICSharpCode.CodeConverter.VB.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)
''' 
''' Input:
'''                 var totalNumberOfValues = fileLength / sizeof(int);
''' 
''' 
                If Parameters IsNot Nothing Then _numMSScans = CInt(totalNumberOfValues / Parameters.NumValuesInScan)
            End Using

            Return _numMSScans
        End Function

        ''' <summary>
        ''' Gets the mass spectrum. Opens the BinaryReader and keeps it open between calls to this method.
        ''' Finds the correct scan by using a relative position within the reader
        ''' </summary>
        ''' <remarks>
        ''' Unit tests in 2010 showed this method to be 3% to 10% faster than GetMassSpectrum
        ''' </remarks>
        ''' <paramname="scanIndex">Zero-based scan index</param>
        ''' <paramname="mzValues">array of m/z values</param>
        ''' <paramname="intensities">Array of intensity values</param>
        ' ReSharper disable once UnusedMember.Global
        Public Sub GetMassSpectrumUsingSupposedlyFasterBinaryReader(ByVal scanIndex As Integer, <Out> ByRef mzValues As Single(), <Out> ByRef intensities As Single())
            Require(Parameters IsNot Nothing, "Cannot get mass spectrum. Need to first set Parameters.")
            Require(scanIndex < GetNumMSScans(), "Cannot get mass spectrum. Requested scan index is greater than number of scans in the dataset.")

            If _reader Is Nothing Then
                _reader = New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            End If

            If Parameters Is Nothing Then Throw New Exception("Parameters is null in GetMassSpectrumUsingSupposedlyFasterBinaryReader")
            Dim vals = New Double(Parameters.NumValuesInScan - 1) {}
            Dim diffBetweenCurrentAndPreviousScan = scanIndex - _lastScanIndexOpened
                        ''' Cannot convert LocalDeclarationStatementSyntax, System.InvalidCastException: Unable to cast object of type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax' to type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax'.
'''    at ICSharpCode.CodeConverter.VB.CommonConversions.RemodelVariableDeclaration(VariableDeclarationSyntax declaration)
'''    at ICSharpCode.CodeConverter.VB.MethodBodyExecutableStatementVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
'''    at Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor`1.Visit(SyntaxNode node)
'''    at ICSharpCode.CodeConverter.VB.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)
''' 
''' Input:
''' 
'''             var byteOffset = diffBetweenCurrentAndPreviousScan * (long)this.Parameters.NumValuesInScan * sizeof(int) - this._bytesAdvanced;
''' 
''' 
            If byteOffset <> 0 Then
                _reader.BaseStream.Seek(byteOffset, SeekOrigin.Current)
            End If

            _previousStartPosition = _reader.BaseStream.Position

            For i = 0 To Parameters.NumValuesInScan - 1
                vals(i) = _reader.ReadInt32()
            Next

            _bytesAdvanced = _reader.BaseStream.Position - _previousStartPosition
            Dim lengthOfMZAndIntensityArray = Parameters.NumValuesInScan / 2
            Dim mzValuesFullRange = New Single(lengthOfMZAndIntensityArray - 1) {}
            Dim intensitiesFullRange = New Single(lengthOfMZAndIntensityArray - 1) {}
            _fourierTransform.RealFourierTransform(vals)

            For i = 0 To lengthOfMZAndIntensityArray - 1
                Dim mz = CSng(GetMZ(i))
                Dim intensity = CSng(Math.Sqrt(vals(2 * i + 1) * vals(2 * i + 1) + vals(2 * i) * vals(2 * i)))
                Dim indexForReverseInsertion = lengthOfMZAndIntensityArray - i - 1
                mzValuesFullRange(indexForReverseInsertion) = mz
                intensitiesFullRange(indexForReverseInsertion) = intensity
            Next

            ' Trim off m/z values according to parameters
            Dim indexOfLowMZ = GetIndexForMZ(Parameters.MinMZFilter, lengthOfMZAndIntensityArray)
            Dim indexOfHighMZ = GetIndexForMZ(Parameters.MaxMZFilter, lengthOfMZAndIntensityArray)
            mzValues = New Single(indexOfHighMZ - indexOfLowMZ - 1) {}
            intensities = New Single(indexOfHighMZ - indexOfLowMZ - 1) {}

            For i = indexOfLowMZ To indexOfHighMZ - 1
                mzValues(i - indexOfLowMZ) = mzValuesFullRange(i)
                intensities(i - indexOfLowMZ) = intensitiesFullRange(i)
            Next

            _lastScanIndexOpened = scanIndex
        End Sub

        ''' <summary>
        ''' Gets the mass spectrum at the given 0-based scan index
        ''' </summary>
        ''' <remarks>
        ''' Main difference with method GetMassSpectrumUsingSupposedlyFasterBinaryReader is that in this method,
        ''' a new BinaryReader is created every time in this method. This is advantageous in terms of making sure
        ''' the file is opened and closed properly. Unit tests in 2010 show method GetMassSpectrum to be about 3% to 10% slower.
        ''' </remarks>
        ''' <paramname="scanIndex">Zero-based scan index</param>
        ''' <paramname="mzValues">m/z values are returned here</param>
        ''' <paramname="intensities">intensity values are returned here</param>
        Public Sub GetMassSpectrum(ByVal scanIndex As Integer, <Out> ByRef mzValues As Single(), <Out> ByRef intensities As Single())
            Dim scanIndices = {scanIndex}
            GetMassSpectrum(scanIndices, mzValues, intensities)
        End Sub

        ''' <summary>
        ''' Gets the mass spectrum at the given 0-based scan index.
        ''' Only returns data within the given m/z range.
        ''' </summary>
        ''' <remarks>
        ''' If this method is called with m/z range filters defined,
        ''' then you later call GetMassSpectrum that does not have an m/z range,
        ''' the data will still be filtered by the previously used m/z filters.
        ''' </remarks>
        ''' <paramname="scanIndex"></param>
        ''' <paramname="minMZ"></param>
        ''' <paramname="maxMZ"></param>
        ''' <paramname="mzValues"></param>
        ''' <paramname="intensities"></param>
        Public Sub GetMassSpectrum(ByVal scanIndex As Integer, ByVal minMZ As Single, ByVal maxMZ As Single, <Out> ByRef mzValues As Single(), <Out> ByRef intensities As Single())
            Require(Parameters?.ML1 > -1, "Cannot get mass spectrum. Need to first set Parameters.")
            Require(maxMZ >= minMZ, "Cannot get mass spectrum. MinMZ is greater than MaxMZ - that's impossible.")
            If Parameters Is Nothing Then Throw New Exception("Parameters is null in GetMassSpectrum")
            Parameters.MinMZFilter = minMZ
            Parameters.MaxMZFilter = maxMZ
            GetMassSpectrum(scanIndex, mzValues, intensities)
        End Sub

        ''' <summary>
        ''' Gets the summed mass spectrum.
        ''' </summary>
        ''' <paramname="scanIndicesToBeSummed"></param>
        ''' <paramname="mzValues"></param>
        ''' <paramname="intensities"></param>
        Public Sub GetMassSpectrum(ByVal scanIndicesToBeSummed As Integer(), <Out> ByRef mzValues As Single(), <Out> ByRef intensities As Single())
            Require(Parameters IsNot Nothing AndAlso Math.Abs(Parameters.ML1 - -1) > Single.Epsilon, "Cannot get mass spectrum. Need to first set Parameters.")
            If Parameters Is Nothing Then Throw New Exception("Parameters is null in GetMassSpectrum")
            ValidateScanIndices(scanIndicesToBeSummed)
            'Check.Require(scanIndex < GetNumMSScans(), "Cannot get mass spectrum. Requested scan index is greater than number of scans in dataset.");

            Dim scanDataList = New List(Of Double())()

            Using reader = New BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))

                For Each scanIndex In scanIndicesToBeSummed
                    Dim vals = New Double(Parameters.NumValuesInScan - 1) {}
                                        ''' Cannot convert LocalDeclarationStatementSyntax, System.InvalidCastException: Unable to cast object of type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax' to type 'Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax'.
'''    at ICSharpCode.CodeConverter.VB.CommonConversions.RemodelVariableDeclaration(VariableDeclarationSyntax declaration)
'''    at ICSharpCode.CodeConverter.VB.MethodBodyExecutableStatementVisitor.VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
'''    at Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor`1.Visit(SyntaxNode node)
'''    at ICSharpCode.CodeConverter.VB.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)
''' 
''' Input:
''' 
'''                     var bytePosition = scanIndex * (long)this.Parameters.NumValuesInScan * sizeof(int);
''' 
''' 
                    reader.BaseStream.Seek(bytePosition, SeekOrigin.Begin)

                    For i = 0 To Parameters.NumValuesInScan - 1
                        vals(i) = reader.ReadInt32()
                    Next

                    scanDataList.Add(vals)
                Next
            End Using

            Dim lengthOfMZAndIntensityArray = Parameters.NumValuesInScan / 2
            Dim mzValuesFullRange = New Single(lengthOfMZAndIntensityArray - 1) {}
            Dim intensitiesFullRange = New Single(lengthOfMZAndIntensityArray - 1) {}

            For i = 0 To scanDataList.Count - 1
                Dim vals = scanDataList(i)
                _fourierTransform.RealFourierTransform(vals)

                For j = 0 To lengthOfMZAndIntensityArray - 1
                    Dim indexForReverseInsertion = lengthOfMZAndIntensityArray - j - 1
                    Dim firstTimeThrough = i = 0

                    If firstTimeThrough Then
                        Dim mz = CSng(GetMZ(j))
                        mzValuesFullRange(indexForReverseInsertion) = mz
                    End If

                    Dim intensity = CSng(Math.Sqrt(vals(2 * j + 1) * vals(2 * j + 1) + vals(2 * j) * vals(2 * j)))
                    intensitiesFullRange(indexForReverseInsertion) += intensity    'sum the intensities
                Next
            Next

            ' Trim off m/z values according to parameters
            Dim indexOfLowMZ = GetIndexForMZ(Parameters.MinMZFilter, lengthOfMZAndIntensityArray)
            Dim indexOfHighMZ = GetIndexForMZ(Parameters.MaxMZFilter, lengthOfMZAndIntensityArray)
            mzValues = New Single(indexOfHighMZ - indexOfLowMZ - 1) {}
            intensities = New Single(indexOfHighMZ - indexOfLowMZ - 1) {}

            For i = indexOfLowMZ To indexOfHighMZ - 1
                mzValues(i - indexOfLowMZ) = mzValuesFullRange(i)
                intensities(i - indexOfLowMZ) = intensitiesFullRange(i)
            Next
        End Sub

        ' ReSharper disable once UnusedMember.Global
        Public Sub GetMassSpectrum(ByVal scansNumsToBeSummed As Integer(), ByVal minMZ As Single, ByVal maxMZ As Single, <Out> ByRef mzValues As Single(), <Out> ByRef intensities As Single())
            Require(maxMZ >= minMZ, "Cannot get mass spectrum. MinMZ is greater than MaxMZ - that's impossible.")
            Parameters.MinMZFilter = minMZ
            Parameters.MaxMZFilter = maxMZ
            GetMassSpectrum(scansNumsToBeSummed, mzValues, intensities)
        End Sub

        Private Sub ValidateScanIndices(ByVal scanIndicesToBeSummed As IEnumerable(Of Integer))
            For Each scanIndex In scanIndicesToBeSummed
                Require(scanIndex < GetNumMSScans(), "Cannot get mass spectrum. Requested scan index (" & scanIndex & ") is greater than number of scans in dataset. Note that the first scan is scan 0")
            Next
        End Sub

        Private Function GetIndexForMZ(ByVal targetMZ As Single, ByVal arrayLength As Integer) As Integer
            Dim index = CInt(Parameters.NumValuesInScan / Parameters.SampleRate * (Parameters.ML1 / targetMZ - Parameters.ML2))
            index = arrayLength - index

            If index < 0 Then
                Return 0
            End If

            If index > arrayLength - 1 Then
                Return arrayLength - 1
            End If

            Return index
        End Function

        Private Function GetMZ(ByVal i As Integer) As Double
            Dim freq = i * Parameters.SampleRate / Parameters.NumValuesInScan

            If Math.Abs(freq + Parameters.ML2) > 0 Then
                Return Parameters.ML1 / (freq + Parameters.ML2)
            End If

            If freq - Parameters.ML2 <= 0 Then
                Return Parameters.ML1
            End If

            Return 0
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Try

                If _reader IsNot Nothing Then
                    Using br = _reader
                        br.Close()
                    End Using
                End If

            Catch __unusedException1__ As Exception
                Console.WriteLine("BrukerDataReader had problems closing the binary reader. Note this.")
            End Try
        End Sub
    End Class
End Namespace
