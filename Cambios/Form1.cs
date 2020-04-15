
namespace Cambios
{
    using Modelos;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Net.Http;
    using Cambios.Servicos;
    using System.Threading.Tasks;

    public partial class Form1 : Form
    {
        #region Atribuitos

        private List<Rate> Rates;
        private NetworkService networkService;
        private ApiService ApiService;
        private DialogService DialogService;
        private DataService DataService;

        #endregion

        public Form1()
        {
            InitializeComponent();
            networkService = new NetworkService();
            ApiService = new ApiService();
            DialogService = new DialogService();
            DataService = new DataService();
            LoadRetes();
            
        }

        private async void LoadRetes() // await para que o sw continua a trabalhar enquanto carrega os no api deste modo usa-se async com await
        {
            bool load;

         //   lblEscolhe.Text = "Atualizar Taxas...";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                LoadLocalRetes();

                load = false;
            }
            else
            {
                await LoaApiRates();
                load = true;
            }

            //no caso o basedados não está preenchido
            if (Rates.Count == 0)
            {
                lblEscolhe.Text = "Não há ligação a Internet" + Environment.NewLine +
                    "e Não foram prévimente carregadas as taxas." + Environment.NewLine +
                    "Tenta mais tarde!";

                lblDetalhes.Text = "Primeira Inicialização devera ter ligação a internet";

                return;
            }


            // progressBar1.Value = 0;

           /* var client = new HttpClient(); //cria um http para fazer ligacao de http
           // client.BaseAddress = new Uri("https://cambiosrafa.azurewebsites.net"); //digo o endereco
            var response =  await client.GetAsync("/api/Rates"); // digo onde esta o controlador de api 
            var result = await response.Content.ReadAsStringAsync();// carrego o result no formato String para od ou variavel result          

           */

            comboBoxOrigem.DataSource = Rates;
            comboBoxOrigem.DisplayMember = "Name";

            //corrige bug da microsoft
            comboBoxDestino.BindingContext = new BindingContext(); //estou a dizer q combo de origeme difernte do destino, mas na verdade sao iguais

            comboBoxDestino.DataSource = Rates;
            comboBoxDestino.DisplayMember = "Name";


          


            lblEscolhe.Text = "Taxas Atualizadas...";

            if (load)
            {
                lblDetalhes.Text = string.Format("Taxas Carregadas da Internet em {0:F}", DateTime.Now);

            }
            else
            {
                lblDetalhes.Text = string.Format("Taxas carregas da Base de Dados");
            }

            progressBar1.Value = 100;
            btnConverter.Enabled = true;
            btnTroca.Enabled = true;
        }

        private void LoadLocalRetes()
        {
             Rates = DataService.GetData();
        }

        private  async Task LoaApiRates()
        {
            progressBar1.Value = 0;

            var response = await ApiService.getRates("https://cambiosrafa.azurewebsites.net","/api/Rates");

            Rates = (List<Rate>)response.Result;

            DataService.DeleteData();

            DataService.SaveData(Rates);
           
        }

        private void btnConverter_Click(object sender, EventArgs e)
        {
            Converter();
        }

        private void Converter()
        {
            if (string.IsNullOrEmpty(textBoxValor.Text))
            {
                DialogService.ShowMessage("Erro", "Insira um valor a Converter");
                return;
            }

            decimal valor;

            if(!decimal.TryParse(textBoxValor.Text, out valor))
            {
                DialogService.ShowMessage("Erro de Conversão", "Valor Terá que ser número");
                return;
            }

            if(comboBoxOrigem.SelectedItem == null)
            {
                DialogService.ShowMessage("Erro", "Tem que escolher uma moeda a converter");
                return;
            }
            if (comboBoxDestino.SelectedItem == null)
            {
                DialogService.ShowMessage("Erro", "Tem que escolher uma moeda de Destino a converter");
                return;
            }

            var taxaOrigem = (Rate) comboBoxOrigem.SelectedItem;
            var taxaDestino = (Rate) comboBoxDestino.SelectedItem;

            var valorConvertido = valor / (decimal)taxaOrigem.TaxRate * (decimal)taxaDestino.TaxRate;

            lblEscolhe.Text = string.Format("{0} {1:c2} = {2} {3:c2}", 
                taxaOrigem.Code, 
                valor,
                taxaDestino.Code, 
                valorConvertido);
        }

        private void btnTroca_Click(object sender, EventArgs e)
        {
            Troca();
        }

        private void Troca()
        {
            var aux = comboBoxOrigem.SelectedItem;
            comboBoxOrigem.SelectedItem = comboBoxDestino.SelectedItem;
            comboBoxDestino.SelectedItem = aux;
            Converter();
        }
    }
}
