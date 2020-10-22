using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFZadanie2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AktualizujInformacje();
        }

        private void AnulujButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Zamówienie zostało przyjęte", "Potwierdzenie", MessageBoxButton.OK);
            ResetujFormularz();
        }

        private void ResetujFormularz()
        {
            NakladTextBox.Text = "0";
            FormatSlider.Value = 1;
            KolorCheckBox.IsChecked = false;
            KolorComboBox.SelectedItem = null;
            foreach (RadioButton radio in GramaturaStackPanel.Children)
            {
                if ((string)radio.Tag == "1,0")
                    radio.IsChecked = true;
                else
                    radio.IsChecked = false;
            }
            foreach (CheckBox check in OpcjeStackPanel.Children)
            {
                check.IsChecked = false;
            }

            StandardRadioButton.IsChecked = true;
            EkspresRadioButton.IsChecked = false;


            AktualizujInformacje();
        }
        private void AktualizujInformacje()
        {
            if (FormatSlider == null)
                return;
            // stringbuilder odpowiadający za ostateczną prezentację informaji
            StringBuilder builder = new StringBuilder();

            // podstawowa cena za odpowiednią ilość stron
            int naklad = 0;
            int.TryParse(NakladTextBox.Text, out naklad);
            int cenaZaStrone = (int)(20 * Math.Pow(2.5, FormatSlider.Value));
            double cena = naklad * cenaZaStrone;
            builder.Append($"Przedmiot zamówienia: {naklad} szt., format A{5 - FormatSlider.Value}");

            // doliczenie mnożnika za gramaturę
            if (GramaturaStackPanel == null)
                return;
            double gramatura = 1.0;
            foreach (RadioButton radio in GramaturaStackPanel.Children)
            {
                if (radio.Tag == null)
                    return;
                if (radio.IsChecked == true)
                {
                    double.TryParse(radio.Tag.ToString(), out gramatura);
                    break;
                }
            }
            cena *= gramatura;
            string g = null;
            switch(gramatura)
            {
                case 1.0:
                    {
                        g = "80";
                        break;
                    }
                case 2.0:
                    {
                        g = "120";
                        break;
                    }
                case 2.5:
                    {
                        g = "200";
                        break;
                    }
                case 3:
                    {
                        g = "340";
                        break;
                    }
            }
            builder.Append($", {g}g/m\x00B2");

            // doliczenie kosztów za zamówienie ekspresowe
            if (EkspresRadioButton == null)
                return;
            if (EkspresRadioButton.IsChecked == true)
            {
                cena += 15.0;
                builder.Append(", zamówienie ekspresowe");
            }

            // dodatkowe opcje
            double opcje = 1;
            foreach (CheckBox checkBox in OpcjeStackPanel.Children)
            {
                if(checkBox.IsChecked == true)
                {
                    double d = 1;
                    double.TryParse(checkBox.Tag.ToString(), out d);
                    opcje *= d;
                    switch(checkBox.Name)
                    {
                        case "DrukKolorCheckBox":
                            {
                                builder.Append(", druk kolor");
                                break;
                            }
                        case "DwustronnyCheckBox":
                            {
                                builder.Append(", druk dwustronny");
                                break;
                            }
                        case "UVCheckBox":
                            {
                                builder.Append(", lakier UV");
                                break;
                            }
                    }
                }
            }
            cena *= (1.0 + opcje);

            //kolorywy papier
            if (KolorCheckBox.IsChecked == true)
            {
                cena *= 1.5;
                ComboBoxItem c = KolorComboBox.SelectedItem as ComboBoxItem;
                builder.Append($", kolorowa kartka({c?.Tag ?? " "})");
            }

            builder.AppendLine();

            // rabat
            double rabat = (naklad / 100) * 0.01;
            if (rabat > 0.1)
                rabat = 0.1;

            builder.AppendLine($"Cena przed rabatem: {cena :F2}zł");
            builder.AppendLine($"Naliczony rabat: {rabat * 100 :F0}%");
            builder.AppendLine($"Cena po rabacie: {cena * (1.0 - rabat) :F2}zł");
            InformacjeTextBox.Text = builder.ToString();
        }

        private void KolorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            KolorComboBox.IsEnabled = true;
            AktualizujInformacje();
        }

        private void KolorCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            KolorComboBox.SelectedIndex = -1;
            KolorComboBox.IsEnabled = false;
            AktualizujInformacje();
        }

        private void FormatSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int cena = (int)(20 * Math.Pow(2.5, FormatSlider.Value));
            switch (FormatSlider.Value)
            {
                case 0:
                    {
                        FormatLabel.Content = $"A5 - cena {cena}gr/szt.";
                        break;
                    }
                case 1:
                    {
                        FormatLabel.Content = $"A4 - cena {cena}gr/szt.";
                        break;
                    }
                case 2:
                    {
                        FormatLabel.Content = $"A3 - cena {cena}gr/szt.";
                        break;
                    }
                case 3:
                    {
                        FormatLabel.Content = $"A2 - cena {cena}gr/szt.";
                        break;
                    }
                case 4:
                    {
                        FormatLabel.Content = $"A1 - cena {cena}gr/szt.";
                        break;
                    }
                case 5:
                    {
                        FormatLabel.Content = $"A0 - cena {cena}gr/szt.";
                        break;
                    }
                default:
                    break;
            }
            AktualizujInformacje();
        }
        private void GramaturaRadioBox_Chenged(object sender, RoutedEventArgs e)
        {
            AktualizujInformacje();
        }

        private void OpcjeCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            AktualizujInformacje();
        }

        private void KolorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AktualizujInformacje();
        }

        private void NakladTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AktualizujInformacje();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
