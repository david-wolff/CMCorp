public static int IdContrato;
public static string Nome;
public static string CPF;
public static string Empresa;
public static string Matricula;
public static int NumeroContrato;
public static decimal ValorContrato;
public static decimal ValorMaximo;
public static int NumeroParcelas;
public static decimal ValorPrevistoParcela;
public static DateTime DataDeConcessao;
public static string Renegociacao;
public static string QuaisContratos;
public static decimal ValorLiquidacaoContratoAnterior;
public static decimal ValorLiquidacaoContratoAnteriorSantander;
public static decimal ValorParcelaAnterior;

#region Construct Query
//Queries envolvidas:
//-Recupera as informações dos itens 1 a 9, 12 e 13:
var queryContrato = String.Format(@"
    select case when pfm.ANNOME is null then pfi.ANNOME else pfm.ANNOME end Nome,
           case when dom.ANNUMERO is null then doc.ANNUMERO else dom.ANNUMERO end CPF,
           pju.ANRAZAOSOCIAL Empresa,
           CASE when dpp.ANMATRICULA is null then hfu.ANMATRICULA else dpp.ANMATRICULA end Matricula,
           con.NMNUMEROCONTRATO Contrato,
           con.VLVALORSOLICITADO ValorContrato,
           con.VLVALORMAXIMO ValorMaximo,
      con.NMNUMEROPARCELAS NumeroParcela,
      con.VLVALORPARCELA ValorParcela,
      con.DTDATADECREDITO DataConcessao,
      dhc.VLVALORPREVISTO ValorQuitacaoAnterior,
      
    from EMPCONTRATO con
    join EMPHISTORICODOCONTRATO hcp on hcp.FKCONTRATO = con.IDCONTRATO and hcp.NMTIPODEMOVIMENTACAO = 1 and hcp.NMSTATUSDOITEM <> 4
    join EMPITEMDECALCULO ite on ite.ANCODIGO = 'IteConRef'
    join EMPITEMPORMODALIDADE imp on imp.FKMODALIDADE = con.FKMODALIDADE and imp.NMTIPODEMOVIMENTACAO = hcp.NMTIPODEMOVIMENTACAO and imp.FKITEMDECALCULO = ite.IDITEMDECALCULO
    join EMPITEMPORMODALIDADE ims on ims.FKMODALIDADE = con.FKMODALIDADE and ims.NMTIPODEMOVIMENTACAO = hcp.NMTIPODEMOVIMENTACAO and ims.FKITEMDECALCULO = its.IDITEMDECALCULO
    left outer join EMPDETALHEDOHISTORICODOCON dhc on dhc.FKHISTORICODOCONTRATO = hcp.IDHISTORICODOCONTRATO and dhc.FKITEMPORMODALIDADE = imp.IDITEMPORMODALIDADE
    left outer join EMPDETALHEDOHISTORICODOCON dhs on dhs.FKHISTORICODOCONTRATO = hcp.IDHISTORICODOCONTRATO and dhs.FKITEMPORMODALIDADE = ims.IDITEMPORMODALIDADE
    join CPVPARTICIPANTEPORPLANO ppp on ppp.IDPARTICIPANTEPORPLANO = con.FKPARTICIPANTEPORPLANO
    join CPVHISTORICOFUNCIONAL hfu on hfu.IDHISTORICOFUNCIONAL = ppp.FKHISTORICOFUNCIONALATUAL
    join PPRPATROCINADORA pat ON pat.IDPATROCINADORA = hfu.FKPATROCINADORA
    join GLBPESSOAJURIDICA pju on pju.FKPESSOA = pat.FKPESSOAJURIDICA
    join CPVASSOCIADO ass on ass.IDASSOCIADO = ppp.FKASSOCIADO
    join GLBPESSOAFISICA pfi on pfi.FKPESSOA = ass.FKPESSOAFISICA
    join GLBPESSOA pes on pes.IDPESSOA = pfi.FKPESSOA
    join GLBDOCUMENTO doc on doc.IDDOCUMENTO = pes.FKDOCUMENTOPRINCIPAL
    left outer join CPVDEPENDENTEDOPARTICIPANT dpp on dpp.IDDEPENDENTEDOPARTICIPANTEPORP = con.FKDEPENDENTEDOPARTICIPANTEPORP
    left outer join GLBDEPENDENTEDAPESSOAFISIC dpf on dpf.IDDEPENDENTEDAPESSOAFISICA = dpp.FKDEPENDENTEDAPESSOAFISICA
    left outer join GLBDEPENDENTE dep on dep.IDDEPENDENTE = dpf.FKDEPENDENTE
    left outer join GLBPESSOAFISICA pfm on pfm.FKPESSOA = dep.FKPESSOAFISICA
    left outer join GLBPESSOA pem on pem.IDPESSOA = pfm.FKPESSOA
    left outer join GLBDOCUMENTO dom on dom.IDDOCUMENTO = pem.FKDOCUMENTOPRINCIPAL
    where con.IDCONTRATO = {0}", IdContrato);
DataTable retornoDoOpenSQL = OpenSQL("Query", queryContrato);
//-Recupera as informações dos itens 10, 11 e 14:
//a) Se a query abaixo retornar dados, a resposta do item 10 é 'Sim'.Caso contrário é 'Não'.
//b) Para o item 11, concatenar separando por vírgulas, os resultados do campo ContratoAnterior.
//c) Para o item 14, somar os valores do campo ValorParcelaAnterior
var queryContratoAnterior = String.Format(@"
            select coa.NMNUMEROCONTRATO ContratoAnterior,
                    moa.ANCODIGO ModalidadeAnterior,
                    coa.VLVALORPARCELA ValorParcelaAnterior
            from EMPCONTRATO coa
            join EMPMODALIDADE moa on moa.IDMODALIDADE = coa.FKMODALIDADE
            where coa.FKCONTRATOQUITACAO = {0}", IdContrato);
DataTable retornoContratoAnterior = OpenSQL("Query", queryContratoAnterior);
#endregion
#region Out Params
if (retornoDoOpenSQL.Rows.Count > 0)
{
    Nome = (string)retornoDoOpenSQL.Rows[0]["Nome"];
    CPF = (string)retornoDoOpenSQL.Rows[0]["CPF"];
    Empresa = (string)retornoDoOpenSQL.Rows[0]["Empresa"];
    Matricula = (string)retornoDoOpenSQL.Rows[0]["Matricula"];
    NumeroContrato = Convert.ToInt32(retornoDoOpenSQL.Rows[0]["Contrato"]);
    ValorContrato = (decimal)Convert.ToDouble(retornoDoOpenSQL.Rows[0]["ValorContrato"]);
    ValorMaximo = (decimal)Convert.ToDouble(retornoDoOpenSQL.Rows[0]["ValorMaximo"]);
    NumeroParcelas = (int)retornoDoOpenSQL.Rows[0]["NumeroParcela"];
    ValorPrevistoParcela = (decimal)Convert.ToDouble(retornoDoOpenSQL.Rows[0]["ValorParcela"]);
    DataDeConcessao = (DateTime)retornoDoOpenSQL.Rows[0]["DataConcessao"];
    //Valor da Liquidação do contrato anterior(se a resposta do item 9 for "SIM" será preciso informar os valor(es) do (s)contrato(s) quitado(s) pelo contrato que está sendo aprovado) ítem de cálculo no movimento de Concessão
    ValorLiquidacaoContratoAnterior = (decimal)Convert.ToDouble(retornoDoOpenSQL.Rows[0]["ValorQuitacaoAnterior"]);
    //Valor da Liquidação do contrato anterior Santander(se a resposta do item 9 for "SIM" será preciso informar os valor(es) do (s)contrato(s) quitado(s) pelo contrato que está sendo aprovado) ítem de cálculo no movimento de Concvessão
   
}
Renegociacao = "Não";
if (retornoContratoAnterior.Rows.Count > 0)
{
    Renegociacao = "Sim";
    foreach (System.Data.DataRow item in (retornoContratoAnterior as System.Data.DataTable).Rows)
    {
        //concatenar separando por vírgulas, os resultados do campo ContratoAnterior
        QuaisContratos += "," + item["ContratoAnterior"].ToString();
        //somar os valores do campo ValorParcelaAnterior
        ValorParcelaAnterior += (decimal)Convert.ToDouble(item["ValorParcelaAnterior"]);
    }
    QuaisContratos = QuaisContratos.Substring(1);
}
#endregion