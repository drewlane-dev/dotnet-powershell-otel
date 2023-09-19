@{
    ModuleVersion = '1.0.31'
    Author = 'Drew Lane'
    Description = 'Dotnet Powershell Otel Module'
    CmdletsToExport = @('Get-OtelTracer', 'Start-OtelSpan', 'Stop-OtelSpan', 'Add-OtelEvent')
    RootModule = 'DotnetPowershellOtel.dll'
}