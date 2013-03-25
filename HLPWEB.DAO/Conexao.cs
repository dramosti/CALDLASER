using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using FirebirdSql.Data.FirebirdClient;

namespace HLPWEB.DAO
{
    public class Conexao
    {
        private FbConnection _conexao;
        public FbConnection conexao
        {
            get
            {
                if (_conexao == null)
                {
                    _conexao = new FbConnection(MontaStringConexao());
                }
                return _conexao;
            }
            set
            {
                _conexao = value;
            }
        }

        public string CD_USUARIO { get; set; }
        public string NM_USUARIO { get; set; }
        public string CD_EMPRESA { get; set; }

        private string MS_DATABASENAME { get; set; }
        private string MS_SERVERNAME { get; set; }
        private string MS_PORTA { get; set; }

        public void Inicializacao()
        {
            this.MS_DATABASENAME = WebConfigurationManager.AppSettings["MS_DATABASENAME"];
            this.MS_SERVERNAME = WebConfigurationManager.AppSettings["MS_SERVERNAME"];
            this.MS_PORTA = WebConfigurationManager.AppSettings["MS_PORTA"];
            this.CD_EMPRESA = WebConfigurationManager.AppSettings["EmpresaDefault"];
        }

        public string MontaStringConexao()
        {
            try
            {
                StringBuilder sbConexao = new StringBuilder();
                sbConexao.Append("User =");
                sbConexao.Append("SYSDBA");
                sbConexao.Append(";");
                sbConexao.Append("Password=");
                sbConexao.Append("masterkey");
                sbConexao.Append(";");
                string sPorta = this.MS_PORTA;
                if (sPorta.Trim() != "")
                {
                    sbConexao.Append("Port=" + sPorta + ";");
                }
                sbConexao.Append("Database=");
                string sdatabase = this.MS_DATABASENAME;
                sbConexao.Append(sdatabase);
                sbConexao.Append(";");
                sbConexao.Append("DataSource=");
                sbConexao.Append(this.MS_SERVERNAME);
                sbConexao.Append(";");
                sbConexao.Append("Dialect=3; Charset=NONE;Role=;Connection lifetime=15;Pooling=true; MinPoolSize=0;MaxPoolSize=2000;Packet Size=8192;ServerType=0;");
                return (string)sbConexao.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
