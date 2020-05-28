using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZPF;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UNO.Sample.Webservice
{
   /// <summary>
   /// An empty page that can be used on its own or navigated to within a Frame.
   /// </summary>
   public sealed partial class MainPage : Page
   {
      public MainPage()
      {
         this.InitializeComponent();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Displaydata(string data)
      {
         if (data == null)
         {
            Displaydata(data);
            tbInfo.Text = $"data length=-1";
            tbData.Text = wsHelper.LastError;

            listView.ItemsSource = null;
         }
         else
         {
            tbInfo.Text = $"data length={data.Length}";
            tbData.Text = data;
            listView.ItemsSource = null;
         };
      }

      private void Displaydata<T>(string data)
      {
         if (data == null)
         {
            Displaydata(data);
         }
         else
         {
            tbInfo.Text = $"data length={data.Length}";
            tbData.Text = data;

            try
            {
               listView.ItemsSource = JsonSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
               listView.ItemsSource = new List<string> { ex.Message };
            };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      // http://worldtimeapi.org/

      private async void btnNowTxt_Click(object sender, RoutedEventArgs e)
      {
         var data = await wsHelper.wGet(new Uri("http://worldtimeapi.org/api/ip.txt"));

         Displaydata(data);
      }

      private async void btnNowJson_Click(object sender, RoutedEventArgs e)
      {
         var data = await wsHelper.wGet(new Uri("http://worldtimeapi.org/api/ip"));

         Displaydata(data);
      }

      private async void btnWorldtimeTxt_Click(object sender, RoutedEventArgs e)
      {
         var data = await wsHelper.wGet(new Uri("http://worldtimeapi.org/api/timezone/Europe/London.txt"));

         Displaydata(data);
      }

      private async void btnWorldtimeJson_Click(object sender, RoutedEventArgs e)
      {
         var data = await wsHelper.wGet(new Uri("http://worldtimeapi.org/api/timezone/Europe/London"));

         Displaydata(data);
      }

      private async void btnTimezonesJson_Click(object sender, RoutedEventArgs e)
      {
         var data = await wsHelper.wGet(new Uri("http://worldtimeapi.org/api/timezone/Europe"));

         Displaydata<List<string>>(data);
      }

      //private async void btnWeatherAlertTypesJson_Click(object sender, RoutedEventArgs e)
      //{
      //   var data = await wsHelper.wGet(new Uri("http://api.weather.gov/alerts/types"));

      //   Displaydata<List<string>>(data);
      //}

      private async void btnWeatherAlertTypesJson_Click(object sender, RoutedEventArgs e)
      {
         var data = await wsHelper.wGet(new Uri("http://wsdev.storecheck.pro/StoreCheck/Interventions/List2/ME/ME"));

         //Displaydata<List<Intervention_CE>>(data);
         Displaydata(data);

         listView.ItemsSource = await wsHelper.wGet<List<Intervention_CE>>(new Uri("http://wsdev.storecheck.pro/StoreCheck/Interventions/List2/ME/ME"));
      }

      public class Intervention_CE
      {
         public Int64 PK { get; set; }
         public string Ref { get; set; }

         public string Client { get; set; }
         public string Action { get; set; }

         public long FKMagasin { get; set; }

         /// <summary>
         /// User affected to this Intervention ... 
         /// </summary>
         public long FKIntervennant { get; set; }

         /// <summary>
         /// User Login en clair --> permet de supprimer l'utilisateur physiquement de la base sans perdre l'info.
         /// </summary>
         public string Intervennant { get; set; }

         public string Questionnaire { get; set; }
         public string Responses { get; set; }

         public string Observations { get; set; }

         /// <summary>
         /// Clos par l'intervennant
         /// </summary>
         public bool IsClosed { get; set; }

         /// <summary>
         /// Verifié et donc publié par l'admin
         /// </summary>
         public bool IsVerified { get; set; }

         /// <summary>
         /// Nombre de Km déclarés par l'intervenant
         /// </summary>
         public decimal NbKM { get; set; }

         /// <summary>
         /// Date Prèvue pour l'Intervention
         /// </summary>
         public DateTime DatePrevue { get; set; }

         /// <summary>
         /// Date heure Début Intervention
         /// </summary>
         public DateTime DateBeginIntervention { get; set; }

         /// <summary>
         /// Date heure Fin Intervention
         /// </summary>
         public DateTime DateEndIntervention { get; set; }

         /// <summary>
         /// Date heure Verifié et donc publié par l'admin
         /// </summary>
         public DateTime DateVerified { get; set; }

         public bool Actif { get; set; }
         public DateTime UpdatedOn { get; set; }

         // - - -  - - - 

         public Intervention_CE()
         {
         }

         // - - -  - - - 

         public override string ToString()
         {
            return $"{PK} {Client} {FKMagasin}";
         }
      }
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
