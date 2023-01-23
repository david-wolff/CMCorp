//Repact implantacao
{
    [TestClass]
    [Regra("EMPElegRepacSim", "EMP ELEGIBILIDADE - REPACTUAÇÃO EVENTO SIMULAÇÃO")]
    public class EMP_Elegibilidade_Repact_Simulacao : RegrasBase<RegraElegibilidadeDeEvento>
    public class EMP_Elegibilidade_Repact_Simulacao : RegrasBase<RegraElegibilidadeEventoSolicitacaoImobiliaria>
    {
        [TestMethod]
        [TestProperty("EMPElegRepacSim  ", "EMP ELEGIBILIDADE - REPACTUAÇÃO EVENTO SIMULAÇÃO")]
        [TestProperty("EMPElegRepacSim", "EMP ELEGIBILIDADE - REPACTUAÇÃO EVENTO SIMULAÇÃO")]
        public void MetodoDeTeste()
        {
            Contrato = new CM.CMPrevWeb.Emprestimo.Contracts.RegraElegibilidadeDeEvento()
            {
                InContrato = new CM.CMPrevWeb.Emprestimo.Domain.Contrato
                {
                    Id = 1,
                    StatusDoContrato = CM.CMPrevWeb.Emprestimo.Domain.StatusDoContratoEnum.Ativo,
                    ValorSaldoDevedor = 0m,
                    Modalidade = new CM.CMPrevWeb.Emprestimo.Domain.Modalidade
                    {
                        Codigo = "FI C"
                    }
                },
                InDataMovimentacao = new DateTime(2020,10,10)
            };
            ExecutarTeste();
        }
        public override void Regra(CM.CMPrevWeb.Emprestimo.Contracts.RegraElegibilidadeDeEvento ctr)
        public override void Regra(CM.CMPrevWeb.Emprestimo.Contracts.RegraElegibilidadeEventoSolicitacaoImobiliaria ctr)
        {
            /*===========================================================================
            NOME: EMP ELEGIBILIDADE - REPACTUAÇÃO EVENTO SIMULAÇÃO
            DEMANDA: 120262
            RESPONSÁVEL: Mariana Mendonça / Jonas Reis
            DESCRIÇÃO: Implementação parcial da regra / Finalização
            ===========================================================================*/
            ===========================================================================
            ===========================================================================
            ALTERAÇÃO: 14/03/2022
            DEMANDA: 209936
            RESPONSÁVEL: Caio Paranhos
            DESCRIÇÃO: Alteração do contract e do código da regra.
            =========================================================================== */
            //Desenvolvimento: 
            var codModalidade = ctr.InContrato.Modalidade.Codigo;
            string[] codigosTipoEmprestimo = {"FI CIM", "FI GT_3", "FI PCE_GT_1", "FI_CPF", "FI_CRE"};
            string[] modalidades = {"FI CIM", "FI GT_3", "FI PCE_GT_1", "FI_CPF", "FI_CRE"};
            var idSolicitacaoBase = ctr.InSolicitacaoImobiliariaBase.Id;
            var itensAMigrar = 0m;
            itensAMigrar = ctr.InContrato.Historico
                .Where(p => p.DataSaldoDevedor < ctr.InDataMovimentacao && p.Contrato.Id == ctr.InContrato.Id)
                .OrderByDescending(p => p.DataSaldoDevedor).Select(p => p.ValorSaldoDevedor).FirstOrDefault();
            CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum[] statusList = 
                { CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum.Cancelada,
                  CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum.AditivoEnviado,
                  CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum.RGIRecebido};
            var solRepac =
                (from sr in Consultar<CM.CMPrevWeb.Emprestimo.Domain.SolicitacaoRepactuacao>()
                 where sr.Contrato != null
                       && sr.Contrato.Id == ctr.InContrato.Id
                 orderby sr.Id descending
                 select sr).FirstOrDefault();
            if (ctr.InContrato == null)
            var statusSolicitacao = Consultar<CM.CMPrevWeb.Emprestimo.Domain.SolicitacaoImobiliariaBase>().Where(x => x.Id == idSolicitacaoBase).Select(x => x.StatusDaSolicitacao).FirstOrDefault();
            var idContrato = Consultar<CM.CMPrevWeb.Emprestimo.Domain.SolicitacaoRepactuacao>().Where(x => x.Id == idSolicitacaoBase).Select(x => x.Contrato.Id).FirstOrDefault();
            #region verificacoes statusSolicitacao e idContrato
            if (statusSolicitacao.IsNull())
            {
                ctr.OutErro = true;
                ctr.OutMessageInfo = "Status da solicitação nulo";
                return;
            }
            if (idContrato.IsNull())
            {
                ctr.OutErro = true;
                ctr.OutResult = false;
                ctr.OutMessageInfo = string.Format("EMPElegRepacSim - 93 - Não foi passado um Contrato para a regra!"); // @ErrorMsg
                ctr.OutMessageInfo = "Id do contrato nulo";
                return;
            }
            #endregion
            var statusInelegList = new List<CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum>();
            statusInelegList.Add(CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum.Cancelada);
            statusInelegList.Add(CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum.AditivoEnviado);
            statusInelegList.Add(CM.CMPrevWeb.CorePrev.Domain.Emprestimo.Enumerados.StatusDaSolicitacaoEnum.RGIRecebido);
            if ((itensAMigrar == 0m && ctr.InContrato.ValorSaldoDevedor == 0m) && ! (modalidades.Contains(ctr.InContrato.Modalidade.Codigo)))
            if (statusInelegList.Contains(statusSolicitacao))
            {
                ctr.OutResult = false;
                ctr.OutMessageInfo = "Participante não possuí contrato elegível para a repactuação.";
                ctr.OutMessageInfo = "Status da Repactuação não permite Simulação.";
                return;
            }
            // Se a consulta não retornar nenhum registro, devolver código de erro com a mensagem:
            if (solRepac == null)
            else
            {
                var consultaContratoQuery = Consultar<CM.CMPrevWeb.Emprestimo.Domain.Contrato>().Where(x => x.Id == idContrato).AsQueryable();
                var statusContrato = consultaContratoQuery.Select(x => x.StatusDoContrato).FirstOrDefault();
                var codTipoEmp = consultaContratoQuery.Select(x => x.Modalidade.TipoDeEmprestimo.Codigo).FirstOrDefault();
                #region verificacoes statusContrato e codTipoEmp
                if (statusContrato.IsNull())
                {
                    ctr.OutErro = true;
                ctr.OutResult = false;
                ctr.OutMessageInfo = "EMPElegRepacSim - 109 - Não localizada a Solicitação de Repactuação";
                    ctr.OutMessageInfo = "Status do contrato nulo";
                    return;
                }
            if (statusList.Contains(solRepac.StatusDaSolicitacao))
                if (codTipoEmp.IsNull())
                {
                ctr.OutResult = false;
                ctr.OutMessageInfo = "Status da Repactuação não permite Simulação.";
                    ctr.OutErro = true;
                    ctr.OutMessageInfo = "Código do tipo de empréstimo nulo";
                    return;
                }
            else
            {
                if (ctr.InContrato.StatusDoContrato == CM.CMPrevWeb.Emprestimo.Domain.StatusDoContratoEnum.Quitado)
                #endregion
                if (statusContrato == CM.CMPrevWeb.Emprestimo.Domain.StatusDoContratoEnum.Quitado || statusContrato == CM.CMPrevWeb.Emprestimo.Domain.StatusDoContratoEnum.Cancelado)
                {
                    ctr.OutResult = false;
                    ctr.OutMessageInfo = "Contrato Quitada.";
                    ctr.OutMessageInfo = "Contrato Quitado ou Cancelado.";
                    return;
                }
                else if (! modalidades.Contains(ctr.InContrato.Modalidade.Codigo))
                else if (!codigosTipoEmprestimo.Contains(codTipoEmp))
                {
                    ctr.OutResult = false;
                    ctr.OutMessageInfo = "Modalidade não permite Repactuação.";
                    ctr.OutMessageInfo = "Participante não possui contrato elegível para a repactuação.";
                    return;
                }
            }