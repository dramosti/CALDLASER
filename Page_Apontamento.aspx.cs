using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HLPWEB.DAO;

public partial class Page_Apontamento : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string sEMPRESA = Request["EMPRESA"];

            if (!string.IsNullOrEmpty(sEMPRESA))
            {
                Session["Oper"] = new Operacional();
                Session["lItens"] = new List<boItensPedido>();
                Session["lFases"] = new List<boFasesPedido>();
                Session["ItemSelected"] = null;
                RefreshGrids();
                lblEmpresa.Text = sEMPRESA;
                txtCracha.Focus();
            }
            else
            {
                Response.Redirect("~/Page_SelectEmpresa.aspx");
            }

            //btnInserir.OnClientClick = "/Page_Observacao.aspx','Observacao','width=350,height=300,sc rollbars=no,resizable=no";

        }
    }



    #region Method Static to JavaScript
    [System.Web.Services.WebMethod]
    public static object[] ValidaOperador(string NR_CRACHA)
    {
        Operacional objOper = new Operacional();
        bool bValido = objOper.ValidaOperador(NR_CRACHA);
        object[] ret = null;

        if (bValido)
        {
            ret = new object[] { bValido, objOper.hlpFuncoes.NM_USUARIO };
        }
        else
        {
            ret = new object[] { bValido };
        }
        return ret;
    }

    [System.Web.Services.WebMethod]
    public static string GetDescricaoFase(string sCD_FASE, string sEMPRESA)
    {
        Operacional objOper = new Operacional();
        return objOper.GetDescricaoFase(sCD_FASE, sEMPRESA);
    }

    #endregion

    #region Eventos
    protected void btnCarregar_Click(object sender, EventArgs e)
    {
        try
        {          
                Session["lItens"] = GetItensPedidos(txtCodPedido.Value);
                Session["lFases"] = GetFasesPedido(txtCodPedido.Value);
                RefreshGrids();
                btnFinalizarFase.OnClientClick = "return confirm('DESEJA FINALIZAR A FASE ?');";
                btnIniciarFase.OnClientClick = "return confirm('DESEJA INICIAR A FASE ?');";
                btnIniciarFase.Visible = true;
                btnFinalizarFase.Visible = true;
                btnInsert.Visible = true;
                btnListaCorte.Visible = true;
                txtCodPedido.Disabled = true;
                txtCracha.Disabled = true;
                btnCarregar.Visible = false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void dgvItem_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = dgvItem.Rows[index];
            string sCodigo = row.Cells[1].Text;

            if (e.CommandName.Equals("Arquivo"))
            {
                Session["path"] = "";
                Operacional objOper = Session["Oper"] as Operacional;
                string sCD_PROD = (Session["lItens"] as List<boItensPedido>).FirstOrDefault(c => c.Codigo == sCodigo.ToString()).cd_prod;
                if (sCD_PROD != "")
                {
                    string url = "./Default.aspx?CD_PROD=" + sCD_PROD;
                    url = Page.ResolveClientUrl(url);
                    Session["path"] = objOper.GetPathFileByItem(sCD_PROD.ToString(), GetCD_EMPRESA());
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "popup", "window.open('" + url + "','toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=no, width=420,height=400,left=430,top=23')", true);
                }
                else
                {
                    ShowPopUpMsg("Nenhum caminho válido foi encontrado.", this);
                }
            }
            else if (e.CommandName.Equals("Item"))
            {
                btnCarregar.Visible = true;
                btnCarregar.Enabled = true;
                foreach (GridViewRow r in dgvItem.Rows)
                {
                    r.Cells[1].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    r.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    r.Cells[3].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    r.Cells[4].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    r.Cells[5].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                }

                row.Cells[1].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFDAB9");
                row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFDAB9");
                row.Cells[3].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFDAB9");
                row.Cells[4].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFDAB9");
                row.Cells[5].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFDAB9");

                Operacional objOper = Session["Oper"] as Operacional;
                Session["ItemSelected"] = (Session["lItens"] as List<boItensPedido>).FirstOrDefault(c => c.Codigo == sCodigo).cd_prod;

                List<string> lsFases = objOper.GetFaseByItem(Session["ItemSelected"].ToString(), GetCD_EMPRESA());

                List<boFasesPedido> lFasesByItem = (from x in (Session["lFases"] as List<boFasesPedido>)
                                                    where lsFases.Contains(x.Fase)
                                                    select x).ToList();

                dgvFases.DataSource = lFasesByItem;
                dgvFases.DataBind();
                ColoreStatusGrid();

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    protected void dgvFases_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (Session["ItemSelected"] != null)
        {
            Operacional objOper = Session["Oper"] as Operacional;
            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = dgvFases.Rows[index];
            string sFase = row.Cells[0].Text;
            string sPath = objOper.GetPathFileByFase(Session["ItemSelected"].ToString(), sFase, GetCD_EMPRESA());
            if (sPath != "")
            {
                string url = "./Default.aspx?PATH=" + sPath;
                Response.Redirect(url);
            }
            else
            {
                ShowPopUpMsg("Nenhum caminho válido foi encontrado.", this);
            }
        }
        else
        {
            ShowPopUpMsg("Nenhum item do pedido foi selecionado, impossível localizar o arquivo", this);
        }
    }
    protected void btnIniciarFase_Click(object sender, EventArgs e)
    {
        try
        {
            List<boFasesPedido> lFases = Session["lFases"] as List<boFasesPedido>;
            string sCD_FASE = txtFase.Value.Split('-')[0].Trim();

            if (lFases.Where(c => c.Fase == sCD_FASE).Count() > 0)
            {
                string sSit = lFases.FirstOrDefault(c => c.Fase == sCD_FASE).Situacao;
                if (sSit.Equals("N_INICIADO"))
                {
                    string sCD_PEDIDO = txtCodPedido.Value;
                    string sNM_OPER = txtOperador.Value;

                    string sCD_OS = "";
                    string sNR_FASEPOS = "";
                    string sCD_MAQUINA = "";
                    string sDT_ATUAL = DateTime.Today.ToString("dd.MM.yyyy");

                    Operacional objOper = Session["Oper"] as Operacional;
                    foreach (boItensPedido item in (Session["lItens"] as List<boItensPedido>))
                    {
                        if (objOper.GetFaseByItem(item.cd_prod, GetCD_EMPRESA()).Contains(sCD_FASE))
                        {
                            sCD_OS = item.OP;
                            sNR_FASEPOS = objOper.GetNR_FASEPOS(sCD_OS, sCD_FASE, GetCD_EMPRESA());
                            sCD_MAQUINA = objOper.GetCD_MAQUINA(sCD_OS, sCD_FASE, GetCD_EMPRESA());

                            sCD_MAQUINA = sCD_MAQUINA == "" ? "null" : "'" + sCD_MAQUINA + "'";

                            objOper.IniciaFase(sNM_OPER,
                                                 sCD_PEDIDO,
                                                 sCD_FASE,
                                                 "0",
                                                 "0",
                                                 sCD_OS, sDT_ATUAL, sDT_ATUAL, "01", sNR_FASEPOS, sCD_MAQUINA, "null", GetCD_EMPRESA());
                        }
                    }
                    Session["lFases"] = GetFasesPedido(txtCodPedido.Value);
                    RefreshGrids();
                }
                else
                {
                    ShowPopUpMsg("SITUAÇÃO ATUAL DA FASE É IGUAL A '" + sSit + "', NÃO PODE SER INICIADA.", this);
                }
            }
            else
            {
                ShowPopUpMsg("Fase não encontrada no Pedido", this);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    protected void btnFinalizarFase_Click(object sender, EventArgs e)
    {
        try
        {
            List<boFasesPedido> lFases = Session["lFases"] as List<boFasesPedido>;
            string sCD_FASE = txtFase.Value.Split('-')[0].Trim();

            if (lFases.Where(c => c.Fase == sCD_FASE).Count() > 0)
            {
                string sSit = lFases.FirstOrDefault(c => c.Fase == sCD_FASE).Situacao;
                if (sSit.Equals("INICIADO"))
                {
                    string sCD_PEDIDO = txtCodPedido.Value;
                    string sNM_OPER = txtOperador.Value;

                    string sCD_OS = "";
                    string sNR_FASEPOS = "";
                    string sCD_MAQUINA = "";
                    string sDT_ATUAL = DateTime.Today.ToString("dd.MM.yyyy");

                    Operacional objOper = Session["Oper"] as Operacional;
                    foreach (boItensPedido item in (Session["lItens"] as List<boItensPedido>))
                    {
                        if (objOper.GetFaseByItem(item.cd_prod, GetCD_EMPRESA()).Contains(sCD_FASE))
                        {
                            sCD_OS = item.OP;
                            sNR_FASEPOS = objOper.GetNR_FASEPOS(sCD_OS, sCD_FASE, GetCD_EMPRESA());
                            sCD_MAQUINA = objOper.GetCD_MAQUINA(sCD_OS, sCD_FASE, GetCD_EMPRESA());

                            sCD_MAQUINA = sCD_MAQUINA == "" ? "null" : "'" + sCD_MAQUINA + "'";

                            objOper.IniciaFase(sNM_OPER,
                                                 sCD_PEDIDO,
                                                 sCD_FASE,
                                                 "0",
                                                 "1",
                                                 sCD_OS, sDT_ATUAL, sDT_ATUAL, "03", sNR_FASEPOS, sCD_MAQUINA, "'" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "'", GetCD_EMPRESA());
                        }
                    }
                    Session["lFases"] = GetFasesPedido(txtCodPedido.Value);
                    RefreshGrids();
                }
                else
                {
                    ShowPopUpMsg("SITUAÇÃO ATUAL DA FASE É IGUAL A '" + sSit + "', NÃO PODE SER FINALIZADA.", this);
                }
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion

    #region Methods
    public List<boItensPedido> GetItensPedidos(string sCD_PEDIDO)
    {
        Operacional objOper = Session["Oper"] as Operacional;
        List<boItensPedido> lRetorno = new List<boItensPedido>();
        DataTable dtRet = objOper.GetItensPedidos(sCD_PEDIDO, GetCD_EMPRESA());

        if (dtRet.Rows.Count > 0)
        {
            lRetorno = dtRet.AsEnumerable().Select(item => new boItensPedido()
            {
                cd_prod = item["cd_prod"].ToString(),
                Codigo = item["cd_alter"].ToString(),
                Descricao = item["ds_prod"].ToString(),
                OP = item["cd_os"].ToString(),
                Qtd = Convert.ToInt32(item["qt_prod"].ToString()),
                Situacao = (item["st_os"].ToString()),
                Un = item["cd_tpunid"].ToString()
            }).ToList();
        }
        return lRetorno;
    }
    public List<boFasesPedido> GetFasesPedido(string sCD_PEDIDO)
    {
        Operacional objOper = Session["Oper"] as Operacional;
        List<boFasesPedido> lRetorno = new List<boFasesPedido>();
        DataTable dtRet = objOper.GetFaseByPedido(sCD_PEDIDO, GetCD_EMPRESA());

        if (dtRet.Rows.Count > 0)
        {
            lRetorno = dtRet.AsEnumerable().Select(item => new boFasesPedido()
            {
                Descricao = item["ds_fase"].ToString(),
                Fase = item["cd_fase"].ToString(),
                Previsao = objOper.GetDT_FINALPREVFase(item["cd_fase"].ToString(), sCD_PEDIDO, GetCD_EMPRESA()),
                Realizado = objOper.GetDT_FINALFase(item["cd_fase"].ToString(), sCD_PEDIDO, GetCD_EMPRESA()),
                Situacao = objOper.GetStatusFase(item["cd_fase"].ToString(), sCD_PEDIDO, GetCD_EMPRESA())
            }).ToList();
        }
        return lRetorno;
    }
    private static void ShowPopUpMsg(string msg, Page objPage)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("alert('");
        sb.Append(msg.Replace("\n", "\\n").Replace("\r", "").Replace("'", "\\'"));
        sb.Append("');");
        System.Web.UI.ScriptManager.RegisterStartupScript(objPage, objPage.GetType(), "showalert", sb.ToString(), true);
    }
    private void RefreshGrids()
    {
        dgvItem.DataSource = Session["lItens"];
        dgvItem.DataBind();

        dgvFases.DataSource = Session["lFases"];
        dgvFases.DataBind();

        ColoreStatusGrid();
    }
    private void ColoreStatusGrid()
    {
        foreach (GridViewRow row in dgvItem.Rows)
        {
            if (row.Cells[6].Text.Trim().Equals("INICIADO"))
            {
                row.Cells[6].BackColor = System.Drawing.ColorTranslator.FromHtml("#00FFFF");//AZUL
            }
            else if (row.Cells[6].Text.Trim().Equals("FINALIZADO"))
            {
                row.Cells[6].BackColor = System.Drawing.ColorTranslator.FromHtml("#B9FFB9");//VERDER
            }
            else
            {
                row.Cells[6].BackColor = System.Drawing.ColorTranslator.FromHtml("#FF9595");//VERMELHO
            }
        }

        foreach (GridViewRow row in dgvFases.Rows)
        {
            if (row.Cells[4].Text.Trim().Equals("INICIADO"))
            {
                row.Cells[4].BackColor = System.Drawing.ColorTranslator.FromHtml("#00FFFF");//AZUL
            }
            else if (row.Cells[4].Text.Trim().Equals("FINALIZADO"))
            {
                row.Cells[4].BackColor = System.Drawing.ColorTranslator.FromHtml("#B9FFB9");//VERDER
            }
            else
            {
                row.Cells[4].BackColor = System.Drawing.ColorTranslator.FromHtml("#FF9595");//VERMELHO
            }
        }
        if (dgvFases.Rows.Count > 0)
            lblTotFases.Text = "Total de Fases: " + dgvFases.Rows.Count.ToString();
    }
    #endregion


    private string GetCD_EMPRESA()
    {
        try
        {
            string sEmpresa = "";
            if (lblEmpresa.Text != "")
            {
                sEmpresa = lblEmpresa.Text.Split('-')[0].Trim();
            }
            return sEmpresa;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }






}
