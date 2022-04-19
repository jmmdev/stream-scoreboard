using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQL.Client.Http;
using GraphQL.Client.Abstractions;
using System.Net.Http.Headers;
using GraphQL;

namespace Scoreboard
{
    public partial class MainWindow : Window
    {
        private const int CB_SETCUEBANNER = 0x1703;

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lParam);

        GraphQLHttpClient client = new GraphQLHttpClient("https://api.smash.gg/gql/alpha", new NewtonsoftJsonSerializer());
        const string token = "ceffaeed94948ae90d5c11287386b979";

        List<Event> localEvents = new List<Event>();
        List<Phase> localPhases = new List<Phase>();
        List<PhaseGroup> localGroups = new List<PhaseGroup>();
        List<Set> localSets = new List<Set>();

        protected OBSWebsocket obs;

        string resourcesDir;
        string outputDir;

        bool isDoubles = false;
        bool isSmashgg = false;
        bool roundNum = true;
        bool obsIsConnected = false;

        readonly string url = "ws://127.0.0.1:";
        string port;
        string password;

        List<Command> commands = new List<Command>();

        bool showUrl = false;

        public MainWindow()
        {
            InitializeComponent();

            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            LoadSettings();
            LoadCommands();

            EnableUpdateButton(btnSaveSettings, false);

            obs = new OBSWebsocket();
            obs.Connected += OnConnect;
            obs.Disconnected += OnDisconnect;

            tbUrl.Text = "ultima-stock-iii";
        }

        public void ClearFields()
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

        public void InitializeManual()
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

            cbRound.SelectedIndex = 0;

            gridSmashGG.Visibility = Visibility.Hidden;

            Thickness margin = roundGrid.Margin;
            margin.Top = 8;
            roundGrid.Margin = margin;
        }

        public void InitializeSmashGG()
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

            string slug = tbUrl.Text;

            System.Windows.Forms.Application.UseWaitCursor = true;
            LoadTournament(slug);
        }

        public void TbUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnSubmitUrl.IsEnabled = !string.IsNullOrEmpty(tbUrl.Text) && !string.IsNullOrWhiteSpace(tbUrl.Text);
        }

        public void BtnSubmitUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbUrl.Text) && !string.IsNullOrWhiteSpace(tbUrl.Text))
            {
                btnSubmitUrl.IsEnabled = false;
                InitializeSmashGG();
            }
        }

        private void CbEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbEvent.SelectedIndex >= 0 && !cbEvent.SelectedValue.ToString().Contains("..."))
            {
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

                cbRound.Margin = roundMargin;

                CheckSaveState();
            }
        }

        private void P1ScoreUp(object sender, RoutedEventArgs e)
        {
            int score1 = int.Parse(lbScore1.Content.ToString());

            if (score1 < 3)
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

            if (score2 < 3)
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

            string auxScore = lbScore1.Content.ToString();
            lbScore1.Content = lbScore2.Content.ToString();
            lbScore2.Content = auxScore;
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

            File.WriteAllText(outputDir + "player1.txt", p1);
            File.WriteAllText(outputDir + "player2.txt", p2);
            File.WriteAllText(outputDir + "match.txt", p1 + " vs " + p2);

            string score1 = lbScore1.Content.ToString();
            string score2 = lbScore2.Content.ToString();

            File.WriteAllText(outputDir + "score1.txt", score1);
            File.WriteAllText(outputDir + "score2.txt", score2);

            if (!string.IsNullOrEmpty(resourcesDir))
            {
                string fileToWrite1 = resourcesDir + score1 + ".png";

                if (File.Exists(fileToWrite1))
                {
                    string score1File = outputDir + "score1.png";

                    if (File.Exists(score1File))

                        File.Delete(score1File);

                    File.Copy(fileToWrite1, score1File);
                }

                string fileToWrite2 = resourcesDir + score2 + ".png";

                if (File.Exists(fileToWrite2))
                {
                    string score2File = outputDir + "score2.png";

                    if (File.Exists(score2File))

                        File.Delete(score2File);

                    File.Copy(fileToWrite2, score2File);
                }
            }

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

        private void BrowseResources(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();

            if (!string.IsNullOrEmpty(folder.SelectedPath))
            {
                string path = folder.SelectedPath;

                tbResourcesFolder.Text = path;
            }
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

                string resourcesPath = settings.resourcesPath;
                string outputPath = settings.outputPath;

                tbResourcesFolder.Text = resourcesPath;
                tbOutputFolder.Text = outputPath;

                resourcesDir = resourcesPath + "\\";
                outputDir = outputPath + "\\";
            }
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            Settings newSettings = new Settings
            {
                outputPath = tbOutputFolder.Text,
                resourcesPath = tbResourcesFolder.Text
            };

            string json = JsonConvert.SerializeObject(newSettings);
            File.WriteAllText("settings.json", json);

            resourcesDir = newSettings.resourcesPath + "\\";
            outputDir = newSettings.outputPath + "\\";

            lbSaveMessage.Visibility = Visibility.Visible;
            EnableUpdateButton(btnSaveSettings, false);
        }

        private void SettingsTextChanged(object sender, TextChangedEventArgs e)
        {
            EnableUpdateButton(btnSaveSettings, true);
            lbSaveMessage.Visibility = Visibility.Hidden;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbSaveMessage.Visibility = Visibility.Hidden;
        }

        private void EnableBtnApply(object sender, EventArgs e)
        {
            btnApplyCommand.IsEnabled = true;
        }

        private void BtnObsConnection_Click(object sender, RoutedEventArgs e)
        {
            tabControl.IsEnabled = false;
            ObsConnect();
        }

        private void ObsConnect()
        {
            port = tbPort.Text;
            password = pbPassword.Password;

            try
            {
                if (!obsIsConnected)
                    obs.Connect(url + port, password);
                else
                    obs.Disconnect();

                tabControl.IsEnabled = true;
                obsIsConnected = !obsIsConnected;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                btnObsConnection.IsEnabled = true;
            }
        }

        private void OnConnect(object sender, EventArgs e)
        {
            gridConnection.IsEnabled = false;
            gridOptions.IsEnabled = true;
            btnObsConnection.IsDefault = false;

            SuccessfulConnection();
            btnObsConnection.Content = "Disconnect";
        }

        private void OnDisconnect(object sender, EventArgs e)
        {
            gridConnection.IsEnabled = true;
            gridOptions.IsEnabled = false;
            btnObsConnection.IsDefault = true;

            lblConnection.Content = "Disconnected";
            lblConnection.Foreground = Brushes.Red;

            btnObsConnection.Content = "Connect";
        }

        private void SuccessfulConnection()
        {
            lblConnection.Content = "Connected";
            lblConnection.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xff, 0x00));
        }

        private void ShowNewCommand(object sender, RoutedEventArgs e)
        {
            PromptCommandWindow(false);
        }

        private void EditCommand(object sender, RoutedEventArgs e)
        {
            PromptCommandWindow(true);
        }

        private void PromptCommandWindow(bool edit)
        {
            NewCommand window = new NewCommand();
            window.Show();
            window.Activate();
            window.Focus();
            window.Topmost = true;
            window.Closed += RefocusMain;

            if (edit)
            {
                Command c = commands[listCommand.SelectedIndex];
                window.isEditing = true;
                window.Title = "Edit command";
                window.EditCommand(c.name, c.value);
            }

            IsEnabled = false;
        }

        public void RefocusMain(object sender, EventArgs e)
        {
            IsEnabled = true;
        }

        public void SaveCommand(string name, string value, bool isEditing)
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
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Delete command \"" + listCommand.SelectedValue + "\"?", "Deleting command", MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
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

                    obs.SendRequest("TriggerHotkeyBySequence", o);
                }
            }
        }

        private void ListCommand_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listCommand.SelectedItem != null)
                PromptCommandWindow(true);
        }


        private void TbInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            HighlightInfoButton(true);
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
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Close this program?", "Confirm", MessageBoxButtons.YesNo);

            e.Cancel = dialogResult == System.Windows.Forms.DialogResult.No;
        }

        private void EnableUpdateButton(System.Windows.Controls.Button target, bool enabled)
        {
            if (enabled)
            {
                target.Background = Brushes.Red;
                target.Foreground = Brushes.White;
            }
            else
            {
                target.Background = default;
                target.ClearValue(ForegroundProperty);
            }

            target.IsEnabled = enabled;
        }

        private void HighlightInfoButton(bool highlight)
        {
            if (highlight)
            {
                btnSaveInfo.Background = Brushes.Red;
                btnSaveInfo.Foreground = Brushes.White;
            }
            else
            {
                btnSaveInfo.Background = default;
                btnSaveInfo.ClearValue(ForegroundProperty);
            }
        }

        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            InitializeManual();
            gridStart.Visibility = Visibility.Hidden;
            tabControl.Visibility = Visibility.Visible;
        }

        private void BtnSmashGG_Click(object sender, RoutedEventArgs e)
        {
            doShowUrl();
        }

        private void doShowUrl()
        {
            showUrl = !showUrl;
            Thickness margin = gridStart.Margin;

            if (showUrl)
            {
                gridStart.Height = 172;
                margin.Top = 30;
                gridUrl.Visibility = Visibility.Visible;
                btnSmashGG.Content = Resources["smashGGButtonCancel"];
            }
            else
            {
                gridStart.Height = 32;
                margin.Top = 0;
                gridUrl.Visibility = Visibility.Hidden;
                btnSmashGG.Content = Resources["smashGGButtonConnect"];
            }

            gridStart.Margin = margin;

            margin = roundGrid.Margin;
            margin.Top = 56;
            roundGrid.Margin = margin;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Go back to main menu?", "Exit", MessageBoxButtons.YesNo);

            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                tabControl.Visibility = Visibility.Hidden;
                gridStart.Visibility = Visibility.Visible;
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

                if (response.Data.Tournament.events != null)
                {
                    localEvents.Clear();
                    cbEvent.Items.Clear();

                    foreach (Event ev in response.Data.Tournament.events)
                    {
                        localEvents.Add(ev);
                        cbEvent.Items.Add(ev.name);
                    }

                    gridStart.Visibility = Visibility.Hidden;
                    tabControl.Visibility = Visibility.Visible;
                    gridSmashGG.Visibility = Visibility.Visible;

                    if (localEvents.Count > 1)
                    {
                        localEvents.Insert(0, null);
                        cbEvent.Items.Insert(0, "Select an event...");
                    }

                    cbEvent.IsEnabled = true;
                    cbEvent.SelectedIndex = 0;

                    return;
                }

                DisplayErrorMessage();
            }
            catch (Exception)
            {
                DisplayErrorMessage();
            }
            finally
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Cross;
                btnSubmitUrl.IsEnabled = true;
                doShowUrl();
            }
        }

        private async void LoadEvent()
        {
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

        public async void CbPhase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPhase.SelectedIndex >= 0 && !cbPhase.SelectedValue.ToString().Contains("..."))
            {
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
        }

        public async void CbPhaseGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPhaseGroup.SelectedIndex >= 0 && !cbPhaseGroup.SelectedValue.ToString().Contains("..."))
            {
                try
                {
                    GraphQLRequest request = new GraphQLRequest
                    {
                        Query = @"
                            query SetQuery($id: ID) {
		                        phaseGroup(id: $id){
                                    id
                                    displayIdentifier
    	                            sets (perPage: 500, filters: {state: [1, 2, 3, 4, 6, 7], hideEmpty: true }, sortType: CALL_ORDER){
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
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public void CbSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSet.SelectedIndex >= 0 && !cbSet.SelectedValue.ToString().Contains("..."))
            {
                cbRound.Items.Clear();
                tbRoundNum.Text = "";

                string[] setText = cbSet.SelectedValue.ToString().Split(new string[] { " vs " }, StringSplitOptions.None);
                if (!isDoubles)
                {
                    tbPlayer1.Text = setText[0];
                    tbPlayer2.Text = setText[1];

                }

                string round = localSets[cbSet.SelectedIndex].fullRoundText;

                string[] roundWords = round.Split(' ');

                int threshold = roundWords.Length;

                if (int.TryParse(roundWords[roundWords.Length-1], out int value))
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

        private void ClearSelectiveFields(string fieldsToChange)
        {
            cbRound.Items.Clear();
            tbRoundNum.Text = "";
            tbPlayer1.Text = "";
            tbPlayer2.Text = "";
            tbPlayer3.Text = "";
            tbPlayer4.Text = "";

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

        private void DisplayErrorMessage()
        {

        }
    }
}
