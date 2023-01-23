//Exemplo regra CBS
using CM.CMFlex.Core.Common;
using CM.CMFlex.Core.Domain;
using CM.CMFoundation.Common;
using CM.CMFoundation.Common.FileGenerator;
using CM.CMFoundation.Domain.Core;
using CM.CMFoundation.Framework;
using CM.CMFoundation.Messaging;
using CM.CMPrevWeb.BeneficioPrev.Contracts;
using CM.CMPrevWeb.CadastroPrev.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regras.TestProject.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Regras.TestProject.Regras.CBS.BeneficioPrev
{
    [TestClass, Regra("900", "PL6-CALCULA DATA INICIO DO AUXÍLIO PECUNIÁRIO")]
    public class PL6_CALCULA_DATA_INICIO_DO_AUXILIO_PECUNIÁRIO : RegrasBase<CalculoDeDataDeBeneficioContract>
    {
        [TestMethod, TestProperty("900", "PL6-CALCULA DATA INICIO DO AUXÍLIO PECUNIÁRIO")]
        public void MetodoDeTeste()
        {
            Contrato = new CalculoDeDataDeBeneficioContract()
            {
                //InParticipantePorPlano = Consultar<ParticipantePorPlano>().FirstOrDefault()
            };

            ExecutarTeste();
        }

        public override void Regra(CalculoDeDataDeBeneficioContract ctr)
        {
            //Desenvolvimento: 
            var W_DATA1 = default(DateTime);
            var W_VALOR1 = "";
            var DATAMORTE = ctr.InParticipantePorPlano.Associado.PessoaFisica.DataDeFalecimento;
            // Passos da regra do contrato

            W_DATA1 = (DateTime)DATAMORTE; // [Passo:10]Atribuir à variavel W_DATA1 o valor do campo Data do Falecimento da Pessoa
            W_VALOR1 = "12"; // [Passo:20]Atribuir à variável W_VALOR1 o valor constante 12
            W_VALOR1 = W_DATA1.AddMonths(W_VALOR1.To<int>()).ToString(); // [Passo:30]Atribuir à variável W_VALOR1 o valor da formula EVOLUIR DATA COM Nº MÊS(@W_DATA1,@W_VALOR1)
            ctr.OutResultado = W_VALOR1.To<DateTime>();
            return; // [Passo:40]Parar a Regra atual


        }
    }
}