
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace NCBI.PubChem.Web

	''' <summary>
	''' result data set for pubchem query summary
	''' </summary>
	''' <remarks>
	''' [Download]
	''' Summary (Search Results)
	''' XML format
	''' </remarks>
	''' 
	<XmlType("row"), XmlRoot("row")>
	Public Class QueryXml

		Public Property cid As Integer
		Public Property cmpdname As String
		Public Property cmpdsynonym As sub_cmpdsynonym
		Public Property mw As Double
		Public Property mf As String
		Public Property polararea As Double
		Public Property complexity As Double
		Public Property xlogp As Double
		Public Property heavycnt As Double
		Public Property hbonddonor As Double
		Public Property hbondacc As Double
		Public Property rotbonds As Double
		Public Property inchi As String
		Public Property isosmiles As String
		Public Property canonicalsmiles As String
		Public Property inchikey As String
		Public Property iupacname As String
		Public Property exactmass As Double
		Public Property monoisotopicmass As Double
		Public Property charge As Double
		Public Property covalentunitcnt As Double
		Public Property isotopeatomcnt As Double
		Public Property totalatomstereocnt As Double
		Public Property definedatomstereocnt As Double
		Public Property undefinedatomstereocnt As Double
		Public Property totalbondstereocnt As Double
		Public Property definedbondstereocnt As Double
		Public Property undefinedbondstereocnt As Double
		Public Property pclidcnt As Double
		Public Property gpidcnt As Double
		Public Property gpfamilycnt As Double
		Public Property neighbortype As String
		Public Property meshheadings As String
		Public Property sidsrcname As sidsrcname

		<XmlElement("annotation")>
		Public Property annotation As Object

		<MethodImpl(MethodImplOptions.AggressiveInlining)>
		Public Shared Iterator Function Load(filepath As String) As IEnumerable(Of QueryXml)
			For Each metabo As QueryXml In LoadUltraLargeXMLDataSet(Of QueryXml)(filepath, typeName:="row", variants:={GetType(String), GetType(annotation)})
				Yield metabo
			Next
		End Function

		<MethodImpl(MethodImplOptions.AggressiveInlining)>
		Public Overrides Function ToString() As String
			Return Me.GetXml
		End Function

	End Class

	<XmlType("sub-cmpdsynonym")>
	Public Class sub_cmpdsynonym

		<XmlElement("sub-cmpdsynonym")>
		Public Property cmpdsynonym As String()

		<MethodImpl(MethodImplOptions.AggressiveInlining)>
		Public Overrides Function ToString() As String
			Return cmpdsynonym.GetJson
		End Function

	End Class

	Public Class sidsrcname

		<XmlElement("sub-sidsrcname")>
		Public Property sidsrcname As String()

		<MethodImpl(MethodImplOptions.AggressiveInlining)>
		Public Overrides Function ToString() As String
			Return sidsrcname.GetJson
		End Function

	End Class

	Public Class annotation

		<XmlElement("sub-annotation")>
		Public Property sub_annotation As String()

		<MethodImpl(MethodImplOptions.AggressiveInlining)>
		Public Overrides Function ToString() As String
			Return sub_annotation.GetJson
		End Function

	End Class
End Namespace