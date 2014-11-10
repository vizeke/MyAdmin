<%
        Set WSHProcess = CreateObject("Wscript.Shell").Environment("Process")
%>
<html>
<head><title></title></head>
<body style="text-align: center;">
    <br>
    <br>
    <br>
    <h1>
        <strong>
            <%
                Response.Write "Servidor: " & WSHProcess("COMPUTERNAME")
            %>
        </strong>
    </h1>
    <br />
    <br />
    <h1>
        <strong>
            <%
                Response.Write "Data: " & Now() 
            %>
        </strong>
    </h1>
</body>
</html>
