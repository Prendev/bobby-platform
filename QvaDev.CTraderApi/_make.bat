@rem %WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe cTraderTest.csproj
@rem %WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe cTraderApi.csproj
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe cTraderApiTest.csproj
@rem %WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe -r:bin\Debug\Google.ProtocolBuffers.dll -r:bin\Debug\cTraderApi.dll -out:bin\Debug\cTraderApiTest.exe cTraderApiTest.cs
 