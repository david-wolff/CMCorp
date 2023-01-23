using CM.CMFlex.Core.Contracts;
using CM.CMFoundation.Architecture.Domain;
using CM.CMFoundation.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regras.TestProject.Attributes;
using Regras.TestProject.Enums;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System;
using CM.CMPrevWeb.Emprestimo.Contracts;

namespace Regras.TestProject.Regras.Previ.Emprestimo
{
    [TestClass]
    [Regra("EMPJurCartNovas", "EMP Concessão - Valor da Taxa de Juros Carteiras NCP1 NCP2")]
    public class ConcessaoValorTaxaJurosCarteiras_NCP1_NCP2 : RegrasBase<RegraValorDaTaxaDeJuros>
    {
        [TestMethod]
        [TestProperty("EMPJurCartNovas", "EMP Concessão - Valor da Taxa de Juros Carteiras NCP1 NCP2")]
        public void MetodoDeTeste()
        {
            var contratoTeste = new CM.CMPrevWeb.Emprestimo.Domain.Contrato()
            {
                PlanoNaConcessao = new CM.CMPrevWeb.ParamPrev.Domain.PlanoPrevidenciario()
                {
                    Id = 2,
                    Descricao = "Plano 1"
                }
            };

            var partipantePlano = new CM.CMPrevWeb.CadastroPrev.Domain.ParticipantePorPlano()
            {
                SituacaoNaFundacao = new CM.CMPrevWeb.ParamPrev.Domain.SituacaoDoParticipanteNaFundacao() { Codigo = "EXO" },
                SituacaoNoPlano = new CM.CMPrevWeb.ParamPrev.Domain.SituacaoDoParticipanteNoPlano() { Codigo = "SOP" }
            };

            var data1 = new DateTime(2022, 01, 01);

            Contrato = new CM.CMPrevWeb.Emprestimo.Contracts.RegraValorDaTaxaDeJuros()
            {
                InModalidadeDeEmprestimo = Consultar<CM.CMPrevWeb.Emprestimo.Domain.Modalidade>().Where(x => x.Codigo == "FI_CRE").FirstOrDefault(),
                InContrato = contratoTeste,
                InDataDeCredito = data1,
                InParticipantePorPlano = partipantePlano

            };

            ExecutarTeste();
        }

        public override void Regra(CM.CMPrevWeb.Emprestimo.Contracts.RegraValorDaTaxaDeJuros ctr)
        {
            /*=====================================================================
             NOME: EMP Concessão - Valor da Taxa de Juros Carteiras NCP1 NCP2
             FUNÇÕES: ----------
             TABELAS GENÉRICAS: EMPSituacao/EMPConcTxJuros
             OBJETIVO: Definir o valor de taxa de juros a ser aplicada ao contrato das carteiras NCP1 e NCP2
             =====================================================================*/
            /*=====================================================================
             ALTERAÇÃO: 23/02/2022
             DEMANDA: 208341
             RESPONSÁVEL: David Wolff 
             DESCRIÇÃO: Alteracao no metodo de consulta da cotacao de moeda e na situacao do plano para consulta em tabela generica.
             =====================================================================*/

            //Desenvolvimento: 
            var query = Consultar<CM.CMPrevWeb.Emprestimo.Domain.CondicoesDoContrato>().AsQueryable();

            //Parâmetros de entrada
            var dataEvento = ctr.InDataDeCredito;
            var plano = ctr.InContrato.PlanoNaConcessao.Id;

            //Parâmetro de saída
            var valorTaxaJuros = 0m;

            //CONFORME RESPOSTA DO IMPEDIMENTO: 115530, FOI DECLARADO QUE O PLANO1 SE REFERE AO ID 2 E O PLANO PREVI FUTURO SE REFERE AO ID 3
            if (plano == 2 || plano == 3) {
                //Verificar se existe condição de contrato com valor para TaxaJuros para o contrato para vigenciainicio e fim na data do evento
                var taxaJuros = query
                    .Where(x =>
                    x.DataDeInicio <= dataEvento &&
                    x.DataDeTermino >= dataEvento
                    ).Select(x => x.TaxaJuros).FirstOrDefault().ToString();

                //Se houver, recuperar o valor da moeda da condição para a data informada do evento para o evento
                if (!String.IsNullOrEmpty(taxaJuros))
                {
                    var moeda = query
                       .Where(x =>
                       x.DataDeInicio <= dataEvento &&
                       x.DataDeTermino >= dataEvento
                       ).Select(x => x.IndiceCorrecao).FirstOrDefault().ToString();
                    if (String.IsNullOrEmpty(moeda))
                    {
                        ctr.OutErro = true;
                        ctr.OutMensagem = "EMPJurCartNovas - 88 - Valor da moeda nulo";
                        return;
                    }
                    valorTaxaJuros = Convert.ToDecimal(Convert.ToDecimal(taxaJuros) * Convert.ToDecimal(moeda));
                }
                else
                {
                    var par = new Dictionary<string, object>()
                            {
                                { "Tabela", "EMPConcTxJuros" },
                                { "Codigo", "moeTaxJur" },
                                { "Vigencia", DateTime.Now.ToShortDateString() },
                                { "TipoEmp", ctr.InModalidadeDeEmprestimo.TipoDeEmprestimo != null ? ctr.InModalidadeDeEmprestimo.TipoDeEmprestimo.Codigo.ToString() : string.Empty },
                                { "Modalidade", ctr.InModalidadeDeEmprestimo != null ? ctr.InModalidadeDeEmprestimo.Codigo.ToString() : string.Empty },
                                { "Plano", ctr.InContrato.PlanoNaConcessao != null ? ctr.InContrato.PlanoNaConcessao.Descricao.ToString() : string.Empty },
                                //{ "Patrocinadora", ctr.InParticipantePorPlano.Patrocinadora != null ? ctr.InParticipantePorPlano.Patrocinadora.Codigo.ToString() : string.Empty },
                                { "Retorno", null }
                            };
                    ExecutarFuncao("BuscaParametros", par);
                    string nomeMoeda = par["Retorno"].ToString();
                    if (string.IsNullOrEmpty(nomeMoeda))
                        throw new Exception("Parametrização não encontrada não tabela genérica EMPConcTxJuros - código de busca moeTaxJuros.");

                    var moedaValor = Consultar<CM.CMFlex.Core.Domain.Moeda.Moeda>().Where(x => x.Simbolo == nomeMoeda).FirstOrDefault();
                    var moedaValorCotacao = moedaValor.GetUltimaCotacaoAteAData(dataEvento);
                    if (moedaValorCotacao.IsNull())
                    {
                        ctr.OutErro = true;
                        ctr.OutMensagem = "Valor nulo da cotação para a moeda " + nomeMoeda + " no mês " + dataEvento.Month;
                        return;
                    }
                    //Deve ser buscada a situação do participante na Patrocinadora/Fundação na data do evento, 
                    //pois a regra pode estar sendo utilizada num processo de Reprocessamento do contrato.
                    var situacaoNaFundacao = ctr.InParticipantePorPlano.SituacaoNaFundacao.Codigo;
                    var situacaoPlano = ctr.InParticipantePorPlano.SituacaoNoPlano.Codigo;
                    if (situacaoNaFundacao == "EXO" && situacaoPlano == "SOP")
                    {
                        var paramet = new Dictionary<string, object>()
                            {
                                {"Tabela", "EMPSituacao" },
                                {"Codigo", "AcrescJurosNCP" },
                                {"Vigencia", DateTime.Now.ToShortDateString() },
                                {"SituacaoFundacao", ctr.InParticipantePorPlano.SituacaoNaFundacao != null ? ctr.InParticipantePorPlano.SituacaoNaFundacao.Codigo.ToString() : string.Empty },
                                {"SituacaoPlano", ctr.InParticipantePorPlano.SituacaoNoPlano != null ? ctr.InParticipantePorPlano.SituacaoNoPlano.Codigo.ToString() : string.Empty },
                                //{ "Plano", ctr.InParticipantePorPlano.PlanoPrevidenciario != null ? ctr.InParticipantePorPlano.PlanoPrevidenciario.Descricao.ToString() : string.Empty },
                                //{ "Patrocinadora", ctr.InParticipantePorPlano.Patrocinadora != null ? ctr.InParticipantePorPlano.Patrocinadora.Codigo.ToString() : string.Empty },
                                { "Retorno", null }
                            };
                        ExecutarFuncao("BuscaParametrosPorSituacao", paramet);
                        string valorRecuperado = paramet["Retorno"].ToString();
                        if (string.IsNullOrEmpty(valorRecuperado))
                            throw new Exception("Parametrização não encontrada não tabela genérica EMPSituacao - código de busca AcrescJurosCRECPFCP.");
                        ctr.OutValorDaTaxaDeJuros = Convert.ToDecimal(valorRecuperado);
                        return;

                    }
                    else if ((situacaoNaFundacao == "EXO" && situacaoPlano == "SOP") || (situacaoNaFundacao == "ENC" && situacaoPlano == "ENC"))
                        {
                            valorTaxaJuros += 2m / 100m;
                        }
                    }
                ctr.OutValorDaTaxaDeJuros = valorTaxaJuros;
            }
            else
            {
                ctr.OutErro = true;
                ctr.OutMensagem = "EMPJurCartNovas - 151 - Plano diferente de Plano 1 e Plano Previ Futuro";
                return;
            }
        }
    }
}