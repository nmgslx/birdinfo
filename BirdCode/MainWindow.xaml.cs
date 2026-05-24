using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BirdCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string[]> birds;
        List<int> found;
        int current;

        new Dictionary<string, int> code4, code6, codesp;

        public MainWindow()
        {
            InitializeComponent();
            birds = new List<string[]>();
            code4 = new Dictionary<string, int>();
            code6 = new Dictionary<string, int>();
            codesp = new Dictionary<string, int>();

            string [] header_name;
            bool header = true;

            foreach (string line in File.ReadAllLines(@"merged_birds.csv"))
            {
                if (header)
                {
                    header_name = SplitCsvLine(line).ToArray<string>();
                    header = false;
                }
                else
                    birds.Add(SplitCsvLine(line).ToArray<string>());
            }

            for (int i = 0; i < birds.Count; i++)
            {
                var x = birds[i][17];
                if (!String.IsNullOrEmpty(x))
                {
                    if (!code4.ContainsKey(x))
                        code4.Add(x, i);
                    else if (birds[i][7] == x)
                        code4[x] = i;
                }
                x = birds[i][2];
                if (!String.IsNullOrEmpty(x))
                {
                    if (!codesp.ContainsKey(x))
                        codesp.Add(x, i);
                }
                x = birds[i][9];
                if (!String.IsNullOrEmpty(x))
                {
                    if (!code6.ContainsKey(x))
                        code6.Add(x, i);
                }
                x = birds[i][5] + " | " + birds[i][6];
                if (!cbOrdFam.Items.Contains(x))
                    cbOrdFam.Items.Add(x);

            }
            found = new List<int>();
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            if (current<found.Count-1)
            {
                current++;
                Display(birds[found[current]]);
            }
        }

        private void Button_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (current > 0)
            {
                current--;
                Display(birds[found[current]]);
            }
        }

        public static List<string> SplitCsvLine(string line)
        {
            List<string> result = new List<string>();
            StringBuilder current = new StringBuilder();

            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    // Handle escaped quote ("")
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result;
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            tbCommonName.Text = "";
            tb4Code.Text = "";
            tb6Code.Text = "";
            tbSpCode.Text = "";
            tbCnName.Text = "";
            tbSciName.Text = "";
            cbOrdFam.SelectedIndex = -1;
        }

        private void Button_Find_Click(object sender, RoutedEventArgs e)
        {
            string[] bird = null;
            var c4 = tb4Code.Text.Trim();
            var c6 = tb6Code.Text.Trim(); 
            var sp = tbSpCode.Text.Trim();
            var comname = tbCommonName.Text;
            var cnname = tbCnName.Text;
            var scname = tbSciName.Text;
            var dp = cbOrdFam.SelectedItem is null ? null : cbOrdFam.SelectedItem.ToString();
            if (!String.IsNullOrWhiteSpace(c4) && code4.ContainsKey(c4))
            {
                bird = birds[code4[c4]];
            }
            else if (!String.IsNullOrWhiteSpace(c6) && code6.ContainsKey(c6))
            {
                bird = birds[code6[c6]];
            }
            else if (!String.IsNullOrWhiteSpace(sp) && codesp.ContainsKey(sp))
            {
                bird = birds[codesp[sp]];
            }
            else if (!String.IsNullOrWhiteSpace(comname) || !String.IsNullOrWhiteSpace(cnname) || !String.IsNullOrWhiteSpace(scname))
            {
                found = new List<int>();
                for (int i=0;i< birds.Count;i++)
                { 
                    if (!String.IsNullOrWhiteSpace(comname) && birds[i][1].ToLower().Contains(comname.ToLower()) ||
                        !String.IsNullOrWhiteSpace(cnname) && birds[i][10].ToLower().Contains(cnname.ToLower()) ||
                        !String.IsNullOrWhiteSpace(scname) && birds[i][0].ToLower().Contains(scname.ToLower()) )
                    {
                        found.Add(i);
                    }
                }
                if (found.Count > 0) bird = birds[found[0]];
                current = 0;
            }
            else if (!String.IsNullOrWhiteSpace(dp))
            {
                found = new List<int>();
                for (int i = 0; i < birds.Count; i++)
                {
                    if (birds[i][5] + " | " + birds[i][6] == dp)
                    {
                        found.Add(i);
                    }
                }
                if (found.Count > 0) bird = birds[found[0]];
                current = 0;
            }

            Display(bird);
        }

        private void Display(string[] bird)
        {
            if (bird == null) return;
            lableDisp.Content = current + 1 + ":" + found.Count;
            tbCommonName.Text = bird[1];
            tb4Code.Text = bird[17];
            tb6Code.Text = bird[9];
            tbSpCode.Text = bird[2];
            tbSciName.Text = bird[0];
            tbCnName.Text = bird[10];
            cbOrdFam.SelectedItem = bird[5] + " | " + bird[6];
        }

    }
}
