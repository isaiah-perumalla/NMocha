<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes"/>
	<xsl:param name="DateTime"/>
	<xsl:param name="ProjectName"/>
	<xsl:param name="Acceptable"/>
	<xsl:template match="/">
		<PartCoverExtension>
			<DateTime><xsl:value-of select="$DateTime" /></DateTime>
			<ProjectName><xsl:value-of select="$ProjectName" /></ProjectName>
			<Acceptable><xsl:value-of select="$Acceptable" /></Acceptable>
		</PartCoverExtension>
	</xsl:template>
</xsl:stylesheet>