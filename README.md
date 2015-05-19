# NLog WebViewer

Web interface for viewing NLog files with specific format on remote server.

## Usage sample

Add in web.config `configSections`

    <section name="nlog.webviewer" type="NLog.WebViewer.Config, NLog.WebViewer" />
    
Sample configuration section:

    <nlog.webviewer
        Fields="Date|Level|Logger|IpAddress|Message"
        Separator="|"
        FileFormat="{0:yyyy-MM-dd}.txt"
        Path="~/App_Data/Logs" />

is valid for following NLog target:

    <target name="logfile" type="File" encoding="utf-8"
        fileName="${basedir}/App_Data/Logs/${shortdate}.txt"
        layout="${longdate}|${uppercase:${level}}|${logger}|${aspnet-request:serverVariable=remote_addr}|${message}${onexception:${newline}${exception:format=tostring}}" />


Following handler in `system.webServer\handlers` could be used to access logs page:

    <add name="LogsViewHandler" verb="*" path="Logs.axd"
         type="NLog.WebViewer.LogViewerHandler, NLog.WebViewer"
         resourceType="Unspecified" requireAccess="Read" />

Then log page will be available via link: `your_site_name/Logs.axd`
