using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using HLPWEB.DAO;

public partial class Page_Observacao : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(Request["PEDIDO"]))
            {
                string sCD_PEDIDO = Request["PEDIDO"].ToString();
                string sFASE = Request["FASE"].ToString();

                txtPedido.Value = sCD_PEDIDO;
                txtFase.Value = sFASE;
            }
        }
    }
    [WebMethod]
    public static string GetObs(string sFASE, string sPEDIDO, string sEMPRESA)
    {

        Operacional objOper = new Operacional();
        return objOper.GetObsFase(sPEDIDO, sFASE,sEMPRESA);
    }

    [WebMethod]
    public static void UpdateObs(string sFASE, string sPEDIDO, string sValor, string sEMPRESA)
    {
        Operacional objOper = new Operacional();
        objOper.AlterObsFase(sValor, sPEDIDO, sFASE, sEMPRESA);
    }

}