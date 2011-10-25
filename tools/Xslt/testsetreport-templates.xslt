<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:temp="http://tempuri.org/TestSetReport.xsd" >	
	<xsl:template name="statistics">
		<xsl:param name="NumberOfTestCases" />
		<xsl:param name="NumberOfPassedTestCases" />
		<xsl:param name="NumberOfFailedTestCases" />
		<xsl:param name="NumberOfWarningTestCases" />
		<xsl:param name="NumberOfNotCompletedTestCases" />
		<xsl:param name="Duration" />
		<p>
			<table class="stats" cellpadding="2" cellspacing="0">
				<tbody>
					<tr>
						<td class="hed" colspan="4">Testreport statistics</td>
					</tr>
					<tr>
						<td>Total number of test cases:</td>
						<td>
							<xsl:value-of select="$NumberOfTestCases" />
						</td>
					</tr>
					<tr>
						<td>Total number of passed test cases:</td>
						<td>
							<xsl:value-of select="$NumberOfPassedTestCases" />
						</td>
					</tr>
					<tr>
						<td>Total number of failed test cases:</td>
						<td>
							<xsl:value-of select="$NumberOfFailedTestCases" />
						</td>
					</tr>
					<tr>
						<td>Total number of warning test cases:</td>
						<td>
							<xsl:value-of select="$NumberOfWarningTestCases" />
						</td>
					</tr>
					<tr>
						<td>Total number of not completed test cases:</td>
						<td>
							<xsl:value-of select="$NumberOfNotCompletedTestCases" />
						</td>
					</tr>
					<tr>
						<td>Duration (in seconds):</td>
						<td>
							<xsl:value-of select="$Duration" />
						</td>
					</tr>
				</tbody>
			</table>
		</p>
	</xsl:template>

	<xsl:template name="details">
		<xsl:param name="TestCases" />
		<p>
			<table class="details" cellpadding="2" cellspacing="0">
				<tbody>
					<tr>
						<td class="hed" colspan="4">Testreport details</td>
					</tr>
					<tr>
						<td class="subhed">Test name</td>
						<td class="subhed">Duration</td>
						<td class="subhed">Results</td>
					</tr>
					<xsl:for-each select="$TestCases">
						<xsl:sort select="./temp:TestName"/>
						<xsl:sort select="./temp:TestResult"/>
						<tr>
							<td>
								<xsl:variable name="path" select="./temp:DetailReportPath"/>
								<a href="{$path}" target="_blank">
									<xsl:value-of select="./temp:TestName" />
								</a>
								<xsl:if test="./temp:Comment/text()[. != '']">
									<br />
									<blockquote>
										<xsl:value-of select="./temp:Comment" />
									</blockquote>
								</xsl:if>
							</td>
							<td width="100">
								<xsl:value-of select="./temp:Duration" />
							</td>
							<xsl:if test="./temp:TestResult/text()[. = 'Passed']">
								<td width="100" style="background-color:GreenYellow ">
									<xsl:value-of select="./temp:TestResult" />
								</td>
							</xsl:if>
							<xsl:if test="./temp:TestResult/text()[. = 'Failed']">
								<td width="100" style="background-color:LightCoral  ">
									<xsl:value-of select="./temp:TestResult" />
								</td>
							</xsl:if>
							<xsl:if test="./temp:TestResult/text()[. = 'Warning']">
								<td width="100" style="background-color:Gold">
									<xsl:value-of select="./temp:TestResult" />
								</td>
							</xsl:if>
							<xsl:if test="./temp:TestResult/text()[. = 'Not Completed']">
								<td width="100" style="background-color:Tomato">
									<xsl:value-of select="./temp:TestResult" />
								</td>
							</xsl:if>
						</tr>
					</xsl:for-each>
				</tbody>
			</table>
		</p>
	</xsl:template>
</xsl:stylesheet>	