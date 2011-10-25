rem @echo off
set PATH=C:\Program Files\TortoiseSVN\bin\;C:\Program Files\subversion\bin;%PATH%
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2\Actions\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2\Internal\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2\Matchers\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2\Monitoring\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2\Properties\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2\Syntax\*.cs

svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.AcceptanceTests\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.AcceptanceTests\Properties\*.cs

svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.Test\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.Test\Actions\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.Test\Internal\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.Test\Matchers\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.Test\Monitoring\*.cs
svn propset svn:keywords "LastChangedBy LastChangedDate LastChangedRevision HeadURL Id" NMock2.Test\Properties\*.cs


pause
