<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>

	<xsl:param name="DateTime" select="//PartCoverExtension/DateTime" />
	<xsl:param name="ProjectName" select="//PartCoverExtension/ProjectName" />
	<xsl:param name="Acceptable" select="//PartCoverExtension/Acceptable"/>

	<xsl:template match="/">
		<html>
			<head>
				<xsl:comment>Adapted from NCoverExplorer (see http://www.kiwidude.com/blog/)</xsl:comment>
				<title>PartCover - Report</title>
				<style>
					body 						{ font: small verdana, arial, helvetica; color:#000000;	}
					.coverageReportTable		{ font-size: 9px; }
					.reportHeader 				{ padding: 5px 8px 5px 8px; font-size: 12px; border: 1px solid; margin: 0px;	}
					.titleText					{ font-weight: bold; font-size: 12px; white-space: nowrap; padding: 0px; margin: 1px; }
					.subtitleText 				{ font-size: 9px; font-weight: normal; padding: 0px; margin: 1px; white-space: nowrap; }
					.projectStatistics			{ font-size: 10px; border-left: #649cc0 1px solid; white-space: nowrap;	width: 40%;	}
					.heading					{ font-weight: bold; }
					.mainTableHeaderLeft 		{ border: #dcdcdc 1px solid; font-weight: bold;	padding-left: 5px; }
					.mainTableHeader 			{ border-bottom: 1px solid; border-top: 1px solid; border-right: 1px solid;	text-align: center;	}
					.mainTableGraphHeader 		{ border-bottom: 1px solid; border-top: 1px solid; border-right: 1px solid;	text-align: left; font-weight: bold; }
					.mainTableCellItem 			{ background: #ffffff; border-left: #dcdcdc 1px solid; border-right: #dcdcdc 1px solid; padding-left: 10px; padding-right: 10px; font-weight: bold; font-size: 10px; }
					.mainTableCellData 			{ background: #ffffff; border-right: #dcdcdc 1px solid;	text-align: center;	white-space: nowrap; }
					.mainTableCellPercent 		{ background: #ffffff; font-weight: bold; white-space: nowrap; text-align: right; padding-left: 10px; }
					.mainTableCellGraph 		{ background: #ffffff; border-right: #dcdcdc 1px solid; padding-right: 5px; }
					.mainTableCellBottom		{ border-bottom: #dcdcdc 1px solid;	}
					.childTableHeader 			{ border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-right: 1px solid;	font-weight: bold; padding-left: 10px; }
					.childTableCellIndentedItem { background: #ffffff; border-left: #dcdcdc 1px solid; border-right: #dcdcdc 1px solid; padding-right: 10px; font-size: 10px; }		
					.exclusionTableCellItem 	{ background: #ffffff; border-left: #dcdcdc 1px solid; border-right: #dcdcdc 1px solid; padding-left: 10px; padding-right: 10px; }
					.projectTable				{ background: #a9d9f7; border-color: #649cc0; }
					.primaryTable				{ background: #d7eefd; border-color: #a4dafc; }
					.secondaryTable 			{ background: #f9e9b7; border-color: #f6d376; }
					.secondaryChildTable 		{ background: #fff6df; border-color: #f5e1b1; }
					.exclusionTable				{ background: #fadada; border-color: #f37f7f; }
					.graphBarNotVisited			{ font-size: 2px; border:#9c9c9c 1px solid; background:#df0000; }
					.graphBarSatisfactory		{ font-size: 2px; border:#9c9c9c 1px solid;	background:#f4f24e; }
					.graphBarVisited			{ background: #00df00; font-size: 2px; border-left:#9c9c9c 1px solid; border-top:#9c9c9c 1px solid; border-bottom:#9c9c9c 1px solid; }
					.graphBarVisitedFully		{ background: #00df00; font-size: 2px; border:#9c9c9c 1px solid; }
				</style>
			</head>
			<body>
				<table class="coverageReportTable" cellpadding="2" cellspacing="0">
					<tbody>
						<xsl:variable name="codeSizeAll" select="sum(//PartCoverReport/type/method/code/pt/@len)+0"/>
						<xsl:variable name="coveredCodeSizeAll" select="sum(//PartCoverReport/type/method/code/pt[@visit>0]/@len)+0"/>
						<xsl:variable name="coverageAll" select="ceiling(100 * $coveredCodeSizeAll div $codeSizeAll)"/>
						
						<xsl:call-template name="header" >
							<xsl:with-param name="DateTime" value="$DateTime"/>
							<xsl:with-param name="ProjectName" value="$ProjectName"/>
							<xsl:with-param name="PartcoverVersion" value="$PartcoverVersion"/>
							<xsl:with-param name="codeSizeAll" select="$codeSizeAll" />
							<xsl:with-param name="coveredCodeSizeAll" select="$coveredCodeSizeAll" />
							</xsl:call-template>
						
						<xsl:variable name="reportType" select="./@reportTitle" />
						<xsl:variable name="threshold">
							<xsl:choose>
								<xsl:when test="$Acceptable > 0"><xsl:value-of select="$Acceptable" />
									</xsl:when>
									<xsl:otherwise><xsl:value-of select="//PartCoverExtension/Acceptable" /></xsl:otherwise>
								</xsl:choose>
							</xsl:variable>
							<xsl:variable name="unvisitedTitle">
								<xsl:choose>
									<xsl:when test="$reportType = 'Module Class Function Summary'">Unvisited Functions</xsl:when>
									<xsl:otherwise>Unvisited LOC</xsl:otherwise>
								</xsl:choose>
							</xsl:variable>
							<xsl:variable name="coverageTitle">
								<xsl:choose>
									<xsl:when test="$reportType = 'Module Class Function Summary'">Function Coverage</xsl:when>
									<xsl:otherwise>Coverage</xsl:otherwise>
								</xsl:choose>
							</xsl:variable>

							<xsl:call-template name="projectSummary">
								<xsl:with-param name="threshold" select="$threshold" />
								<xsl:with-param name="unvisitedTitle" select="$unvisitedTitle" />
								<xsl:with-param name="coverageTitle" select="$coverageTitle" />
								<xsl:with-param name="reportType" select="$reportType" />
								<xsl:with-param name="codeSizeAll" select="$codeSizeAll" />
								<xsl:with-param name="coveredCodeSizeAll" select="$coveredCodeSizeAll" />
								<xsl:with-param name="coverageAll" select="$coverageAll" />
							</xsl:call-template>

							<xsl:call-template name="moduleSummary">
								<xsl:with-param name="threshold" select="$threshold" />
								<xsl:with-param name="unvisitedTitle" select="$unvisitedTitle" />
								<xsl:with-param name="coverageTitle" select="$coverageTitle" />
							</xsl:call-template>

						</tbody>
					</table>
				</body>
			</html>
		</xsl:template>

		<!-- Report Header -->
		<xsl:template name="header">
			<xsl:param name="codeSizeAll" />
			<xsl:param name="coveredCodeSizeAll" />
			<tr>
				<td class="projectTable reportHeader" colspan="5">
					<table width="100%">
						<tbody>
							<tr>
								<td valign="top">
									<h1 class="titleText">PartCover Coverage Report - <xsl:value-of select="$ProjectName" />&#160;&#160;</h1>
									<table cellpadding="1" class="subtitleText">
										<tbody>
											<tr>
												<td class="heading">Report generated on:</td>
												<td>
													<xsl:value-of select="$DateTime" />
												</td>
											</tr>
											<tr>
												<td class="heading">PartCover version:</td>
												<td>
													<xsl:value-of select="//PartCoverReport/@ver" />
												</td>
											</tr>
										</tbody>
									</table>
								</td>
								<td class="projectStatistics" align="right" valign="top">
									<table cellpadding="1">
										<tbody>
											<tr>
												<td rowspan="4" valign="top" nowrap="true" class="heading">Project Statistics:</td>
												<td align="right" class="heading">Files:</td>
												<td align="right">
													<xsl:value-of select="count(//PartCoverReport/file)" />
												</td>
												<td rowspan="4">&#160;</td>
											</tr>
											<tr>
												<td align="right" class="heading">Classes:</td>
												<td align="right">
													<xsl:value-of select="count(//PartCoverReport/type)" />
												</td>
												<td align="right" class="heading">&#160;</td>
												<td align="right">&#160;</td>
											</tr>
											<tr>
												<td align="right" class="heading">Functions:</td>
												<xsl:variable name="methods" select="count(//PartCoverReport/type/method)"/>
												<td align="right">
													<xsl:value-of select="$methods" />
												</td>
												<td align="right" class="heading">Unvisited:</td>
												<xsl:variable name="unvisited-methods" select="count(//PartCoverReport/type/method[./code/pt[@visit=0]])"/>
												<td align="right">
													<xsl:value-of select="($methods - $unvisited-methods)" />
												</td>
											</tr>
											<tr>
												<td align="right" class="heading">LOC:</td>
												<td align="right">
													<xsl:value-of select="$codeSizeAll" />
												</td>
												<td align="right" class="heading">Unvisited:</td>
												<td align="right">
													<xsl:value-of select="($codeSizeAll - $coveredCodeSizeAll)" />
												</td>
											</tr>
										</tbody>
									</table>
								</td>
							</tr>
						</tbody>
					</table>
				</td>
			</tr>
		</xsl:template>

		<!-- Project Summary -->
		<xsl:template name="projectSummary">
			<xsl:param name="threshold" />
			<xsl:param name="unvisitedTitle" />
			<xsl:param name="coverageTitle" />
			<xsl:param name="reportType" />
			<xsl:param name="codeSizeAll" />
			<xsl:param name="coveredCodeSizeAll" />
			<xsl:param name="coverageAll" />
			<tr>
				<td colspan="5">&#160;</td>
			</tr>
			<tr>
				<td class="projectTable mainTableHeaderLeft">Project</td>
				<td class="projectTable mainTableHeader">Acceptable</td>
				<td class="projectTable mainTableHeader">
					<xsl:value-of select="$unvisitedTitle" />
				</td>
				<td class="projectTable mainTableGraphHeader" colspan="2">
					<xsl:value-of select="$coverageTitle" />
				</td>
			</tr>
			<xsl:call-template name="coverageDetail">
				<xsl:with-param name="name" select="$ProjectName" />
				<xsl:with-param name="unvisitedPoints" select="($codeSizeAll - $coveredCodeSizeAll)" />
				<xsl:with-param name="sequencePoints" select="$codeSizeAll" />
				<xsl:with-param name="coverage" select="$coverageAll" />
				<xsl:with-param name="threshold" select="$threshold" />
				<xsl:with-param name="showThreshold">True</xsl:with-param>
			</xsl:call-template>
		</xsl:template>

		<!-- Modules Summary -->
		<xsl:template name="moduleSummary">
			<xsl:param name="threshold" />
			<xsl:param name="unvisitedTitle" />
			<xsl:param name="coverageTitle" />
			<tr>
				<td colspan="5">&#160;</td>
			</tr>
			<tr>
				<td class="primaryTable mainTableHeaderLeft">Modules</td>
				<td class="primaryTable mainTableHeader">Acceptable</td>
				<td class="primaryTable mainTableHeader">
					<xsl:value-of select="$unvisitedTitle" />
				</td>
				<td class="primaryTable mainTableGraphHeader" colspan="2">
					<xsl:value-of select="$coverageTitle" />
				</td>
			</tr>				
			<xsl:variable name="unique-asms" select="//PartCoverReport/type[not(@asm=following::type/@asm)]"/>
			<xsl:for-each select="$unique-asms">
				<xsl:sort select="./@asm"/>
				<xsl:variable name="current-asm" select="./@asm"/>
				<xsl:variable name="codeSize" select="sum(//PartCoverReport/type[@asm=$current-asm]/method/code/pt/@len)+0"/>
				<xsl:variable name="coveredCodeSize" select="sum(//PartCoverReport/type[@asm=$current-asm]/method/code/pt[@visit>0]/@len)+0"/>
				<xsl:variable name="coverage" select="ceiling(100 * $coveredCodeSize div $codeSize)"/>
				<xsl:call-template name="coverageDetail">
					<xsl:with-param name="name" select="$current-asm" />
					<xsl:with-param name="unvisitedPoints" select="($codeSize - $coveredCodeSize)" />
					<xsl:with-param name="sequencePoints" select="$codeSize" />
					<xsl:with-param name="coverage" select="$coverage" />
					<xsl:with-param name="threshold" select="$threshold" />
					<xsl:with-param name="showThreshold">True</xsl:with-param>
					</xsl:call-template>
				</xsl:for-each>
			</xsl:template>
		
		<!-- Coverage detail row in main grid displaying a name, statistics and graph bar -->
		<xsl:template name="coverageDetail">
			<xsl:param name="name" />
			<xsl:param name="unvisitedPoints" />
			<xsl:param name="sequencePoints" />
			<xsl:param name="coverage" />
			<xsl:param name="threshold" />
			<xsl:param name="showThreshold" />
			<tr>
				<xsl:choose>
					<xsl:when test="$showThreshold='True'">
						<td class="mainTableCellBottom mainTableCellItem">
							<xsl:value-of select="$name" />
							</td>
						<td class="mainTableCellBottom mainTableCellData">
							<xsl:value-of select="concat(format-number($threshold,'#0.0'), ' %')" />
							</td>
						</xsl:when>
					<xsl:otherwise>
						<td class="mainTableCellBottom mainTableCellItem" colspan="2">
							<xsl:value-of select="$name" />
							</td>
						</xsl:otherwise>
					</xsl:choose>
				<td class="mainTableCellBottom mainTableCellData">
					<xsl:value-of select="$unvisitedPoints" />
					</td>
				<td class="mainTableCellBottom mainTableCellPercent">
					<xsl:value-of select="concat(format-number($coverage,'#0.0'), ' %')" />
					</td>
				<td class="mainTableCellBottom mainTableCellGraph">
					<xsl:call-template name="detailPercent">
						<xsl:with-param name="notVisited" select="$unvisitedPoints" />
						<xsl:with-param name="total" select="$sequencePoints" />
						<xsl:with-param name="threshold" select="$threshold" />
						<xsl:with-param name="scale" select="200" />
						</xsl:call-template>
					</td>
				</tr>
			</xsl:template>
		
		<!-- Draw % Green/Red/Yellow Bar -->
		<xsl:template name="detailPercent">
			<xsl:param name="notVisited" />
			<xsl:param name="total" />
			<xsl:param name="threshold" />
			<xsl:param name="scale" />
			<xsl:variable name="visited" select="$total - $notVisited" />
			<xsl:variable name="coverage" select="$visited div $total * 100"/>
			<table cellpadding="0" cellspacing="0">
				<tbody>
					<tr>
						<xsl:if test="$notVisited = 0">
							<td class="graphBarVisitedFully" height="14">
								<xsl:attribute name="width">
									<xsl:value-of select="$scale" />
									</xsl:attribute>.</td>
							</xsl:if>
						<xsl:if test="($visited != 0) and ($notVisited != 0)">
							<td class="graphBarVisited" height="14">
								<xsl:attribute name="width">
									<xsl:value-of select="format-number($coverage div 100 * $scale, '0') - 1" />
									</xsl:attribute>.</td>
							</xsl:if>
						<xsl:if test="$notVisited != 0">
							<td height="14">
								<xsl:attribute name="class">
									<xsl:if test="$coverage &gt;= $threshold">graphBarSatisfactory</xsl:if>
									<xsl:if test="$coverage &lt; $threshold">graphBarNotVisited</xsl:if>
									</xsl:attribute>
								<xsl:attribute name="width">
									<xsl:value-of select="format-number($notVisited div $total * $scale, '0')" />
									</xsl:attribute>.</td>
							</xsl:if>
						</tr>
					</tbody>
				</table>
			</xsl:template>
		
		</xsl:stylesheet>
