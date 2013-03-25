using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace HLPWEB.DAO
{
    public class Operacional
    {
        public Funcoes hlpFuncoes { get; set; }

        public Operacional()
        {
            hlpFuncoes = new Funcoes();
            hlpFuncoes.Inicializacao();
        }

        public bool ValidaOperador(string sNR_CRACHA)
        {
            bool bValida = false;
            int iCount = Convert.ToInt32(this.hlpFuncoes.qrySeekValue("ACESSO", "COUNT(*)TOT", "nr_cracha = '" + sNR_CRACHA + "'"));
            if (iCount > 0)
            {
                bValida = true;
                this.hlpFuncoes.CD_USUARIO = this.hlpFuncoes.qrySeekValue("ACESSO", "COALESCE(cd_operado,'')", "nr_cracha = '" + sNR_CRACHA + "'");
                this.hlpFuncoes.NM_USUARIO = this.hlpFuncoes.qrySeekValue("ACESSO", "COALESCE(nm_operado,'')", "nr_cracha = '" + sNR_CRACHA + "'");
            }
            else
            {
                this.hlpFuncoes.CD_USUARIO = null;
                this.hlpFuncoes.NM_USUARIO = null;
                bValida = false;
            }
            return bValida;
        }



        public DataTable GetItensPedidos(string sCD_PEDIDO, string sCD_EMPRESA)
        {
            StringBuilder sQuery = new StringBuilder();
            sQuery.Append("select P.cd_prod, P.cd_alter, P.ds_prod, P.cd_tpunid, O.qt_prod, O.cd_os, O.st_os ");
            sQuery.Append("from ORDEMP O inner join PRODUTO P on O.cd_prod = P.cd_prod ");
            sQuery.Append("where O.cd_pedido = '{0}' and O.cd_empresa = '{1}' ");

            string sQueryFim = string.Format(sQuery.ToString(), sCD_PEDIDO, sCD_EMPRESA);

            return this.hlpFuncoes.qrySeekRet(sQueryFim);
        }

        public string GetDescricaoFase(string sCD_FASE, string sCD_EMPRESA)
        {
            return hlpFuncoes.qrySeekValue("FASES", "ds_fase", string.Format("cd_fase ='{0}' and cd_empresa = '{1}'", sCD_FASE, sCD_EMPRESA));
        }

        /// <summary>
        /// Obtem todas as fases em comum dos item contidos em um determinado pedido
        /// </summary>
        /// <param name="sCD_PEDIDO"></param>
        /// <returns></returns>
        public DataTable GetFaseByPedido(string sCD_PEDIDO, string sCD_EMPRESA)
        {
            StringBuilder sQuery = new StringBuilder();
            sQuery.Append("select PED.cd_pedido, fped.cd_fase, FASES.ds_fase from FASESPED FPED ");
            sQuery.Append("inner join FASES on FPED.cd_fase = FASES.cd_fase and FPED.cd_empresa = FASES.cd_empresa ");
            sQuery.Append("inner join pedido PED on FPED.cd_pedido = PED.cd_pedido ");
            sQuery.Append("where FPED.cd_pedido = '{0}' and FPED.cd_empresa = '{1}' ");

            string sQueryFim = string.Format(sQuery.ToString(), sCD_PEDIDO, sCD_EMPRESA);

            return this.hlpFuncoes.qrySeekRet(sQueryFim);
        }

        /// <summary>
        /// Obtem todas as fases de um determinado produto
        /// </summary>
        /// <param name="sCD_PROD"></param>
        /// <returns></returns>
        public List<string> GetFaseByItem(string sCD_PROD, string sCD_EMPRESA)
        {

            string sQueryFim = string.Format("select f.cd_fase from faseprod f where f.cd_prod = '{0}' and f.cd_empresa = '{1}'", sCD_PROD, sCD_EMPRESA);

            DataTable dt = hlpFuncoes.qrySeekRet(sQueryFim);
            List<string> lRet = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                lRet.Add(row[0].ToString());
            }
            return lRet;

        }

        public string GetNR_FASEPOS(string sCD_OS, string sCD_FASE, string sCD_EMPRESA)
        {
            return hlpFuncoes.qrySeekValue("FASEPOS", "nr_fasepos", string.Format("cd_os = '{0}' and cd_fase = '{1}' and cd_empresa = '{2}'", sCD_OS, sCD_FASE, sCD_EMPRESA));
        }

        public string GetCD_MAQUINA(string sCD_OS, string sCD_FASE, string sCD_EMPRESA)
        {
            return hlpFuncoes.qrySeekValue("FASEPOS", "cd_maquina", string.Format("cd_os = '{0}' and cd_fase = '{1}' and cd_empresa = '{2}'", sCD_OS, sCD_FASE, sCD_EMPRESA));
        }

        public string GetDT_FINALPREVFase(string sCD_FASE, string sCD_PEDIDO, string sCD_EMPRESA)
        {
            StringBuilder sQuery = new StringBuilder();
            sQuery.Append("select coalesce(max(fasepos.dt_finalprev),'')  dt_finalprev ");
            sQuery.Append("from ORDEMP O inner join fasepos  on ordemp.cd_os = fasepos.cd_os ");
            sQuery.Append("inner join FASES on fasepos.cd_fase = fases.cd_fase ");
            sQuery.Append("where   fases.cd_fase = '{0}' and O.cd_pedido = '{1}' and fases.cd_empresa = '{2}' group by fases.cd_fase, fases.ds_fase ");
            string sQueryFim = string.Format(sQuery.ToString(), sCD_FASE, sCD_PEDIDO, sCD_EMPRESA);
            DataTable dt = this.hlpFuncoes.qrySeekRet(sQueryFim);
            string sRet = "";
            if (dt.Rows.Count > 0)
            {
                sRet = dt.Rows[0][0].ToString();
            }
            if (sRet != "")
            {
                sRet = Convert.ToDateTime(sRet).ToString("dd/MM/yyyy HH:mm");
            }

            return sRet;
        }

        public string GetDT_FINALFase(string sCD_FASE, string sCD_PEDIDO, string sCD_EMPRESA)
        {
            StringBuilder sQuery = new StringBuilder();
            sQuery.Append("select coalesce(max(fasepos.dt_final),'')  dt_finalprev ");
            sQuery.Append("from ORDEMP O inner join fasepos  on ordemp.cd_os = fasepos.cd_os ");
            sQuery.Append("inner join FASES on fasepos.cd_fase = fases.cd_fase ");
            sQuery.Append("where   fases.cd_fase = '{0}' and O.cd_pedido = '{1}' and fasepos.cd_empresa = '{2}' group by fases.cd_fase, fases.ds_fase ");
            string sQueryFim = string.Format(sQuery.ToString(), sCD_FASE, sCD_PEDIDO, sCD_EMPRESA);
            DataTable dt = this.hlpFuncoes.qrySeekRet(sQueryFim);

            string sRet = "";
            if (dt.Rows.Count > 0)
            {
                sRet = dt.Rows[0][0].ToString();
            }
            if (sRet != "")
            {
                sRet = Convert.ToDateTime(sRet).ToString("dd/MM/yyyy HH:mm");
            }

            if (sRet.Equals("01/01/1900 00:00"))
            {
                sRet = "00/00/0000 00:00";
            }

            return sRet;
        }

        public string GetStatusFase(string sCD_FASE, string sCD_PEDIDO, string sCD_EMPRESA)
        {
            StringBuilder sQuery = new StringBuilder();
            sQuery.Append("select coalesce(max(fasepos.st_fase),'')st_fase ");
            sQuery.Append("from ORDEMP O inner join fasepos  on ordemp.cd_os = fasepos.cd_os ");
            sQuery.Append("inner join FASES on fasepos.cd_fase = fases.cd_fase ");
            sQuery.Append("where   fases.cd_fase = '{0}' and O.cd_pedido = '{1}' and fasepos.cd_empresa = '{2}' group by fases.cd_fase, fases.ds_fase ");
            string sQueryFim = string.Format(sQuery.ToString(), sCD_FASE, sCD_PEDIDO, sCD_EMPRESA);
            DataTable dt = this.hlpFuncoes.qrySeekRet(sQueryFim);

            string sRet = "";
            if (dt.Rows.Count > 0)
            {
                sRet = dt.Rows[0][0].ToString();
            }
            return sRet;
        }

        public string GetPathFileByItem(string sCD_PROD, string sCD_EMPRESA)
        {
            int iCount = Convert.ToInt32(this.hlpFuncoes.qrySeekValue("arqprod", "COUNT(*)", string.Format("cd_prod = '{0}' and cd_empresa = '{1}'", sCD_PROD, sCD_EMPRESA)));
            string sPath = "";

            if (iCount > 0)
            {
                sPath = this.hlpFuncoes.qrySeekValue("arqprod", "first 1 ds_link_arquivo", string.Format("cd_prod = '{0}' and cd_empresa = '{1}'", sCD_PROD, sCD_EMPRESA));

                if (!File.Exists(sPath))
                {
                    sPath = "";
                }
            }
            return sPath;
        }

        public string GetPathFileByFase(string sCD_PROD, string sCD_FASE, string sCD_EMPRESA)
        {
            int iCount = Convert.ToInt32(this.hlpFuncoes.qrySeekValue("faseprod", "COUNT(*)", string.Format("cd_prod = '{0}' and cd_fase = '{1}' and cd_empresa = '{2}'", sCD_PROD, sCD_FASE, sCD_EMPRESA)));
            string sPath = "";

            if (iCount > 0)
            {
                sPath = this.hlpFuncoes.qrySeekValue("faseprod", "first 1 ds_link_arquivo_it", string.Format("cd_prod = '{0}' and cd_fase = '{1}' and cd_empresa = '{2}'", sCD_PROD, sCD_FASE, sCD_EMPRESA));

                if (!File.Exists(sPath))
                {
                    sPath = "";
                }
            }
            return sPath;
        }


        public string GetObsFase(string sCD_PEDIDO, string sCD_FASE, string sCD_EMPRESA)
        {
            string sQyery = string.Format("select DS_OBS FROM FASESPED WHERE (CD_FASE = '{0}') AND (CD_PEDIDO = '{1}') and cd_empresa = '{2}'", sCD_FASE, sCD_PEDIDO, sCD_EMPRESA);

            string sValor = hlpFuncoes.RetornaBlob(sQyery);

            if (sValor == "")
            {
                sValor = hlpFuncoes.qrySeekValue("FASESPED", "DS_OBS", string.Format("(CD_FASE = '{0}') AND (CD_PEDIDO = '{1}' and cd_empresa = '{2}')", sCD_FASE, sCD_PEDIDO, sCD_EMPRESA));
            }

            return sValor;
        }


        #region INSERT

        public void FinalisaFase(string nm_operador,
                              string cd_pedido,
                              string cd_fase,
                              string qt_produzida,
                              string st_fimfase,
                              string cd_os,
                              string dt_lanc,
                              string dt_registro,
                              string st_fase,
                              string nr_fasepos,
                              string cd_maquina, string dt_final, string sCD_EMPRESA)
        {
            string sNr_lanc = hlpFuncoes.RetornaProximoValorGenerator("horareal" + sCD_EMPRESA, 7);

            string sQuery = "INSERT INTO horareal" +
                            "(nr_lanc,cd_empresa,nm_operador,cd_pedido,cd_fase,qt_produzida,st_fimfase,cd_os,dt_lanc,dt_registro ,st_fase,nr_fasepos ,cd_maquina,dt_final) values" +
                            "('{0}',     '{1}',      '{2}',    '{3}',   '{4}',    {5},        {6},   '{7}', '{8}',     '{9}',    '{10}',   '{11}',        {12} , {13})";
            sQuery = string.Format(sQuery, sNr_lanc, sCD_EMPRESA, nm_operador, cd_pedido, cd_fase, qt_produzida, st_fimfase, cd_os, dt_lanc, dt_registro, st_fase, nr_fasepos, cd_maquina, dt_final);//
            this.hlpFuncoes.qrySeekInsert(sQuery);
        }


        public void IniciaFase(string nm_operador,
                                string cd_pedido,
                                string cd_fase,
                                string qt_produzida,
                                string st_fimfase,
                                string cd_os,
                                string dt_lanc,
                                string dt_registro,
                                string st_fase,
                                string nr_fasepos,
                                string cd_maquina, string dt_final, string sCD_EMPRESA)
        {
            string sNr_lanc = hlpFuncoes.RetornaProximoValorGenerator("horareal" + sCD_EMPRESA, 7);

            string sQuery = "INSERT INTO horareal" +
                            "(nr_lanc,cd_empresa,nm_operador,cd_pedido,cd_fase,qt_produzida,st_fimfase,cd_os,dt_lanc,dt_registro ,st_fase,nr_fasepos ,cd_maquina,dt_final) values" +
                            "('{0}',     '{1}',      '{2}',    '{3}',   '{4}',    {5},        {6},   '{7}', '{8}',     '{9}',    '{10}',   '{11}',        {12} , {13})";
            sQuery = string.Format(sQuery, sNr_lanc, sCD_EMPRESA, nm_operador, cd_pedido, cd_fase, qt_produzida, st_fimfase, cd_os, dt_lanc, dt_registro, st_fase, nr_fasepos, cd_maquina, dt_final);//

            this.hlpFuncoes.qrySeekInsert(sQuery);


        }
        #endregion

        #region UPDATE


        public void AlterObsFase(string sValor, string sCD_PEDIDO, string sCD_FASE, string sCD_EMPRESA)
        {
            string sQuery = string.Format("UPDATE FASESPED SET DS_OBS = '{0}' WHERE (CD_FASE = '{1}') AND (CD_PEDIDO = '{2}') and cd_pedido = '{3}'", sValor, sCD_FASE, sCD_PEDIDO, sCD_EMPRESA);

            this.hlpFuncoes.qrySeekUpdate(sQuery);
        }

        #endregion


    }
}
