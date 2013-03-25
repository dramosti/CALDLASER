using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace HLPWEB.DAO
{
    public class Funcoes : Conexao
    {
        private FuncoesGerais funcGerais { get; set; }
        public Funcoes()
        {
            funcGerais = new FuncoesGerais();
        }

        public DataTable qrySeekRet(string sExpressao)
        {
            DataTable dt = funcGerais.QrySeekRet(sExpressao);
            return dt;
        }

        public FbDataReader qrySeekReader(string sExpressao)
        {
            FbDataReader dr = funcGerais.QrySeekReader(sExpressao);
            return dr;
        }

        public DataTable qrySeekRet(string sTabela, string sCampos = "", List<ListaCampos> lCampos = null)
        {
            if (lCampos != null)
            {
                sCampos = MontaStringCampos(lCampos);
            }
            return (qrySeekRet(sTabela, sCampos, String.Empty, String.Empty));
        }

        public DataTable qrySeekRet(string sTabela, string sCampos, string sWhere, List<ListaCampos> lCampos = null)
        {
            if (lCampos != null)
            {
                sCampos = MontaStringCampos(lCampos);
            }
            return (qrySeekRet(sTabela, sCampos, sWhere, String.Empty));
        }

        public DataTable qrySeekRet(string sTabela, string sCampos, string sWhere, string sOrdem, List<ListaCampos> lCampos = null)
        {
            if (lCampos != null)
            {
                sCampos = MontaStringCampos(lCampos);
            }
            StringBuilder strExpressao = new StringBuilder();
            strExpressao.Append("SELECT ");
            if (sCampos.Trim() == String.Empty)
                strExpressao.Append("*");
            else
                strExpressao.Append(sCampos);
            strExpressao.Append(" FROM " + sTabela);

            StringBuilder strWhere = new StringBuilder();
            if ((sWhere != null) && (sWhere.Trim() != String.Empty))
                strWhere.Append(sWhere);
            if ((fExisteCampo(sTabela, "CD_EMPRESA")) &&
                (sWhere.ToUpper().IndexOf("CD_EMPRESA") < 0))
            {
                if (strWhere.Length > 0)
                    strWhere.Append(" AND ");
                strWhere.Append("(CD_EMPRESA = '" + CD_EMPRESA + "')");
            }
            if (strWhere.Length > 0)
            {
                strWhere.Insert(0, " WHERE ");
                strExpressao.Append(strWhere.ToString());
            }

            if ((sOrdem != null) && (sOrdem.Trim() != String.Empty))
                strExpressao.Append(" ORDER BY " + sOrdem);

            DataTable dt = funcGerais.QrySeekRet(strExpressao.ToString());
            return dt;
        }

        public string qrySeekValue(string sTabela, string sCampo,
            string sWhere)
        {
            return qrySeekValue(sTabela, sCampo, sWhere, false);
        }

        private string qrySeekValue(string sTabela, string sCampo,
            string sWhere, bool bGeraErroSemRegistros)
        {

            DataTable dt = qrySeekRet(sTabela, sCampo, sWhere, null);
            String svalor = null;
            if (dt.Rows.Count > 0)
            {
                DataRow registro = dt.Rows[0];
                //svalor = registro[sCampo].ToString();
                svalor = registro[0].ToString();
            }
            else
            {
                if (bGeraErroSemRegistros)
                    throw new Exception("Não foram encontrados registros válidos!");
            }
            if (svalor == null)
                svalor = String.Empty;
            return svalor;
        }

        public bool qrySeek(string sTabela, string[] sCampos, string[] sValores)
        {
            StringBuilder strCampos = new StringBuilder();
            StringBuilder strWhere = new StringBuilder();
            string sCampo;

            for (int i = 0; i < sCampos.Length; i++)
            {
                if (strCampos.Length > 0)
                {
                    strCampos.Append(", ");
                    strWhere.Append(" AND ");
                }
                sCampo = sCampos[i];
                strCampos.Append(sCampo);
                strWhere.Append("(" + sCampo + " = '" + sValores[i].Trim() + "')");
            }

            DataTable dt = qrySeekRet(sTabela, strWhere.ToString(), strCampos.ToString(), null);

            return (dt.Rows.Count > 0);
        }

        public bool qrySeek(string sExpressao)
        {
            DataTable dt = qrySeekRet(sExpressao);
            return (dt.Rows.Count > 0);
        }

        public void qrySeekUpdate(string sExpressao)
        {
            funcGerais.QrySeekUpdate(sExpressao);
        }

        public void qrySeekInsert(string sExpressao)
        {
            funcGerais.QrySeekInsert(sExpressao);
        }

        public bool fExisteCampo(string sNomeCampo, string sTabela)
        {
            try
            {
                string sQuery = "SELECT " +
                "RDB$FIELD_NAME CAMPO " +
                "FROM " +
                "RDB$RELATION_FIELDS " +
                "WHERE " +
                " RDB$FIELD_NAME = '{0}' AND" +
                " RDB$RELATION_NAME = '{1}'";

                FbConnection cx = conexao;
                if (cx.State != ConnectionState.Open)
                {
                    cx.Open();
                }
                FbCommand cmd = new FbCommand(string.Format(sQuery, sNomeCampo.ToUpper(), sTabela.ToUpper()), cx);
                object iResult = cmd.ExecuteScalar();
                cx.Close();
                if (iResult == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RetornaProximoValorGenerator(string sNomeGernerator, int sTamanho)
        {
            try
            {
                string sQuery = string.Format("select gen_id({0},1) from empresa", sNomeGernerator);
                string sRetorno = this.qrySeekValue("empresa", string.Format("gen_id({0},1)", sNomeGernerator), "");
                return sRetorno.PadLeft(sTamanho, '0');
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public struct ListaCampos
        {
            public string sCampo { get; set; }
            public string sCoalesce { get; set; }
            public string sAlias { get; set; }
        }

        private string MontaStringCampos(List<ListaCampos> objlCampos)
        {
            string sCampos = string.Empty;

            foreach (ListaCampos item in objlCampos)
            {
                string Campo = item.sCampo == "" ? "''" : item.sCampo;

                sCampos += (item.sCoalesce == null ? Campo :
                    string.Format("coalesce({0},'{1}')",
                    Campo, item.sCoalesce)) + " " + (item.sAlias == null ? Campo : item.sAlias) + ",";
            }
            sCampos = sCampos.Remove(sCampos.Length - 1, 1);
            return sCampos;
        }

        public string RetornaBlob(string sQuery)
        {
            string texto = "";
            FbConnection Conn = this.conexao;
            FbCommand comando = new FbCommand(sQuery.ToString(), Conn);
            if (Conn.State == ConnectionState.Closed)
            {
                Conn.Open();
            }
            FbDataReader Reader = comando.ExecuteReader();
            Byte[] blob = null;
            MemoryStream ms = new MemoryStream();
            while (Reader.Read())
            {
                blob = new Byte[(Reader.GetBytes(0, 0, null, 0, int.MaxValue))];
                try
                {
                    Reader.GetBytes(0, 0, blob, 0, blob.Length);
                }
                catch
                {
                    texto = "";
                }
                ms = new MemoryStream(blob);
            }
            Conn.Close();

            StreamReader Ler = new StreamReader(ms);
            texto += Ler.ReadLine();
            while (Ler.Peek() != -1)
            {
                texto += Ler.ReadLine();
            }
            return TiraCaracterEstranho(texto).Trim();
        }

        public static string TiraCaracterEstranho(string sString)
        {
            sString = sString.Replace("{\\colortbl ;\\red0\\green0\\blue0;}\\viewkind4\\uc1\\pard\\cf1\\lang1046\\f0\\fs16 ", "");
            sString = sString.Replace(@"{\colortbl ;\red0\green0\blue255;}\viewkind4\uc1\pard\cf1\lang1046\f0\fs16 ", "");
            sString = sString.Replace("\\viewkind4\\uc1\\pard\\f0\\fs16 ", "");
            sString = sString.Replace("\\viewkind4\\uc1\\pard\\lang1046\\f0\\fs16 ", "");
            sString = sString.Replace("\\viewkind4\\uc1 d\\lang1046 ", "");
            sString = sString.Replace("\\f1\\'c7", "C");
            sString = sString.Replace("\\'c3", "A");
            sString = sString.Replace("\\f0 ", "");
            sString = sString.Replace("\\par", " ");
            sString = sString.Replace("}\0", "");
            sString = sString.Replace("\\f0", "");
            sString = sString.Replace("{\\colortbl ;\\red0\\green0\\blue255;}\\viewkind4\\uc1 d\\cf1\\lang1046\\fs16   ", "");
            sString = sString.Replace("\\'ba", "o");
            sString = sString.Replace("\\f1", "");
            sString = sString.Replace("\\'cd", "I");
            sString = sString.Replace("\\viewkind4\\uc1 d\\b\\fs16 ", "");
            sString = sString.Replace("\\'aa", "a");
            sString = sString.Replace("\\'e1", "a");
            sString = sString.Replace("\\'e7\\'e3", "ca");
            sString = sString.Replace("\\b0", ".");
            sString = sString.Replace("\\'e3", "a");
            sString = sString.Replace("\\'ea", "e");
            sString = sString.Replace("\\c9", "E");
            sString = sString.Replace("\\c9", "E");
            sString = sString.Replace("\\fs52", "");
            sString = sString.Replace("\\fs16", "");
            sString = sString.Replace("  }", "");

            while (sString.Contains("  "))
            {
                sString = sString.Replace("  ", " ");

            }

            return sString.Trim();
        }

        public List<string> GetListEmpresas()
        {
            DataTable dt = this.qrySeekRet("select e.cd_empresa,(e.cd_empresa || ' - ' || e.nm_empresa) as Nome from empresa e");
            List<string> lEmpresas = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                lEmpresas.Add(row["Nome"].ToString());
            }
            return lEmpresas;
        }
    }
}
