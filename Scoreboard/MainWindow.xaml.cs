using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Scoreboard
{
    public partial class MainWindow : Window
    {
        private GraphQLHttpClient client = new GraphQLHttpClient("https://api.smash.gg/gql/alpha", new NewtonsoftJsonSerializer());
        private const string token = "ceffaeed94948ae90d5c11287386b979";

        private List<Tournament> searchedTournaments = new List<Tournament>();
        private List<string> searchedTournamentsNames = new List<string>();

        private List<TournamentItem> localTournaments = new List<TournamentItem>();
        private List<Event> localEvents = new List<Event>();
        private List<Phase> localPhases = new List<Phase>();
        private List<PhaseGroup> localGroups = new List<PhaseGroup>();
        private List<Set> localSets = new List<Set>();

        protected OBSWebsocket obs;

        private string outputDir;

        private bool isDoubles = false;
        private bool isSmashgg = false;
        private bool roundNum = true;
        private bool obsIsConnected = false;

        private List<Command> commands = new List<Command>();
        private List<string> keys = new List<string>();

        private bool isEditing = false;

        private bool showUrl = false;
        private string lastIp;

        private bool search = true;

        public MainWindow()
        {
            InitializeComponent();

            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            LoadTournamentFile();

            cbTournaments.SelectedIndex = 0;

            LoadSettings();
            LoadCommands();

            EnableUpdateButton(btnSaveSettings, false);

            obs = new OBSWebsocket();

            Action dispatcherConnect = new Action(() =>
            {
                gridConnection.IsEnabled = false;
                gridOptions.IsEnabled = true;
                btnObsConnection.IsDefault = false;

                SuccessfulConnection();
                btnObsConnection.Content = "Disconnect";

                obsIsConnected = true;

                lastIp = tbIp1.Text + "." + tbIp2.Text + "." + tbIp3.Text + "." + tbIp4.Text + ":" + tbPort.Text;
                SaveSettingsFunc();
            });

            Action dispatcherDisconnect = new Action(() =>
            {
                gridConnection.IsEnabled = true;
                gridOptions.IsEnabled = false;
                btnObsConnection.IsDefault = true;

                lblConnection.Content = "Disconnected";
                lblConnection.Foreground = Brushes.Red;

                btnObsConnection.Content = "Connect";

                obsIsConnected = false;
            });

            void OnConnect(object sender, EventArgs e)
            {
                Dispatcher.Invoke(dispatcherConnect, DispatcherPriority.Normal);
            }

            void OnDisconnect(object sender, ObsDisconnectionInfo e)
            {
                Dispatcher.Invoke(dispatcherDisconnect, DispatcherPriority.Normal);
            }

            obs.Connected += OnConnect;
            obs.Disconnected += OnDisconnect;


            initializeThemes();
            btnSaveSettings.IsEnabled = false;
        }

        private void ClearFields()
        {
            cbEvent.Items.Clear();
            localEvents.Clear();

            cbPhase.Items.Clear();
            localPhases.Clear();

            cbRound.Items.Clear();

            tbPlayer1.Text = "";
            tbPlayer2.Text = "";
            tbPlayer3.Text = "";
            tbPlayer4.Text = "";

            lbScore1.Content = "0";
            lbScore2.Content = "0";
        }

        private void InitializeManual()
        {
            ClearFields();

            isSmashgg = false;
            cbRound.IsEnabled = true;
            tbRoundNum.IsEnabled = true;
            tbRoundNum.Text = "1";
            tbPlayer1.IsEnabled = true;
            tbPlayer2.IsEnabled = true;
            tbPlayer3.IsEnabled = true;
            tbPlayer4.IsEnabled = true;
            chkLosers1.IsEnabled = true;
            chkLosers2.IsEnabled = true;

            string[] events = { "Singles Bracket", "Doubles Bracket" };
            string[] brackets = { "Winners", "Losers" };
            string[] rounds = { "Round", "Quarters", "Semis", "Finals" };

            foreach (string ev in events)
            {
                cbEvent.Items.Add(ev);
            }

            cbEvent.SelectedIndex = 0;

            foreach (string b in brackets)
            {
                foreach (string r in rounds)

                    cbRound.Items.Add(b + " " + r);
            }

            cbRound.Items.Add("Grand Finals");
            cbRound.Items.Add("Grand Finals Reset");

            cbRound.SelectedIndex = 0;

            gridSmashGG.Visibility = Visibility.Hidden;

            cbEvent.IsEnabled = true;

            Thickness margin = roundGrid.Margin;
            margin.Top = 8;
            roundGrid.Margin = margin;
        }

        private void InitializeSmashGG(string slug)
        {
            ClearFields();

            isSmashgg = true;
            cbPhase.IsEnabled = false;
            cbPhaseGroup.IsEnabled = false;
            cbSet.IsEnabled = false;
            cbRound.IsEnabled = false;
            tbRoundNum.IsEnabled = false;
            tbPlayer1.IsEnabled = false;
            tbPlayer2.IsEnabled = false;
            tbPlayer3.IsEnabled = false;
            tbPlayer4.IsEnabled = false;
            chkLosers1.IsEnabled = false;
            chkLosers2.IsEnabled = false;

            cbEvent.IsEnabled = false;

            System.Windows.Forms.Application.UseWaitCursor = true;
            LoadTournament(slug);
        }

        private void tbSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (tbSearch.Text.Length < 4)
            {
                searchedTournaments.Clear();
                searchedTournamentsNames.Clear();
                listTournaments.ItemsSource = null;
                listTournaments.Visibility = Visibility.Hidden;
                listTournaments.IsDropDownOpen = false;
            }
        }

        private void tbSearch_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (tbSearch.Text.Length < 4)
            {
                searchedTournaments.Clear();
                searchedTournamentsNames.Clear();
                listTournaments.ItemsSource = null;
                listTournaments.Visibility = Visibility.Hidden;
                listTournaments.IsDropDownOpen = false;
            }
            else if(e.Key == Key.Enter && search) {
                SearchTournament(tbSearch.Text);
            }
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSubmitUrl.IsEnabled = false;
            

            if (!string.IsNullOrEmpty(tbSearch.Text) && !string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                cbTournaments.SelectedIndex = 0;

                if (tbSearch.Text.Length >= 4 && search)
                    SearchTournament(tbSearch.Text);
            }
        }

        private async void SearchTournament(string name)
        {
            try
            {
                GraphQLRequest request = new GraphQLRequest
                {
                    Query = @"
                        query TournamentsByName($name: String!) {
                            tournaments(query: {
                              filter: {
                                name: $name,
                                upcoming: true
                              }
                            }) {
                            nodes {
                              name
                              slug
                            }
                          }
                        }",
                    Variables = new
                    {
                        name = name
                    }
                };

                var response = await client.SendQueryAsync(request, () => new { Tournaments = new TournamentConnection() });

                if (response.Data.Tournaments.nodes != null)
                {
                    searchedTournaments.Clear();
                    searchedTournamentsNames.Clear();
                    listTournaments.ItemsSource = null;
                    listTournaments.Visibility = Visibility.Hidden;

                    if (response.Data.Tournaments.nodes.Count > 0)
                    {

                        foreach (Tournament t in response.Data.Tournaments.nodes)
                        {
                            searchedTournaments.Add(t);
                            searchedTournamentsNames.Add(t.name);
                        }

                        searchedTournaments.Sort((p, q) => p.name.CompareTo(q.name));
                        searchedTournamentsNames.Sort();

                        listTournaments.ItemsSource = searchedTournamentsNames;

                        listTournaments.IsDropDownOpen = true;
                        listTournaments.Visibility = Visibility.Visible;
                    }
                    else
                        listTournaments.IsDropDownOpen = false;
                }
            }
            catch (Exception)
            {
            }
        }



        private void BtnSubmitUrl_Click(object sender, RoutedEventArgs e)
        {
            string slug = "";

            if (!string.IsNullOrEmpty(tbSearch.Text) && !string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                slug = searchedTournaments[searchedTournamentsNames.IndexOf(tbSearch.Text)].slug;
                btnSubmitUrl.IsEnabled = false;
                
            }
            else if(cbTournaments.SelectedIndex >= 1)
            {
                slug = localTournaments[cbTournaments.SelectedIndex].slug;
            }

            InitializeSmashGG(slug);
        }

        private void CbEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEvent.SelectedIndex >= 0 && !cbEvent.SelectedValue.ToString().Contains("..."))
            {
                if (localEvents.Count > 1 && cbEvent.Items[0].ToString().Contains("..."))
                {
                    cbEvent.Items.Remove(cbEvent.Items[0]);
                    localEvents.RemoveAt(0);
                }

                string ev = cbEvent.SelectedValue.ToString().ToLower();
                if (ev.Contains("double") || ev.Contains("team"))
                {
                    isDoubles = true;

                    tbPlayer3.Visibility = Visibility.Visible;
                    tbPlayer4.Visibility = Visibility.Visible;

                    Thickness swapMargin = btnSwap.Margin;
                    swapMargin.Top = 158;
                    btnSwap.Margin = swapMargin;

                    lbSide1.Content = "Team 1";
                    lbSide2.Content = "Team 2";
                }
                else
                {
                    isDoubles = false;

                    tbPlayer3.Visibility = Visibility.Hidden;
                    tbPlayer4.Visibility = Visibility.Hidden;


                    Thickness swapMargin = btnSwap.Margin;
                    swapMargin.Top = 132;
                    btnSwap.Margin = swapMargin;


                    lbSide1.Content = "Player 1";
                    lbSide2.Content = "Player 2";
                }

                if (!isSmashgg)
                    CheckSaveState();
                else
                    LoadEvent();
            }
        }

        private void CbRound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRound.SelectedIndex >= 0)
            {
                chkLosers1.IsChecked = false;
                chkLosers2.IsChecked = false;

                Thickness roundMargin = cbRound.Margin;
                if (cbRound.SelectedValue.ToString().ToLower().Contains("round"))
                {
                    tbRoundNum.Visibility = Visibility.Visible;
                    cbRound.Width = 152;
                    roundMargin.Right = 28;
                    roundNum = true;
                }
                else
                {
                    tbRoundNum.Visibility = Visibility.Collapsed;
                    cbRound.Width = 180;
                    roundMargin.Right = 0;
                    roundNum = false;
                }

                if (cbRound.SelectedValue.ToString().ToLower().Contains("grand"))
                {
                    chkLosers1.Visibility = Visibility.Visible;
                    chkLosers2.Visibility = Visibility.Visible;

                    if (isSmashgg)
                    {
                        chkLosers2.IsChecked = true;
                    }

                    else
                    {
                        chkLosers1.IsEnabled = true;
                        chkLosers2.IsEnabled = true;
                    }

                    if (cbRound.SelectedValue.ToString().ToLower().Contains("reset"))
                    {
                        chkLosers1.IsChecked = true;
                        chkLosers2.IsChecked = true;

                        if (!isSmashgg)
                        {
                            chkLosers1.IsEnabled = false;
                            chkLosers2.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    chkLosers1.Visibility = Visibility.Hidden;
                    chkLosers2.Visibility = Visibility.Hidden;
                }

                cbRound.Margin = roundMargin;

                CheckSaveState();
            }
            else
            {
                chkLosers1.Visibility = Visibility.Hidden;
                chkLosers2.Visibility = Visibility.Hidden;
            }
        }

        private void P1ScoreUp(object sender, RoutedEventArgs e)
        {
            int score1 = int.Parse(lbScore1.Content.ToString());

            if (score1 < 99)
            {
                score1++;
                lbScore1.Content = score1;

                CheckSaveState();
            }
        }
        private void P1ScoreDown(object sender, RoutedEventArgs e)
        {
            int score1 = int.Parse(lbScore1.Content.ToString());

            if (score1 > 0)
            {
                score1--;
                lbScore1.Content = score1;

                CheckSaveState();
            }
        }
        private void P2ScoreUp(object sender, RoutedEventArgs e)
        {
            int score2 = int.Parse(lbScore2.Content.ToString());

            if (score2 < 99)
            {
                score2++;
                lbScore2.Content = score2;

                CheckSaveState();
            }
        }
        private void P2ScoreDown(object sender, RoutedEventArgs e)
        {
            int score2 = int.Parse(lbScore2.Content.ToString());

            if (score2 > 0)
            {
                score2--;
                lbScore2.Content = score2;

                CheckSaveState();
            }
        }


        private void SwapPlayers(object sender, RoutedEventArgs e)
        {
            string auxPlayer = tbPlayer1.Text;
            tbPlayer1.Text = tbPlayer2.Text;
            tbPlayer2.Text = auxPlayer;

            if (isDoubles)
            {
                string auxTeammate = tbPlayer3.Text;
                tbPlayer3.Text = tbPlayer4.Text;
                tbPlayer4.Text = auxTeammate;
            }

            string auxScore = lbScore1.Content.ToString();
            lbScore1.Content = lbScore2.Content.ToString();
            lbScore2.Content = auxScore;

            bool auxChk = chkLosers1.IsChecked.Value;
            chkLosers1.IsChecked = chkLosers2.IsChecked;
            chkLosers2.IsChecked = auxChk;
        }

        private void SwapCasters(object sender, RoutedEventArgs e)
        {
            string aux = tbCaster1.Text;
            tbCaster1.Text = tbCaster2.Text;
            tbCaster2.Text = aux;

            CheckCasters();
        }

        private void CheckTextboxes(object sender, TextChangedEventArgs e)
        {
            CheckSaveState();
        }

        private void CheckSaveState()
        {
            bool enabled;

            enabled = !string.IsNullOrEmpty(tbPlayer1.Text) && !string.IsNullOrWhiteSpace(tbPlayer1.Text) &&
                !string.IsNullOrEmpty(tbPlayer2.Text) && !string.IsNullOrWhiteSpace(tbPlayer2.Text);

            if (isDoubles)
            {
                enabled &= !string.IsNullOrEmpty(tbPlayer3.Text) && !string.IsNullOrWhiteSpace(tbPlayer3.Text) &&
                    !string.IsNullOrEmpty(tbPlayer4.Text) && !string.IsNullOrWhiteSpace(tbPlayer4.Text);
            }

            if (cbRound.SelectedValue != null && cbRound.SelectedValue.ToString().ToLower().Contains("grand"))
            {
                if (cbRound.SelectedValue != null && cbRound.SelectedValue.ToString().ToLower().Contains("reset"))
                    enabled &= chkLosers1.IsChecked.Value && chkLosers2.IsChecked.Value;
                else
                    enabled &= chkLosers1.IsChecked.Value || chkLosers2.IsChecked.Value;
            }

            EnableUpdateButton(btnSave, enabled);
        }

        private void CheckSaveCasters(object sender, TextChangedEventArgs e)
        {
            CheckCasters();
        }

        private void CheckCasters()
        {
            bool c1 = !string.IsNullOrEmpty(tbCaster1.Text) && !string.IsNullOrWhiteSpace(tbCaster1.Text);
            bool c2 = !string.IsNullOrEmpty(tbCaster2.Text) && !string.IsNullOrWhiteSpace(tbCaster2.Text);

            gridSecondCaster.IsEnabled = c1;
            EnableUpdateButton(btnSaveCasters, c1);
            btnSwapCasters.IsEnabled = c2;
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(outputDir + "event.txt", cbEvent.Text);

            string round = cbRound.SelectedValue.ToString();

            if (roundNum)
                round += " " + tbRoundNum.Text;

            File.WriteAllText(outputDir + "round.txt", round);

            string p1 = tbPlayer1.Text;
            string p2 = tbPlayer2.Text;

            if (isDoubles)
            {
                p1 += " & " + tbPlayer3.Text;
                p2 += " & " + tbPlayer4.Text;
            }

            if (chkLosers1.IsChecked.Value)
                p1 += " [L]";

            if (chkLosers2.IsChecked.Value)
                p2 += " [L]";

            File.WriteAllText(outputDir + "player1.txt", p1);
            File.WriteAllText(outputDir + "player2.txt", p2);
            File.WriteAllText(outputDir + "match.txt", p1 + " vs " + p2);

            string score1 = lbScore1.Content.ToString();
            string score2 = lbScore2.Content.ToString();

            File.WriteAllText(outputDir + "score1.txt", score1);
            File.WriteAllText(outputDir + "score2.txt", score2);

            EnableUpdateButton(btnSave, false);
        }

        private void SaveCasters(object sender, RoutedEventArgs e)
        {
            string c1 = tbCaster1.Text;
            string c2 = tbCaster2.Text;

            File.WriteAllText(outputDir + "caster1.txt", c1);

            if (!string.IsNullOrEmpty(c2) && !string.IsNullOrWhiteSpace(c2))
                File.WriteAllText(outputDir + "caster2.txt", c2);

            EnableUpdateButton(btnSaveCasters, false);
        }

        private void BrowseOutput(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();

            if (!string.IsNullOrEmpty(folder.SelectedPath))
            {
                string path = folder.SelectedPath;

                tbOutputFolder.Text = path;
            }
        }

        private void LoadSettings()
        {
            if (File.Exists("settings.json"))
            {
                string settingsString = File.ReadAllText("settings.json");
                Settings settings = JsonConvert.DeserializeObject<Settings>(settingsString);

                string outputPath = settings.outputPath;
                tbOutputFolder.Text = outputPath;
                outputDir = outputPath + "\\";

                string theme = settings.theme;
                cbThemes.SelectedValue = theme;

                string obsIp = settings.obsIp;

                if (!string.IsNullOrEmpty(obsIp) && !string.IsNullOrWhiteSpace(obsIp))
                {
                    string[] formattedIp = obsIp.Split(':');
                    string port = formattedIp[1];
                    string[] ipFields = formattedIp[0].Split('.');

                    tbIp1.Text = ipFields[0];
                    tbIp2.Text = ipFields[1];
                    tbIp3.Text = ipFields[2];
                    tbIp4.Text = ipFields[3];

                    tbPort.Text = port;
                }
            }
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            SaveSettingsFunc();

            ((App)System.Windows.Application.Current).ChangeSkin(cbThemes.SelectedValue.ToString());

            EnableUpdateButton(btnSaveSettings, false);
        }

        private void SaveSettingsFunc()
        {
            Settings newSettings = new Settings
            {
                outputPath = tbOutputFolder.Text,
                theme = cbThemes.SelectedValue.ToString(),
                obsIp = lastIp
            };

            string json = JsonConvert.SerializeObject(newSettings);
            File.WriteAllText("settings.json", json);

            outputDir = newSettings.outputPath + "\\";

            Console.WriteLine("Guardado mano");
        }

        private void SettingsTextChanged(object sender, TextChangedEventArgs e)
        {
            EnableUpdateButton(btnSaveSettings, true);
        }

        private void cbThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableUpdateButton(btnSaveSettings, true);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isEditing = false;
            tbCommandName.Text = "";
            tbCommandValue.Text = "";
            btnSaveCommand.Content = "Add";
        }

        private void EnableBtnApply(object sender, EventArgs e)
        {
            btnApplyCommand.IsEnabled = true;
        }

        private void BtnObsConnection_Click(object sender, RoutedEventArgs e)
        {
            btnObsConnection.IsEnabled = false;
            tabControl.IsEnabled = false;
            ObsConnect();
            tabControl.IsEnabled = true;
            btnObsConnection.IsEnabled = true;
        }

        private void ObsConnect()
        {
            string ip = tbIp1.Text + "." + tbIp2.Text + "." + tbIp3.Text + "." + tbIp4.Text + ":";
            string url = "ws://" + ip;
            string port = tbPort.Text;
            string password = pbPassword.Password;

            try
            {
                if (!obsIsConnected)
                    obs.ConnectAsync(url + port, password);
                else
                    obs.Disconnect();
            }
            catch (Exception)
            {
            }
        }

        private void SuccessfulConnection()
        {
            lblConnection.Content = "Connected";
            lblConnection.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xff, 0x00));
        }

        private void EditCommand(object sender, RoutedEventArgs e)
        {
            EditCommandValues(commands[listCommand.SelectedIndex].name, commands[listCommand.SelectedIndex].value);
            isEditing = true;
        }

        private void SaveCommand(string name, string value, bool isEditing)
        {
            if (!isEditing)
            {
                Command command = new Command
                {
                    name = name,
                    value = value
                };

                commands.Add(command);
            }
            else
            {
                Command command = commands[listCommand.SelectedIndex];
                command.name = name;
                command.value = value;
                commands[listCommand.SelectedIndex] = command;
            }
            string json = JsonConvert.SerializeObject(commands.ToArray());
            File.WriteAllText("commands.json", json);

            keys.Clear();

            LoadCommands();
        }

        private void LoadCommands()
        {
            if (File.Exists("commands.json"))
            {
                string commandsString = File.ReadAllText("commands.json");
                commands = JsonConvert.DeserializeObject<Command[]>(commandsString).ToList();
                listCommand.Items.Clear();
                foreach (Command c in commands)
                {
                    listCommand.Items.Add(c.name);
                }
            }
        }

        private void RemoveCommand(object sender, RoutedEventArgs e)
        {

            Window message = DisplayMessage("Delete command \"" + listCommand.SelectedValue + "\"?", "check", "yesno");
            bool dialogResult = (bool)message.ShowDialog();

            if (dialogResult)
            {
                commands.Remove(commands[listCommand.SelectedIndex]);

                string json = JsonConvert.SerializeObject(commands.ToArray());
                File.WriteAllText("commands.json", json);
                LoadCommands();
            }
        }

        private void ListCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool enabled = listCommand.SelectedIndex >= 0 && listCommand.Items.Count > 0;

            btnRemoveCommand.IsEnabled = enabled;
            btnEditCommand.IsEnabled = enabled;
            btnApplyCommand.IsEnabled = enabled;

            isEditing = false;
        }

        private void BtnApplyCommand_Click(object sender, RoutedEventArgs e)
        {
            char[] separators = { ' ', '+' };
            string[] keys = commands[listCommand.SelectedIndex].value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string[] modTypes = { "alt", "shift", "control" };
            bool isModifier;

            JObject o;
            List<JObject> modifiers = new List<JObject>();

            foreach (string k in keys)
            {
                isModifier = false;
                foreach (string type in modTypes)
                {
                    if (k.ToLower().Contains(type))
                    {
                        isModifier = true;
                        JObject m = new JObject();
                        m.Add(type, true);
                        modifiers.Add(m);
                    }
                }

                if (!isModifier)
                {
                    o = new JObject();
                    o.Add("keyId", "OBS_KEY_" + k);
                    foreach (JObject mod in modifiers)
                    {
                        o.Add("keyModifiers", mod);
                    }
                    modifiers.Clear();

                    obs.SendRequest("TriggerHotkeyByKeySequence", o);
                }
            }
        }

        private void TbInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            HighlightInfoButton(!string.IsNullOrEmpty(tbInfo.Text) && !string.IsNullOrWhiteSpace(tbInfo.Text));
        }

        private void BtnSaveInfo_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(outputDir + "info.txt", tbInfo.Text);
            HighlightInfoButton(false);
        }

        private void TabControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Window message = DisplayMessage("Close this program?", "check", "yesno");

            e.Cancel = !(bool)message.ShowDialog();
        }

        private void EnableUpdateButton(System.Windows.Controls.Button target, bool enabled)
        {
            target.IsEnabled = enabled;
        }

        private void HighlightInfoButton(bool highlight)
        {
            btnSaveInfo.IsEnabled = highlight;
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            InitializeManual();
            gridStart.Visibility = Visibility.Hidden;
            tabControl.Visibility = Visibility.Visible;
        }

        private void BtnSmashGG_Click(object sender, RoutedEventArgs e)
        {
            DoShowUrl();
        }

        private void DoShowUrl()
        {
            listTournaments.IsDropDownOpen = false;
            tbSearch.Text = "";

            showUrl = !showUrl;
            Thickness margin = gridStart.Margin;

            if (showUrl)
            {
                gridStart.Height = 216;
                gridUrl.Visibility = Visibility.Visible;
                btnSmashGG.Content = Resources["smashGGButtonCancel"];
                btnSmashGG.Width = 32;
                btnManual.Visibility = Visibility.Hidden;
            }
            else
            {
                gridStart.Height = 32;
                gridUrl.Visibility = Visibility.Hidden;
                btnSmashGG.Content = Resources["smashGGButtonConnect"];
                btnSmashGG.Width = 190;
                btnManual.Visibility = Visibility.Visible;
            }

            gridStart.Margin = margin;

            margin = roundGrid.Margin;
            margin.Top = 56;
            roundGrid.Margin = margin;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Window message = DisplayMessage("Go back to the main menu?", "check", "yesno");

            bool dialogResult = (bool)message.ShowDialog();

            if (dialogResult)
            {
                tabControl.Visibility = Visibility.Hidden;
                gridStart.Visibility = Visibility.Visible;
                btnRefresh.Visibility = Visibility.Hidden;

                if (isSmashgg)
                    ClearSelectiveFields("P");

                LoadTournamentFile();
                cbTournaments.SelectedIndex = 0;
                tbSearch.Text = "";

                if (isSmashgg)
                    Title = "Stream Scoreboard";
            }
        }

        private async void LoadTournament(string slug)
        {
            try
            {
                GraphQLRequest request = new GraphQLRequest
                {
                    Query = @"
                        query TournamentQuery($slug: String) {
		                    tournament(slug: $slug){
                                name
                                state
			                    events {
				                    id
				                    name
			                    }
		                    }
	                    }",
                    Variables = new
                    {
                        slug = slug
                    }
                };

                var response = await client.SendQueryAsync(request, () => new { Tournament = new Tournament() });

                if (response.Data.Tournament == null)
                {
                    DisplayMessage("Tournament not found. Check the url and try again", "x", "ok");
                    return;
                }

                if (response.Data.Tournament.events != null)
                {
                    if (response.Data.Tournament.state < 1 && response.Data.Tournament.state > 4 || response.Data.Tournament.state == 3)
                    {
                        Window message = DisplayMessage(response.Data.Tournament.name + " is unavailable or already over. Please check your tournament information.", "info", "ok");
                        message.ShowDialog();
                        return;
                    }

                    localEvents.Clear();
                    cbEvent.Items.Clear();

                    Title = "Stream Scoreboard - " + response.Data.Tournament.name;

                    foreach (Event ev in response.Data.Tournament.events)
                    {
                        localEvents.Add(ev);
                        cbEvent.Items.Add(ev.name);
                    }

                    gridStart.Visibility = Visibility.Hidden;
                    tabControl.Visibility = Visibility.Visible;
                    gridSmashGG.Visibility = Visibility.Visible;

                    AddTournamentToFile(slug, response.Data.Tournament.name);

                    if (localEvents.Count > 1)
                    {
                        localEvents.Insert(0, null);
                        cbEvent.Items.Insert(0, "Select an event...");
                        cbEvent.IsEnabled = true;
                    }

                    cbEvent.SelectedIndex = 0;
                    tbSearch.Text = "";
                    DoShowUrl();

                    return;
                }

                DisplayMessage("An error ocurred while getting the tournament events. Please try again", "x", "ok");
            }
            catch (Exception)
            {
            }
        }

        private async void LoadEvent()
        {
            tabControl.IsEnabled = false;
            if (cbEvent.SelectedIndex >= 0 && !cbEvent.SelectedValue.ToString().Contains("..."))
            {

                if (localEvents.Count > 1 && cbEvent.Items[0].ToString().Contains("..."))
                {
                    cbEvent.Items.Remove(cbEvent.Items[0]);
                    localEvents.RemoveAt(0);
                }

                try
                {
                    GraphQLRequest request = new GraphQLRequest
                    {
                        Query = @"
                            query EventQuery($id: ID!) {
		                        event(id: $id){
                                    id
                                    name
    	                            phases {
      	                                id
      	                                name
    	                            }
		                        }
	                        }",
                        Variables = new
                        {
                            id = int.Parse(localEvents[cbEvent.SelectedIndex].id)
                        }
                    };

                    var response = await client.SendQueryAsync(request, () => new { Event = new Event() });

                    if (response.Data.Event.phases != null)
                    {
                        ClearSelectiveFields("P");

                        foreach (Phase p in response.Data.Event.phases)
                        {
                            localPhases.Add(p);
                            cbPhase.Items.Add(p.name);
                        }

                        if (localPhases.Count > 1)
                        {
                            localPhases.Insert(0, null);
                            cbPhase.Items.Insert(0, "Select a phase...");
                            cbPhase.IsEnabled = true;
                        }

                        cbPhase.SelectedIndex = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            tabControl.IsEnabled = true;
        }

        private async void CbPhase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabControl.IsEnabled = false;
            if (cbPhase.SelectedIndex >= 0 && !cbPhase.SelectedValue.ToString().Contains("..."))
            {
                if (localPhases.Count > 1 && cbPhase.Items[0].ToString().Contains("..."))
                {
                    cbPhase.Items.Remove(cbPhase.Items[0]);
                    localPhases.RemoveAt(0);
                }

                try
                {
                    GraphQLRequest request = new GraphQLRequest
                    {
                        Query = @"
                            query PhaseQuery($id: ID) {
		                        phase(id: $id){
                                    id
                                    name
    	                            phaseGroups {
      	                                nodes {
                                          id
                                          displayIdentifier
                                        }
    	                            }
		                        }
                            }",
                        Variables = new
                        {
                            id = int.Parse(localPhases[cbPhase.SelectedIndex].id)
                        }
                    };

                    var response = await client.SendQueryAsync(request, () => new { Phase = new Phase() });

                    if (response.Data.Phase.phaseGroups.nodes != null)
                    {
                        ClearSelectiveFields("G");

                        foreach (PhaseGroup pg in response.Data.Phase.phaseGroups.nodes)
                        {
                            localGroups.Add(pg);
                            cbPhaseGroup.Items.Add("Pool " + pg.displayIdentifier);
                        }

                        if (localGroups.Count > 1)
                        {
                            localGroups.Insert(0, null);
                            cbPhaseGroup.Items.Insert(0, "Select a group...");
                            cbPhaseGroup.IsEnabled = true;
                        }

                        cbPhaseGroup.SelectedIndex = 0;
                    }
                }
                catch (Exception)
                {
                }
            }
            tabControl.IsEnabled = true;
        }

        private void CbPhaseGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSets();
        }

        private void CbSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSet.SelectedIndex >= 0 && !cbSet.SelectedValue.ToString().Contains("..."))
            {
                if (localSets.Count > 1 && cbSet.Items[0].ToString().Contains("..."))
                {
                    cbSet.Items.Remove(cbSet.Items[0]);
                    localSets.RemoveAt(0);
                }

                cbRound.Items.Clear();
                tbRoundNum.Text = "";

                string[] setText = cbSet.SelectedValue.ToString().Split(new string[] { " vs " }, StringSplitOptions.None);
                if (!isDoubles)
                {
                    tbPlayer1.Text = setText[0];
                    tbPlayer2.Text = setText[1];

                }
                else
                {
                    string[] team1 = setText[0].Split(new string[] { " / " }, StringSplitOptions.None);
                    string[] team2 = setText[1].Split(new string[] { " / " }, StringSplitOptions.None);

                    tbPlayer1.Text = team1[0];
                    tbPlayer2.Text = team2[0];
                    tbPlayer3.Text = team1[1];
                    tbPlayer4.Text = team2[1];
                }

                string round = localSets[cbSet.SelectedIndex].fullRoundText;

                string[] roundWords = round.Split(' ');

                int threshold = roundWords.Length;

                if (int.TryParse(roundWords[roundWords.Length - 1], out int value))
                {
                    tbRoundNum.Text = value.ToString();
                    threshold--;
                }

                string output = "";

                for (int i = 0; i < threshold; i++)
                {
                    string s = roundWords[i];

                    if (s.Contains("-Final"))
                        output += s.Replace("-Final", "s");
                    else if (s.Contains("Final"))
                        output += s + "s";
                    else
                        output += s;

                    if (i < threshold - 1)
                        output += " ";
                }

                cbRound.Items.Add(output);
                cbRound.SelectedIndex = 0;
            }
        }

        private async void LoadSets()
        {
            tabControl.IsEnabled = false;
            if (cbPhaseGroup.SelectedIndex >= 0 && !cbPhaseGroup.SelectedValue.ToString().Contains("..."))
            {
                if (localGroups.Count > 1 && cbPhaseGroup.Items[0].ToString().Contains("..."))
                {
                    cbPhaseGroup.Items.Remove(cbPhaseGroup.Items[0]);
                    localGroups.RemoveAt(0);
                }

                try
                {
                    GraphQLRequest request = new GraphQLRequest
                    {
                        Query = @"
                            query SetQuery($id: ID) {
		                        phaseGroup(id: $id){
                                    id
                                    displayIdentifier
    	                            sets (perPage: 500, filters: {state: [1, 2, 4, 6, 7], hideEmpty: true }, sortType: CALL_ORDER){
                                        nodes {
                                            id
                                            fullRoundText
                                            slots {
                                                entrant {
                                                    name
                                                }
                                            }
                                        }
                                    }
		                        }
                            }",
                        Variables = new
                        {
                            id = int.Parse(localGroups[cbPhaseGroup.SelectedIndex].id)
                        }
                    };

                    var response = await client.SendQueryAsync(request, () => new { PhaseGroup = new PhaseGroup() });

                    List<Set> sets = response.Data.PhaseGroup.sets.nodes;

                    if (sets != null)
                    {
                        ClearSelectiveFields("S");

                        foreach (Set s in sets)
                        {
                            if (s.slots[0].entrant != null && s.slots[1].entrant != null)
                            {
                                localSets.Add(s);
                                cbSet.Items.Add(s.slots[0].entrant.name + " vs " + s.slots[1].entrant.name);
                            }
                        }

                        if (localSets.Count > 1)
                        {
                            localSets.Insert(0, null);
                            cbSet.Items.Insert(0, "Select a set...");
                            cbSet.IsEnabled = true;
                        }

                        cbSet.SelectedIndex = 0;

                        btnRefresh.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception)
                {
                }
            }
            tabControl.IsEnabled = true;
        }

        private void ClearSelectiveFields(string fieldsToChange)
        {
            cbRound.Items.Clear();
            tbRoundNum.Text = "";
            tbPlayer1.Text = "";
            tbPlayer2.Text = "";
            tbPlayer3.Text = "";
            tbPlayer4.Text = "";
            btnRefresh.Visibility = Visibility.Hidden;

            if (fieldsToChange.Contains("P"))
            {
                cbPhase.Items.Clear();
                cbPhaseGroup.Items.Clear();
                cbSet.Items.Clear();

                localPhases.Clear();
                localGroups.Clear();
                localSets.Clear();

                cbPhase.IsEnabled = false;
                cbPhaseGroup.IsEnabled = false;
                cbSet.IsEnabled = false;

                return;
            }

            if (fieldsToChange.Contains("G"))
            {
                cbPhaseGroup.Items.Clear();
                cbSet.Items.Clear();

                localGroups.Clear();
                localSets.Clear();

                cbPhaseGroup.IsEnabled = false;
                cbSet.IsEnabled = false;

                return;
            }

            if (fieldsToChange.Contains("S"))
            {
                cbSet.Items.Clear();

                localSets.Clear();

                cbSet.IsEnabled = false;

                return;
            }
        }

        private void SaveCommand(object sender, RoutedEventArgs e)
        {
            SaveCommand(tbCommandName.Text, tbCommandValue.Text, isEditing);
            tbCommandName.Text = "";
            tbCommandValue.Text = "";
            btnSaveCommand.Content = "Add";
        }

        private void tbCommandValue_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;

            string keyString;

            if (IsAcceptedKey(e.Key))
            {
                string keyValue = e.Key.ToString();

                if (keyValue.ToLower().Contains("system"))
                    keyString = "Alt";
                else if (keyValue.ToLower().Contains("ctrl"))
                    keyString = "Control";
                else if (keyValue.ToLower().Contains("shift"))
                    keyString = "Shift";
                else if (int.TryParse(keyValue.Replace("D", string.Empty), out int value))
                {
                    keyString = value.ToString();
                }
                else
                    keyString = e.Key.ToString();


                if (!ListContainsPrimaryKey())
                {
                    if (IsModifier(keyString))
                    {
                        if (keys.IndexOf(keyString) < 0)
                            keys.Add(keyString);
                    }
                    else
                    {
                        keys.Add(keyString);
                    }

                    UpdateCommandValue();
                }

            }
        }

        private void tbCommandValue_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!IsAcceptedKey(e.Key))

                e.Handled = true;

            else if (e.Key == Key.Back)
            {
                e.Handled = true;

                if (keys.Count > 0)
                    keys.Remove(keys[keys.Count - 1]);

                UpdateCommandValue();
            }
        }

        private bool IsAcceptedKey(Key key)
        {
            int code = (int)key;
            return (code >= 34 && code <= 69) || (code >= 90 && code <= 101)
                || (code >= 116 && code <= 119) || code == 156 || code == 2;
        }

        private bool IsModifier(string key)
        {
            return key == "Alt" || key == "Shift" || key == "Control";
        }

        private bool ListContainsPrimaryKey()
        {
            foreach (string key in keys)
            {
                if (key != "Control" && key != "Alt" && key != "Shift")
                    return true;
            }

            return false;
        }

        private void UpdateCommandValue()
        {
            tbCommandValue.Text = "";

            foreach (string key in keys)
            {
                tbCommandValue.Text += key;

                if (keys.IndexOf(key) != keys.Count - 1)
                    tbCommandValue.Text += " + ";
            }

            tbCommandValue.SelectionStart = tbCommandValue.Text.Length;
        }

        private void tbCommandName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTextboxes();
        }

        private void tbCommandValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTextboxes();
        }

        private void CheckTextboxes()
        {
            bool hasName = !string.IsNullOrEmpty(tbCommandName.Text) && !string.IsNullOrWhiteSpace(tbCommandName.Text);
            bool hasValue = !string.IsNullOrEmpty(tbCommandValue.Text) && !string.IsNullOrWhiteSpace(tbCommandValue.Text);

            btnSaveCommand.IsEnabled = hasName && hasValue;
        }

        private void EditCommandValues(string name, string value)
        {
            tbCommandName.Text = name;
            tbCommandValue.Text = value;

            char[] separators = { ' ', '+' };
            string[] values = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string k in values)
            {
                keys.Add(k);
            }

            btnSaveCommand.Content = "Update";
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            tabControl.IsEnabled = false;
            LoadSets();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void LoadTournamentFile()
        {
            string tournamentFile = "tournaments.json";

            if (File.Exists(tournamentFile))
            {
                string tournamentsString = File.ReadAllText(tournamentFile);
                TournamentItem[] tournamentItems = JsonConvert.DeserializeObject<TournamentItem[]>(tournamentsString);

                localTournaments.Clear();
                cbTournaments.Items.Clear();

                for (int i = 0; i < tournamentItems.Length; i++)
                {
                    TournamentItem t = tournamentItems[i];

                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = t.name;
                    item.Tag = i + 1;
                    localTournaments.Add(t);
                    cbTournaments.Items.Add(item);
                }

            }

            ComboBoxItem firstItem = new ComboBoxItem();

            if (cbTournaments.Items.Count > 0 && !cbTournaments.Items[0].ToString().Contains("recent"))
            {
                firstItem.Content = "Select a tournament...";
                firstItem.Tag = "0";
                cbTournaments.Items.Insert(0, firstItem);
                cbTournaments.IsEnabled = true;
            }
            else
            {
                firstItem.Content = "No recent tournaments";
                firstItem.Tag = "0";
                cbTournaments.Items.Add(firstItem);
            }

            localTournaments.Insert(0, null);
        }

        private void AddTournamentToFile(string slug, string name)
        {
            TournamentItem t = new TournamentItem()
            {
                name = name,
                slug = slug
            };

            if (localTournaments.Count == 1)
            {
                cbTournaments.Items.RemoveAt(0);
                cbTournaments.Items.Add("Select a tournament...");
            }

            int index = TournamentExists(slug);

            if (index < 1)
            {
                localTournaments.Insert(1, t);
                cbTournaments.Items.Insert(1, t.name);
            }
            else
            {
                localTournaments.RemoveAt(index);
                localTournaments.Insert(1, t);

                cbTournaments.Items.RemoveAt(index);
                cbTournaments.Items.Insert(1, t.name);
            }

            TournamentItem[] actualTournaments = new ArraySegment<TournamentItem>(localTournaments.ToArray(), 1, localTournaments.Count - 1).ToArray();
            string json = JsonConvert.SerializeObject(actualTournaments);
            File.WriteAllText("tournaments.json", json);
        }

        private void cbTournaments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTournaments.SelectedIndex >= 1)
            {
                tbSearch.Text = "";
            }

            btnSubmitUrl.IsEnabled = true;
        }

        private int TournamentExists(string slug)
        {
            for (int i = 1; i < localTournaments.Count; i++)
            {
                TournamentItem t = localTournaments[i];
                if (t != null && t.slug == slug)
                    return i;
            }

            return -1;
        }

        private void DeleteTournamentButton_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject parent = FindParent(sender as DependencyObject);

            int index = int.Parse(((ComboBoxItem)parent).Tag.ToString());

            Window message = DisplayMessage("Delete tournament \"" + ((ComboBoxItem)cbTournaments.Items[index]).Content + "\"?", "check", "yesno");
            bool dialogResult = (bool)message.ShowDialog();

            if (dialogResult)
            {

                if (cbTournaments.SelectedIndex == index)
                {
                    cbTournaments.SelectedIndex = 0;
                    tbSearch.Text = "";
                }

                cbTournaments.Items.RemoveAt(index);
                localTournaments.RemoveAt(index);

                UpdateTournamentTags();

                if (localTournaments.Count < 2)
                {
                    cbTournaments.Items[0] = "No recent tournaments";
                    cbTournaments.SelectedIndex = 0;
                    cbTournaments.IsEnabled = false;
                }

                TournamentItem[] actualTournaments = new ArraySegment<TournamentItem>(localTournaments.ToArray(), 1, localTournaments.Count - 1).ToArray();
                string json = JsonConvert.SerializeObject(actualTournaments);
                File.WriteAllText("tournaments.json", json);
            }
        }

        private void UpdateTournamentTags()
        {
            for (int i = 1; i < cbTournaments.Items.Count; i++)
                ((ComboBoxItem)cbTournaments.Items[i]).Tag = i;
        }

        private DependencyObject FindParent(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent.GetType() == typeof(ComboBoxItem))
                return parent;
            else
                return FindParent(parent);
        }

        private void chkLosers1_Checked(object sender, RoutedEventArgs e)
        {
            CheckSaveState();
        }

        private void chkLosers1_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckSaveState();
        }

        private void chkLosers2_Checked(object sender, RoutedEventArgs e)
        {
            CheckSaveState();
        }

        private void chkLosers2_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckSaveState();
        }

        private Window DisplayMessage(string text, string messageType, string windowType)
        {
            Message message = new Message(text, messageType, windowType);
            message.Owner = this;

            return message;
        }

        private void initializeThemes()
        {
            string[] themes = Directory.GetFiles(@"../../Skins/");
            foreach (string t in themes)
            {
                cbThemes.Items.Add(Path.GetFileName(t).Replace(".xaml", ""));
            }
        }

        private void tbOnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int result;
            e.Handled = !int.TryParse(e.Text, out result);
        }

        private void tbOnlyNumbers_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        private void tbOnlyNumbers_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            int result;
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!int.TryParse(text, out result))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void checkConnectionInfo()
        {
            btnObsConnection.IsEnabled = !string.IsNullOrEmpty(tbIp1.Text) && !string.IsNullOrWhiteSpace(tbIp1.Text) &&
                                            !string.IsNullOrEmpty(tbIp2.Text) && !string.IsNullOrWhiteSpace(tbIp2.Text) &&
                                            !string.IsNullOrEmpty(tbIp3.Text) && !string.IsNullOrWhiteSpace(tbIp3.Text) &&
                                            !string.IsNullOrEmpty(tbIp4.Text) && !string.IsNullOrWhiteSpace(tbIp4.Text) &&
                                            !string.IsNullOrEmpty(tbPort.Text) && !string.IsNullOrWhiteSpace(tbPort.Text) &&
                                            !string.IsNullOrEmpty(pbPassword.Password) && !string.IsNullOrWhiteSpace(pbPassword.Password);
        }

        private void tbIp_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkConnectionInfo();
        }

        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            checkConnectionInfo();
        }

        private void Window_TouchMove(object sender, TouchEventArgs e)
        {
            listTournaments.IsDropDownOpen = false;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            listTournaments.IsDropDownOpen = false;

        }

        private void listTournaments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listTournaments.Items.Count > 0)
            {
                search = false;
                tbSearch.Text = listTournaments.SelectedValue.ToString();

                btnSubmitUrl.IsEnabled = true;
                listTournaments.ItemsSource = null;
                search = true;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            listTournaments.IsDropDownOpen = false;
        }
    }
}
