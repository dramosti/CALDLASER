using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace HLPWEB.DAO
{
    public class FuncoesGerais : Conexao
    {

        public FuncoesGerais() 
        {
            Inicializacao();
        }
        public DataTable QrySeekRet(string sExpressaoSql)
        {
            try
            {
                FbDataAdapter da = new FbDataAdapter(sExpressaoSql, conexao);
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();
                DataSet ds = new DataSet("dadoshlp");
                da.Fill(ds, "registro");
                DataTable dt = ds.Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexao.Close();
            }
        }

        public void QrySeekUpdate(
        string sExpressaoSql)
        {
            try
            {
                FbCommand cmdUpDateMoviPend = new FbCommand();
                cmdUpDateMoviPend.CommandText = sExpressaoSql;
                cmdUpDateMoviPend.Connection = conexao;
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();
                cmdUpDateMoviPend.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexao.Close();
            }

        }

        public void QrySeekInsert(
       string sExpressaoSql)
        {
            try
            {
                FbCommand cmdUpDateMoviPend = new FbCommand();
                cmdUpDateMoviPend.CommandText = sExpressaoSql;
                cmdUpDateMoviPend.Connection = conexao;
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();
                cmdUpDateMoviPend.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexao.Close();
            }

        }

        public FbDataReader QrySeekReader(
    string sExpressaoSql)
        {
            try
            {
                FbCommand cmd = new FbCommand();
                cmd.CommandText = sExpressaoSql;
                cmd.Connection = conexao;
                if (conexao.State != ConnectionState.Open)
                    conexao.Open();

                FbDataReader Reader = cmd.ExecuteReader();

                return Reader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conexao.Close();
            }
        }
        
    }
}
