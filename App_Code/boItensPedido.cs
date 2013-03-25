using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for boItensPedido
/// </summary>
public class boItensPedido
{
    public string cd_prod { get; set; }
    public string Codigo { get; set; }
    public string Descricao { get; set; }
    public string Un { get; set; }
    public int Qtd { get; set; }
    public string OP { get; set; }
    private string _Situacao;

    public string Situacao
    {
        get { return _Situacao; }
        set
        {
            switch (value)
            {
                case "1": { _Situacao = "INICIADO"; }
                    break;
                case "2": { _Situacao = "FINALIZADO"; }
                    break;
                case "3": { _Situacao = "N_INICIADO"; }
                    break;
                default:
                    {
                        _Situacao = "N_INICIADO";
                    }
                    break;
            }
        }
    }

    //public enum sit { INICIADO, N_INICIADO, FINALIZADO }

}