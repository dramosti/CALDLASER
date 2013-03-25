<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Page_SelectEmpresa.aspx.cs" Inherits="Page_SelectEmpresa" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Content/Site.css" rel="stylesheet" />
    <link href="Content/Grid.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.7.1.js"></script>
    <title>Empresa</title>
    <script type="text/javascript">

        function Iniciar() {
            var mylist = document.getElementById("ddlEmpresas");
            var sEmpresa = mylist.options[mylist.selectedIndex].text;
            var urlApontamento = "Page_Apontamento.aspx?EMPRESA=" + sEmpresa;
            window.location.href = urlApontamento;
        };


        function CarregaDropList() {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "Page_SelectEmpresa.aspx/GetListEmpresas",
                //data: JSON.stringify({ sFASE: fase, sPEDIDO: ped }),
                dataType: "json",
                success: function (data) {
                    for (var i = 0; i < data.d.length; i++) {
                        var opt = new Option(data.d[i]);
                        document.getElementById("ddlEmpresas").options[i] = opt;
                    }
                },
                error: function (result) {
                    alert("Error");
                }
            });
        }


        window.onload = CarregaDropList();
    </script>
</head>
<body>
    <form id="form1" runat="server" style="position: absolute; left: 35%; top: 30%;">
        <div>

            <table>
                <tr>
                    <td >
                        <label style="text-align: center; font-size: 20px; color: #0000FF;" >Apontamento de Ordem de Produção</label>
                        <br />
                        <br />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Selecione a Empresa</label>
                        <select id="ddlEmpresas" runat="server" class="dropList">
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input type="button" id="btnIni" onclick="Iniciar()" runat="server" value="INICIAR" />
                    </td>
                </tr>
            </table>

        </div>
    </form>
</body>
</html>
