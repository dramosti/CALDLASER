using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for boFasesPedido
/// </summary>
public class boFasesPedido
{
    public string Descricao { get; set; }
    public string Fase { get; set; }
    public string Previsao { get; set; }
    public string Realizado { get; set; }
   private string _Situacao;

    public string Situacao
    {
        get { return _Situacao; }
        set
        {
            switch (value)
            {
                case "01": { _Situacao = "INICIADO"; }
                    break;
                case "03": { _Situacao = "FINALIZADO"; }
                    break;
                case "06": { _Situacao = "N_INICIADO"; }
                    break;
                default:
                    {
                        _Situacao = "N_INICIADO";
                    }
                    break;
            }
        }
    }

    public List<boFasesPedido> getall() { return new List<boFasesPedido>(); }

}