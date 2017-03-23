<%@ Page Language="C#" AutoEventWireup="true" %>
<script type="text/C#" runat="server">
    protected string GetServerAppN()
    {
        string temp;
        try
        {
            temp = ((double)GC.GetTotalMemory(false) / 1048576).ToString("N2") + "M";
        }
        catch
        {
            temp = "未知";
        }
        return temp;
    }
    protected string GetPrStart()
    {
        string temp;
        try
        {
            temp = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm");
        }
        catch
        {
            temp = "未知";
        }
        return temp;
    }
</script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
</head>
<body>
    <div style="line-height: 20px;">
        <%
            var sb = new StringBuilder();
            sb.AppendLine(".NET解释引擎版本：" + ".NET CLR" + Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." + Environment.Version.Revision);//.NET解释引擎版本  
            sb.AppendLine("<br />服务器操作系统版本：" + Environment.OSVersion);//服务器操作系统版本  
            sb.AppendLine("<br />服务器IIS版本：" + Request.ServerVariables["SERVER_SOFTWARE"]);//服务器IIS版本  
            sb.AppendLine("<br />服务器区域语言：" + Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"]);//服务器区域语言  
            sb.AppendLine("<br />用户浏览器信息：" + Request.ServerVariables["HTTP_USER_AGENT"]);
            sb.AppendLine("<br />CPU个数：" + Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));//CPU个数  
            sb.AppendLine("<br />CPU类型：" + Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"));//CPU类型  
            sb.AppendLine("<br />进程开始时间：" + GetPrStart());//进程开始时间  
            sb.AppendLine("<br />应用程序占用内存：" + GetServerAppN());//应用程序占用内存 
        
            Response.Write(sb.ToString());
        %>
    </div>
</body>
</html>
