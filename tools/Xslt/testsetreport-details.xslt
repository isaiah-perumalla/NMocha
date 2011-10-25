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
					blockquote, pre 			{
													background:#F4F5F7 3px 3px no-repeat;
													border:2px dashed #CCC;
													padding:8px 8px 8px 8px;
													margin:5px 0;
												}
					a:link, a:visited 			{
													text-decoration:none;
													color:#000;
												}
					a:link 						{
													border-bottom:1px dotted #4A4A4A;
												}
					a:visited 					{
													color:#000;
													border-bottom:1px dotted #4A4A4A;
												}
					a:hover 					{
													color:#000;
													font-weight:bold;
													border-bottom:1px solid #4A4A4A;
												}
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
													width: 700px;
													background-color: #666;
													border: 0px;
													border-collapse: collapse;
													border-spacing: 0px;
												}
					table.details td				{
													background-color: #CCC;
													color: #000;
													padding: 4px;
													text-align: left;
													border: 1px #fff solid;
												}
					table.details td.hed
												{
													background-color: #666;
													color: #fff;
													padding: 4px;
													text-align: left;
													border-bottom: 2px #fff solid;
													font-size: 12px;
													font-weight: bold;
												} 	
					table.details td.subhed
												{
													background-color: #999;
													color: #fff;
													padding: 4px;
													text-align: left;
													border-bottom: 2px #fff solid;
													font-size: 12px;
													font-weight: bold;
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

				<xsl:call-template name="details" >
					<xsl:with-param name="TestCases" select="//temp:TestCaseResult" />
				</xsl:call-template>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>	