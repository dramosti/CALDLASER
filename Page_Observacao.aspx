<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Page_Observacao.aspx.cs" Inherits="Page_Observacao" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Observação da Fase</title>
    <link href="Content/Site.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.7.1.js"></script>
    <script type="text/javascript">
        function BuscaObs() {
            var fase = $("#txtFase").val().split('-', 1)[0].trim();
            var ped = $("#txtPedido").val();
            var sEmpresa = getParameter("EMPRESA", window.location.href);
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "Page_Observacao.aspx/GetObs",
                data: JSON.stringify({ sFASE: fase, sPEDIDO: ped, sEMPRESA: sEmpresa }),
                dataType: "json",
                success: function (data) {
                    if (data.d != "") {
                        $("#txtObs").val(data.d);
                    }
                },
                error: function (result) {
                    alert("Error");
                }
            });
        }
        function CloseForm() {
            window.close();
        }
        function UpdateObs() {
            if ($("#txtFase").val() != "") {
                var fase = $("#txtFase").val().split('-', 1)[0].trim();
                var ped = $("#txtPedido").val();
                var valor = $("#txtObs").val();
                var sEmpresa = getParameter("EMPRESA", window.location.href);
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "Page_Observacao.aspx/UpdateObs",
                    data: JSON.stringify({ sFASE: fase, sPEDIDO: ped, sValor: valor, sEMPRESA: sEmpresa }),
                    dataType: "json",
                    success: function (data) {
                        alert("Observação salva com sucesso!");
                        window.close();
                    }
                });
            }
        }

        function getParameter(p, href) {
            var parName = p.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var rx = new RegExp("[\\?&#]" + parName + "=([^&#]*)");
            var valor = rx.exec(href);
            if (valor == null) {
                return "";
            } else {
                return valor[1];
            }
        }


    </script>

</head>
<body onload="BuscaObs()">
    <form id="form1" runat="server">
        <div>
            <table style="width: 320px">
                <tr>
                    <td>
                        <label>Pedido</label>
                        <input type="text" id="txtPedido" runat="server" disabled="disabled" value="0003357" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Fase</label>
                        <input type="text" id="txtFase" runat="server" disabled="disabled" value="00122-1234" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Observação</label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <textarea name="txtObs" id="txtObs" required="required" rows="6" style="width: 300px"></textarea>

                    </td>
                </tr>
            </table>
            <table style="width: 320px">
                <tr>
                    <td>
                        <input type="button" runat="server" id="btnSalvar" onclick="UpdateObs()" value="SALVAR" />
                    </td>
                    <td>
                        <input type="button" runat="server" id="Button1" onclick="CloseForm()" value="CANCELAR" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
