using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using HLPWEB.DAO;

public partial class Page_SelectEmpresa : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ddlEmpresas.Focus();
       
    }

    [WebMethod]
    public static List<string> GetListEmpresas() 
    {
        Operacional objOper = new Operacional();
        return objOper.hlpFuncoes.GetListEmpresas();
    }
}