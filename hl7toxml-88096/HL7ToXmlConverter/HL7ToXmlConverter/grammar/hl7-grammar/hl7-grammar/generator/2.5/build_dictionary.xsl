<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:hl7="urn:hl7-org:v2xml" 
xmlns="urn:hl7-org:v2xml" 
xmlns:xsd="http://www.w3.org/2001/XMLSchema"
>

<xsl:output method="text" indent="no"/>

<xsl:param name="VERSION" select="'HL7v2XML 2.5 200401'"/>

<xsl:key name='attributes' match='/xsd:schema/xsd:attributeGroup' use='@name'/>


<xsl:variable name='datatypes' select='document("xsd/datatypes.xsd")'/>
<xsl:variable name='fields' select='document("xsd/fields.xsd")'/>
<xsl:variable name='segments' select='document("xsd/segments.xsd")'/>


  
<xsl:template match="/">
<hl7v2xml version="{$VERSION}">
	<xsl:apply-templates 
		select="$datatypes/xsd:schema/xsd:simpleType" 
		mode="string"/>
	<xsl:apply-templates 
		select="$datatypes/xsd:schema/xsd:complexType[xsd:sequence]" 
		mode="datatype"/>
	<xsl:apply-templates 
		select="$datatypes/xsd:schema/xsd:complexType[xsd:simpleContent or xsd:complexContent]"
		mode="type"/>
	<xsl:apply-templates 
		select="$fields/xsd:schema/xsd:complexType[xsd:simpleContent or xsd:complexContent]" 
		mode="type"/>
	<xsl:apply-templates 
		select="$segments/xsd:schema/xsd:complexType[xsd:sequence] | $segments/xsd:schema/xsd:complexType[xsd:choice]" 
		mode="segment"/>
	<xsl:for-each select="job/message">
		<xsl:variable name="GET" select="@name"/>
			<xsl:variable name='MESSAGE' select='concat("xsd/",@name,".xsd")'/>

			<xsl:variable name='messageDef' select='document($MESSAGE)'/>

			<xsl:apply-templates 
				select="$messageDef/xsd:schema/xsd:complexType[xsd:sequence]" 
				mode="segment"/>
	</xsl:for-each>
	<xsl:text>
</xsl:text>
</hl7v2xml>
</xsl:template>

<xsl:template match="xsd:complexType[xsd:sequence/xsd:element]" mode="segment">
	<xsl:text>
</xsl:text>
	<xsl:value-of select="substring-before(@name,'.CONTENT')"/>
	<xsl:text> (</xsl:text>
	<xsl:apply-templates mode="sequence"/>
	<xsl:text>)</xsl:text><xsl:text>&#x0D;</xsl:text>
</xsl:template>

<xsl:template match="xsd:complexType[xsd:choice/xsd:element]" mode="segment">
	<xsl:text>
</xsl:text>
	<xsl:value-of select="concat('%',substring-before(@name,'.TYPE'))"/>
	<xsl:text> (</xsl:text>
	<xsl:apply-templates mode="sequence"/>
	<xsl:text>)</xsl:text><xsl:text>&#x0D;</xsl:text>
</xsl:template>

<xsl:template match="xsd:simpleType" mode="string">
	<xsl:text>
</xsl:text>
	<xsl:value-of select="concat('%',@name)"/><xsl:text> (#PCDATA)</xsl:text><xsl:text>&#x0D;</xsl:text>
</xsl:template>

<xsl:template match="xsd:complexType[xsd:sequence/xsd:element/@type='escapeType']" mode="datatype">
	<xsl:text>
</xsl:text>
	<xsl:value-of select="concat('%',@name)"/><xsl:text> (#PCDATA)</xsl:text><xsl:text>&#x0D;</xsl:text>
</xsl:template>

<xsl:template match="xsd:complexType" mode="datatype">
	<xsl:text>
</xsl:text>
	<xsl:value-of select="concat('%',@name)"/>
	<xsl:text> (</xsl:text>
	<xsl:apply-templates mode="sequence"/>
	<xsl:text>)</xsl:text><xsl:text>&#x0D;</xsl:text>
</xsl:template>

<xsl:template match="xsd:choice" mode="sequence">
	<xsl:apply-templates mode="sequence"/>
</xsl:template>

<xsl:template match="xsd:sequence" mode="sequence">
	<xsl:apply-templates mode="sequence"/>
</xsl:template>

<xsl:template match="xsd:element" mode="sequence">
	<xsl:variable name="CARDINALITY">
		<xsl:choose>
			<xsl:when 
			test="@minOccurs='0' and ( @maxOccurs='1' or not(@maxOccurs) )">?</xsl:when>
			<xsl:when 
			test="@minOccurs='0' and @maxOccurs='unbounded'">*</xsl:when>
			<xsl:when 
			test="( @minOccurs='1' or not(@minOccurs)) and ( @maxOccurs='1' or not(@maxOccurs) )">					</xsl:when>
			<xsl:when 
			test="( @minOccurs='1' or not(@minOccurs)) and @maxOccurs='unbounded'">+</xsl:when>
		</xsl:choose>
	</xsl:variable>
	<xsl:if test="position() > 1">,</xsl:if>
	<xsl:value-of select="concat(@ref,$CARDINALITY)"/>
</xsl:template>

<xsl:template match="xsd:complexType" mode="type">
	<xsl:if test="@name != 'varies'">
	<xsl:text>
</xsl:text>
	<xsl:value-of select="substring-before(@name,'.CONTENT')"/>
	<xsl:value-of select="substring-before(@name,'.TYPE')"/>
	<xsl:value-of select="concat(' %',
			xsd:simpleContent/xsd:extension/@base,
			xsd:complexContent/xsd:extension/@base)"/>
	<xsl:apply-templates mode="type"/>
	</xsl:if>
</xsl:template>

<xsl:template match="xsd:complexContent" mode="type">
	<xsl:apply-templates mode="type"/>
</xsl:template>

<xsl:template match="xsd:annotation" mode="type">
</xsl:template>

<xsl:template match="xsd:extension" mode="type">
	<xsl:apply-templates mode="type"/>
</xsl:template>

<xsl:template match="xsd:attributeGroup" mode="type">
	<xsl:apply-templates select="key('attributes',@ref)" mode="attributes"/>
</xsl:template>

<xsl:template match="xsd:attributeGroup" mode="attributes">
	<xsl:apply-templates mode="attributes"/>
	<xsl:text>&#x0D;</xsl:text>
</xsl:template>

<xsl:template match="xsd:attribute" mode="attributes">
	<xsl:if test="@name != 'Type'">
		<xsl:text> </xsl:text><xsl:value-of select="@name"/>='<xsl:value-of select="@fixed"/>			<xsl:text>'</xsl:text>
	</xsl:if>
</xsl:template>


<xsl:template match="node() | @* | comment() | processing-instruction()">
<!--
  <xsl:copy>
    <xsl:apply-templates select="@* | node()"/>
  </xsl:copy>
-->
</xsl:template>

</xsl:stylesheet>
