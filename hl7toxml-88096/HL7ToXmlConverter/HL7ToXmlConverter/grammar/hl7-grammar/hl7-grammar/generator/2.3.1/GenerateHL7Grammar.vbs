'==========================================================================
'
' VBScript Source File
'
' NAME: GenerateHL7Grammar.vbs
'
' AUTHOR: Jan Schuster , OPTIMAL SYSTEMS Vertriebsgesellschaft mbH Berlin
' DATE  : 15.02.2007
'
' COMMENT: Generierung der HL7 Grammatik für den Parser im OS|COMMUNICATOR
'
'==========================================================================
Option Explicit

Dim xmlDocument, xslStylesheet, xmlLoaded, XslLoaded, scriptPath
Dim xmlfile, xslfile, xmlout
Dim result
Dim fso, file
Set xmlDocument 	= WScript.CreateObject("Msxml.DOMDocument")
Set xslStylesheet 	= WScript.CreateObject("Msxml.DOMDocument")

Set fso = CreateObject("Scripting.FileSystemObject")

scriptPath = Replace(WScript.ScriptFullName,"create.vbs","")

xmlfile = scriptPath & "hl7_231_job.xml"
xslfile = scriptPath & "build_dictionary.xsl"
xmlout 	= scriptPath & "hl7_231_generated_DO_NO_USE_ME.dat"

xmlDocument.async = False
xmlLoaded = XmlDocument.load(xmlfile)
If xmlLoaded Then
	xslStylesheet.async = False
	If xslStylesheet.load(xslfile) Then
		result = XmlDocument.transformnode(xslStylesheet)
		Set file=fso.CreateTextFile(xmlout, True, False)
		file.Write result
		file.Close
	Else
		WScript.Echo xslStylesheet.parseError.reason
	End If
Else
	WScript.Echo xmlDocument.parseError.reason
End If
