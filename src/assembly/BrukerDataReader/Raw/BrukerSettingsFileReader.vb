#Region "Microsoft.VisualBasic::9267d2594fd2fad917ce8340091b3adc, G:/mzkit/src/assembly/BrukerDataReader//Raw/BrukerSettingsFileReader.vb"

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

    '   Total Lines: 207
    '    Code Lines: 139
    ' Comment Lines: 22
    '   Blank Lines: 46
    '     File Size: 8.98 KB


    '     Class BrukerSettingsFileReader
    ' 
    '         Function: GetDoubleFromParamList, GetNameFromNode, GetValueFromNode, LoadApexAcqParameters, LoadApexAcqusParameters
    '                   PreScanApexAcqFile
    '         Structure BrukerNameValuePair
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Xml.Linq

Namespace Raw

    Friend Class BrukerSettingsFileReader
        ' ReSharper disable once CommentTypo
        ' Ignore Spelling: Bruker, paramlist, pre

        Private Structure BrukerNameValuePair
            Public Name As String
            Public Value As String
        End Structure

        Private Function GetNameFromNode(node As XElement) As String
            Dim elementNames = From n In node.Elements() Select n.Name.LocalName
            Dim attributeNames = From n In node.Attributes() Select n.Name.LocalName
            Dim nameIsAnXMLElement = elementNames.Contains("name")
            Dim nameIsAnAttribute = attributeNames.Contains("name")
            Dim nameValue = String.Empty

            If nameIsAnXMLElement Then
                Dim element = node.Element("name")

                If element IsNot Nothing Then
                    nameValue = element.Value
                End If
            ElseIf nameIsAnAttribute Then
                nameValue = node.Attribute("name")?.Value
            End If

            Return nameValue
        End Function

        Private Function GetValueFromNode(node As XContainer) As String
            Dim elementNames = (From n In node.Elements() Select n.Name.LocalName).ToList()
            Dim fieldName = String.Empty

            If elementNames.Contains("value") Then
                fieldName = "value"
            ElseIf elementNames.Contains("Value") Then
                fieldName = "Value"
            End If

            Dim valueString = String.Empty

            If Not String.IsNullOrWhiteSpace(fieldName) Then
                Dim element = node.Element(fieldName)

                If element IsNot Nothing Then
                    valueString = element.Value
                End If
            End If

            Return valueString
        End Function

        Private Function GetDoubleFromParamList(paramList As IEnumerable(Of BrukerNameValuePair), paramName As String, valueIfMissing As Double) As Double
            For Each item In paramList

                If Equals(item.Name, paramName) Then
                    Return Convert.ToDouble(item.Value)
                End If
            Next

            Return valueIfMissing
        End Function

        Public Function LoadApexAcqParameters(fiSettingsFile As FileInfo) As GlobalParameters
            ' Bruker acquisition software will write out data like this to the apexAcquisition.method file
            ' if the user enters a sample description of "<2mg/mL  100mM  AA  SID35"
            '
            '   <sampledescription>&lt;<2mg/mL&#x20; 100mM&#x20; AA&#x20; SID35</sampledescription>
            '
            ' The extra "<" after &lt; should not be there
            ' Its presence causes the XDocument.Load() event to fail
            ' Thus, we must pre-scrub the file prior to passing it to .Load()

            Dim paramList = New List(Of BrukerNameValuePair)()
            Dim tmpFilePath = PreScanApexAcqFile(fiSettingsFile.FullName)

            Using fileReader = New StreamReader(New FileStream(tmpFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                Dim reader = XDocument.Load(fileReader)
                Dim methodElement = reader.Element("method")

                ' ReSharper disable once StringLiteralTypo
                Dim paramListElement = methodElement?.Element("paramlist")

                If paramListElement IsNot Nothing Then
                    Dim paramNodes = From node In paramListElement.Elements() Select node

                    For Each node In paramNodes
                        Dim nameValuePair = New BrukerNameValuePair With {
                            .Name = GetNameFromNode(node),
                            .Value = GetValueFromNode(node)
                        }
                        If Not Equals(nameValuePair.Name, Nothing) Then paramList.Add(nameValuePair)
                    Next
                End If
            End Using

            Dim parameters = New GlobalParameters() With {
                .ML1 = GetDoubleFromParamList(paramList, "ML1", 0),                  ' calA
                .ML2 = GetDoubleFromParamList(paramList, "ML2", 0),                  ' calB
                .SampleRate = GetDoubleFromParamList(paramList, "SW_h", 0) * 2,       ' sampleRate; SW_h is the digitizer rate and Bruker entered it as the Nyquist frequency so it needs to be multiplied by 2.
                .NumValuesInScan = Convert.ToInt32(GetDoubleFromParamList(paramList, "TD", 0)),   ' numValuesInScan
                .AcquiredMZMinimum = GetDoubleFromParamList(paramList, "EXC_low", 0),         ' Minimum m/z value in each mass spectrum
                .AcquiredMZMaximum = GetDoubleFromParamList(paramList, "EXC_hi", 0)           ' Maximum m/z value in each mass spectrum
            }

            ' Additional parameters that may be of interest

            ' FR_Low = GetDoubleFromParamList(paramList, "FR_low", 0),

            ' ReSharper disable once CommentTypo
            ' ByteOrder = Convert.ToInt32(GetDoubleFromParamList(paramList, "BYTORDP", 0))

            'this.CalibrationData.NF = Convert.ToInt32(getDoubleFromParamList(paramList, "NF", 0));

            Try
                ' Delete the temp file
                File.Delete(tmpFilePath)
            Catch __unusedException1__ As Exception
                ' Ignore errors here
            End Try

            Return parameters
        End Function

        Private Function PreScanApexAcqFile(apexAcqFilePath As String) As String
            ' Look for
            '   &lt;<
            ' but exclude matches to
            '   &lt;</

            Dim reLessThanMatcher = New Regex("&lt;<(?!/)", RegexOptions.Compiled)
            Dim fixedFilePath = Path.GetTempFileName()

            Using reader = New StreamReader(New FileStream(apexAcqFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))

                Using writer = New StreamWriter(New FileStream(fixedFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))

                    While Not reader.EndOfStream
                        Dim dataLine = reader.ReadLine()

                        If String.IsNullOrEmpty(dataLine) Then
                            writer.WriteLine()
                            Continue While
                        End If

                        Dim reMatch = reLessThanMatcher.Match(dataLine)

                        If reMatch.Success Then
                            writer.WriteLine(reLessThanMatcher.Replace(dataLine, "&lt;"))
                        Else
                            writer.WriteLine(dataLine)
                        End If
                    End While
                End Using
            End Using

            Return fixedFilePath
        End Function

        Public Function LoadApexAcqusParameters(fiSettingsFile As FileInfo) As GlobalParameters
            Dim dataLookupTable = New Dictionary(Of String, Double)()
            Dim parsedResult As Double = Nothing

            Using sr = New StreamReader(fiSettingsFile.FullName)

                While Not sr.EndOfStream
                    Dim currentLine = sr.ReadLine()
                    If String.IsNullOrWhiteSpace(currentLine) Then Continue While
                    Dim match = Regex.Match(currentLine, "^##\$(?<name>.*)=\s(?<value>[0-9-\.]+)")

                    If Not match.Success Then
                        Continue While
                    End If

                    Dim variableName = match.Groups("name").Value
                    Dim canParseValue = Double.TryParse(match.Groups("value").Value, parsedResult)

                    If Not canParseValue Then
                        parsedResult = -1
                    End If

                    dataLookupTable.Add(variableName, parsedResult)
                End While
            End Using

            Dim parameters = New GlobalParameters With {
                .ML1 = dataLookupTable("ML1"),
                .ML2 = dataLookupTable("ML2"),
                .SampleRate = dataLookupTable("SW_h") * 2,  ' From Gordon A.:  SW_h is the digitizer rate and Bruker entered it as the Nyquist frequency so it needs to be multiplied by 2.
                .NumValuesInScan = CInt(dataLookupTable("TD"))
            }
            Dim dataValue As Double = Nothing
            If dataLookupTable.TryGetValue("EXC_low", dataValue) Then parameters.AcquiredMZMinimum = dataValue
            If dataLookupTable.TryGetValue("EXC_hi", dataValue) Then parameters.AcquiredMZMaximum = dataValue
            Return parameters
        End Function
    End Class
End Namespace
