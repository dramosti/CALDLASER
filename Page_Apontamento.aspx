<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Page_Apontamento.aspx.cs" Inherits="Page_Apontamento" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Apontamento de Ordem de Produção</title>
    <link href="Content/Site.css" rel="stylesheet" />
    <link href="Content/Grid.css" rel="stylesheet" />
    <script src="Scripts/jquery-1.7.1.js"></script>
    <script type="text/javascript" src="Scripts/idle-timer.min.js"></script>

    <script type="text/javascript">
        (function ($) {
            var timeout = 50000;
            $(document).bind("idle.idleTimer", function () {
                // $("#status").html("User is idle :(").css("backgroundColor", "silver");
                window.location = location.href;
            });
            $(document).bind("active.idleTimer", function () {
                // $("#status").html("User is active :D").css("backgroundColor", "yellow");                
            });
            $.idleTimer(timeout);
            $('#timeout').text(timeout / 1000);
        })(jQuery);
    </script>

    <script type="text/javascript">


        function padLeft(nr, n, str) {
            return Array(n - String(nr).length + 1).join(str || '0') + nr;
        }

        var params;
        function ShowPopup() {
            if ($("#txtCodPedido").val() != "") {
                var sEmpresa = document.getElementById('<%=lblEmpresa.ClientID%>').innerText.split('-')[0].trim();
                var sPage = "Page_Observacao.aspx?PEDIDO=" + $("#txtCodPedido").val() + "&FASE=" + $("#txtFase").val() + "&EMPRESA=" + sEmpresa;
                showModalDialog(sPage, window, 'help:no;status:no;scroll:yes;edge:raised;dialogWidth:' + 450 + 'px;edge:raised;dialogHeight:' + 450 + 'px')
            }
        };

        $(document).ready(function () {
            $("#txtCracha").blur(function () {
                if ($("#txtCracha").val() != "") {
                    var nr_cracha = padLeft($("#txtCracha").val(), 15, '0');
                    params = '{"NR_CRACHA":"' + nr_cracha + '"}';
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "Page_Apontamento.aspx/ValidaOperador",
                        data: params,
                        dataType: "json",
                        success: function (data) {
                            if (data.d[0] == true) {
                                $("#txtOperador").val(data.d[1]);
                                $("#txtCracha").val(nr_cracha);
                                $("#txtCodPedido").focus();
                                document.getElementById('<%=txtCracha.ClientID%>').disabled = true;

                            } else {
                                $("#txtCracha").val("");
                                $("#txtOperador").val("");
                                $("#txtCracha").focus();
                            }
                        },
                        error: function (result) {
                            alert("Error");
                        }
                    });
                }
            });


            $("#txtCodPedido").blur(function (e) {
                //if (e.char != "\b" && $("#txtCodPedido").val().length == 11) {
                if ($("#txtCodPedido").val().length > 0) {
                    var sValor = padLeft($("#txtCodPedido").val(), 12, '0');
                    var sEmpresa = document.getElementById('<%=lblEmpresa.ClientID%>').innerText.split('-')[0].trim();
                    var sFase = sValor.substr(7, 12);
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "Page_Apontamento.aspx/GetDescricaoFase",
                        data: JSON.stringify({ sCD_FASE: sFase, sEMPRESA: sEmpresa }),
                        dataType: "json",
                        success: function (data) {
                            if (data.d != "") {
                                $("#txtCodPedido").val(sValor.substr(0, 7)); //valor inteiro com o padleft
                                sValor = sValor.toString().substring(7, 12) + " - " + data.d; // codigo da fase + descrição
                                $("#txtFase").val(sValor.toString());
                                //document.getElementById('<%=txtCodPedido.ClientID%>').disabled = true;
                                //$("#btnCarregar").visible = true;
                                document.getElementById('<%=btnCarregar.ClientID%>').disabled = false;
                                document.getElementById('<%=btnCarregar.ClientID%>').focus();

                            } else {
                                $("#txtFase").val("");
                                $("#txtCodPedido").val("");
                                $("#txtCodPedido").focus();
                            }


                            //btnCarregar
                        },
                        error: function (result) {
                            alert("Error");
                        }
                    });

                } else {
                    $("#txtFase").val("");
                }
            });
        });


    </script>



    <style type="text/css">
        
        .auto-style2
        {
            width: 293px;
        }

        .auto-style3
        {
            width: 140px;
        }

        .auto-style4
        {
            width: 143px;
        }

        .auto-style5
        {
            width: 465px;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server" style="margin: 0% 3% 80% 3%;">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <fieldset id="fildCabecalho">
                        <table style="width: 100%">
                            <tr class="BordaInferior">  
                                <td class="Titulo" style="text-align: left; color: Black">APONTAMENTO</td>
                            </tr>
                        </table>
                        <table id="Table1" runat="server" style="width: 100%">
                            <tr>
                                <td class="auto-style4">
                                    <label>Numero Cracha</label>
                                    <input type="text" id="txtCracha" maxlength="15" runat="server" required="required" style="width: 150px" />
                                </td>
                                <td class="auto-style2">
                                    <label>Operador</label>
                                    <input type="text" id="txtOperador" runat="server" required="required" disabled="disabled" maxlength="15" />
                                </td>
                                <td class="auto-style3">
                                    <label>Pedido/OS</label>
                                    <input type="text" maxlength="12" required="required" id="txtCodPedido" runat="server" style="width: 150px" />
                                </td>
                                <td class="auto-style5">
                                    <label>Fase</label>
                                    <input type="text" id="txtFase" required="required" disabled="disabled" runat="server" style="width: 100%" />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 85%">
                                            <table style="width: 100%">
                                                <tr class="BordaInferior">
                                                    <td class="Titulo" style="text-align: left; color: Black">Itens do Pedido / OS
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>

                                                        <asp:GridView ID="dgvItem" Style="width: 100%"
                                                            runat="server"
                                                            CssClass="mGrid"
                                                            PagerStyle-CssClass="pgr"
                                                            Font-Names="Segoe UI"
                                                            Font-Size="13px"
                                                            AlternatingRowStyle-CssClass="alt"
                                                            AutoGenerateColumns="False"
                                                            Width="800px"
                                                            AllowPaging="True"
                                                            ShowHeaderWhenEmpty="True" DataKeyNames="Codigo" OnRowCommand="dgvItem_RowCommand">
                                                            <AlternatingRowStyle CssClass="alt" />
                                                            <Columns>
                                                                <asp:ButtonField CommandName="Item" Text="Item">
                                                                    <ControlStyle BackColor="Transparent" BorderStyle="None" Font-Names="Segoe UI" Font-Size="13px" />
                                                                    <ItemStyle BorderStyle="None" Font-Underline="True" ForeColor="Blue" />
                                                                    <HeaderStyle Width="20px" />
                                                                </asp:ButtonField>
                                                                <asp:BoundField DataField="Codigo" HeaderText="Codigo" SortExpression="Codigo" />
                                                                <asp:BoundField DataField="Descricao" HeaderText="Descricao" SortExpression="Descricao" />
                                                                <asp:BoundField DataField="Un" HeaderText="Un" SortExpression="Un" />
                                                                <asp:BoundField DataField="Qtd" HeaderText="Qtd" SortExpression="Qtd" />
                                                                <asp:BoundField DataField="OP" HeaderText="OP" SortExpression="OP" />
                                                                <asp:BoundField DataField="Situacao" HeaderText="Situacao" SortExpression="Situacao">
                                                                    <ItemStyle Width="100px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="cd_prod" HeaderText="cd_prod" SortExpression="cd_prod" Visible="false" />
                                                                <asp:ButtonField CommandName="Arquivo" Text="Arquivo">
                                                                    <ControlStyle BackColor="Transparent" BorderStyle="None" Font-Names="Segoe UI" Font-Size="13px" />
                                                                    <ItemStyle BorderStyle="None" Font-Underline="True" ForeColor="Blue" />
                                                                    <HeaderStyle Width="20px" />
                                                                </asp:ButtonField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr class="BordaInferior">
                                                    <td class="Titulo" style="text-align: left; color: Black">Fases do Pedido / OS
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="dgvFases" Style="width: 100%"
                                                            runat="server"
                                                            CssClass="mGrid"
                                                            PagerStyle-CssClass="pgr"
                                                            Font-Names="Segoe UI"
                                                            Font-Size="13px"
                                                            AlternatingRowStyle-CssClass="alt"
                                                            AutoGenerateColumns="False"
                                                            Width="800px"
                                                            AllowPaging="True"
                                                            ShowHeaderWhenEmpty="True" OnRowCommand="dgvFases_RowCommand">
                                                            <AlternatingRowStyle CssClass="alt" />
                                                            <Columns>
                                                                <asp:BoundField DataField="Fase" HeaderText="Fase" SortExpression="Fase">
                                                                    <ItemStyle Width="80px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Descricao" HeaderText="Descricao" SortExpression="Descricao" />
                                                                <asp:BoundField DataField="Previsao" HeaderText="Previsao" SortExpression="Previsao">
                                                                    <ItemStyle Width="140px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Realizado" HeaderText="Realizado" SortExpression="Realizado">
                                                                    <ItemStyle Width="140px" />
                                                                </asp:BoundField>
                                                                <asp:BoundField DataField="Situacao" HeaderText="Situacao" SortExpression="Situacao">
                                                                    <ItemStyle Width="100px" />
                                                                </asp:BoundField>
                                                                <asp:ButtonField CommandName="Arquivo" Text="Arquivo">
                                                                    <ControlStyle BackColor="Transparent" BorderStyle="None" Font-Names="Segoe UI" Font-Size="13px" />
                                                                    <ItemStyle BorderStyle="None" Font-Underline="True" ForeColor="Blue" />
                                                                    <HeaderStyle Width="20px" />
                                                                </asp:ButtonField>
                                                            </Columns>
                                                            <PagerStyle CssClass="pgr" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblTotFases" runat="server" Text=""></asp:Label>
                                                    </td>
                                                </tr>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td style="right: auto">
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="btnCarregar" runat="server"  Enabled="false" Text="Carregar" OnClick="btnCarregar_Click" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="btnListaCorte" Visible="false" runat="server" Text="Lista de Corte" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input type="button" id="btnInsert" value="Inserir Obs" visible="false" onclick="ShowPopup()" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="btnIniciarFase" runat="server" visible="false" Text="Iniciar Fase" OnClick="btnIniciarFase_Click" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                                <ContentTemplate>
                                                    <asp:Button ID="btnFinalizarFase" runat="server" Visible="false" Text="Finalizar Fase" OnClick="btnFinalizarFase_Click" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                            <label style="color: #FF0000">F5-Reiniciar</label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <footer style="text-align: right">
                        <asp:Label ID="lblEmpresa" runat="server" Text="Empresa" Font-Size="14px"></asp:Label>
                        <a href ="Page_SelectEmpresa.aspx" style="color: #FF0000" > ( MUDAR )</a>
                    </footer>



                </ContentTemplate>
            </asp:UpdatePanel>
            <div style="position: ">
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1"
                    DynamicLayout="true">
                    <ProgressTemplate>
                        <asp:Label ID="Label1" runat="server" CssClass="label" Style="position: absolute; left: 71%">Carregando...</asp:Label>
                        <br />
                        <%--  <img src="Images/ajax-loader.gif" style="position: absolute; left: 72%" />--%>
                        <img src="Images/balls64.gif" style="position: absolute; left: 72%" />
                    </ProgressTemplate>
                </asp:UpdateProgress>


            </div>
    </form>
</body>

</html>
