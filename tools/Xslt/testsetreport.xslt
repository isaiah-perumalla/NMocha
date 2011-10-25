<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:temp="http://tempuri.org/TestSetReport.xsd" >
	<xsl:output method="html" indent="yes"/>
	<xsl:include href="testsetreport-templates.xslt" />

	<xsl:template match="/">
		<html>
			<head>
				<title>TestSet - Report</title>
				<style>
					body 						{ font: small verdana, arial, helvetica; color:#000000;	}
					table.stats					{
													text-align: center;
													font-family: Verdana, Geneva, Arial, Helvetica, sans-serif ;
													font-weight: normal;
													font-size: 11px;
													color: #fff;
													width: 400px;
													background-color: #666;
													border: 0px;
													border-collapse: collapse;
													border-spacing: 0px;
												}
					table.stats td				{
													background-color: #CCC;
													color: #000;
													padding: 4px;
													text-align: left;
													border: 1px #fff solid;
												}
					table.stats td.hed
												{
													background-color: #666;
													color: #fff;
													padding: 4px;
													text-align: left;
													border-bottom: 2px #fff solid;
													font-size: 12px;
													font-weight: bold;
												} 
					table.details					{
													text-align: center;
													font-family: Verdana, Geneva, Arial, Helvetica, sans-serif ;
													font-weight: normal;
													font-size: 11px;
													color: #fff;
													width: 600px;
													background-color: #666;
													border: 0px;
													border-collapse: collapse;
													border-spacing: 0px;
												}												
				</style>
			</head>
			<body>
				<xsl:call-template name="statistics" >
					<xsl:with-param name="NumberOfTestCases" select="count(//temp:TestCaseResult)" />
					<xsl:with-param name="NumberOfPassedTestCases" select="count(//temp:TestCaseResult/temp:TestResult/text()[. = 'Passed'])" />
					<xsl:with-param name="NumberOfFailedTestCases" select="count(//temp:TestCaseResult/temp:TestResult/text()[. = 'Failed'])" />
					<xsl:with-param name="NumberOfWarningTestCases" select="count(//temp:TestCaseResult/temp:TestResult/text()[. = 'Warning'])" />
					<xsl:with-param name="NumberOfNotCompletedTestCases" select="count(//temp:TestCaseResult/temp:TestResult/text()[. = 'Not Completed'])" />
					<xsl:with-param name="Duration" select="sum(//temp:TestCaseResult/temp:Duration/text()) div 1000" />
				</xsl:call-template>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>	