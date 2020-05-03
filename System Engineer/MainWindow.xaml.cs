using System;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;


namespace System_Engineer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow: Window
	{
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
	    private NetworkInterface[] nicArr;

	    public MainWindow()
		{
			InitializeComponent(); 
			LoadDefault();
			clock();
			InitializeNetworkInterface();
            InitializeTimer();
			
            homeTab.Visibility = System.Windows.Visibility.Visible;
            optimizerTab.Visibility = System.Windows.Visibility.Hidden;
            sequrityTab.Visibility = System.Windows.Visibility.Hidden;
            networkTab.Visibility = System.Windows.Visibility.Hidden;
            customizationTab.Visibility = System.Windows.Visibility.Hidden;
            systemToolsTab.Visibility = System.Windows.Visibility.Hidden;
            helpTab.Visibility = System.Windows.Visibility.Hidden;			
			
			
			//System Information
			textCPU.Text = SystemInfo.ProcessorInfo();
			textRAM.Text=SystemInfo.Ram();
			textRamManufacturer.Text= SystemInfo.MemoryInfo();
			textHDD.Text=SystemInfo.HDDInfo();
			textGraphics.Text=SystemInfo.VideoInfo();
			textMotherboard.Text=SystemInfo.MotherboardInfo();
			textBIOS.Text=SystemInfo.BIOSInfo();
			textOS.Text=SystemInfo.OSInfo();
			textSystemMenufacturer.Text=SystemInfo.ProductBrand();
			
		}
 		 
		private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        	 CheckInstance();
        }

		//only one interface running
		private static void CheckInstance()
		{
			    Process[] thisnameprocesslist;
			    string modulename, processname;
			    Process p = Process.GetCurrentProcess();
			    modulename = p.MainModule.ModuleName.ToString();
			    processname = System.IO.Path.GetFileNameWithoutExtension(modulename);
			    thisnameprocesslist = Process.GetProcessesByName(processname);
			    if (thisnameprocesslist.Length > 1)
			    {
				    MessageBox.Show("Application is already running.", "System Mechanics", MessageBoxButton.OK, MessageBoxImage.Stop);
				    Application.Current.Shutdown();
			    }
		}
		
		public void RestartExplorer()
		{
		try
			{
			foreach(System.Diagnostics.Process myProcess in System.Diagnostics.Process.GetProcessesByName("explorer"))
				{
					myProcess.Kill();
					myProcess.Start();
				}
			}
			catch
			{
			}
		}
		
		
		//clear temp, prefetch, temporary file etc.
		public void QuickClean()
		{
			try
			{
			String tempFolder = Environment.ExpandEnvironmentVariables("%TEMP%");    
            String prefetch = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%") + "\\Prefetch";
            String temp = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%") + "\\Temp";
			String recent = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + "\\Recent";
			String cookies = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + "\\cookies";
			String history = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + "\\Local Settings\\History";
			String tempInternetFile  = Environment.ExpandEnvironmentVariables("%USERPROFILE%") + "\\Local Settings\\Temporary Internet Files";	
            String dllchache = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%") + "\\system32\\dllcache";
			EmptyFolderContents(tempFolder);
            EmptyFolderContents(temp);
            EmptyFolderContents(prefetch);
            EmptyFolderContents(recent);
			EmptyFolderContents(cookies);
			EmptyFolderContents(history);
			EmptyFolderContents(tempInternetFile);
			EmptyFolderContents(dllchache);
			}
			catch
			{
    			
			}
		}
		
		private void EmptyFolderContents(string folderName)
        {
            foreach (var folder in Directory.GetDirectories(folderName))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception excep)
                {
                    System.Diagnostics.Debug.WriteLine(excep);
                }
            }
            foreach (var file in Directory.GetFiles(folderName))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception excep)
                {
                    System.Diagnostics.Debug.WriteLine(excep);
                }
            }
        }
		
		// Clock 
		public void clock()
		{

			    DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
			    {
				    this.lableClock.Content = DateTime.Now.ToString("hh:mm:ss");
				    this.ampm.Text = DateTime.Now.ToString("tt");	//("HH:mm:ss tt"); //for AM PM HH=24 hours
				    this.dateLabel.Text = DateTime.Now.ToLongDateString();
			    }, this.Dispatcher);
		}
		
        
		private void InitializeTimer()
        {
            var timer1 = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            timer1.Tick += new EventHandler(timer_Tick);
            timer1.Start();
        }		
		 
		private void InitializeNetworkInterface()
        {
            // Grab all local interfaces to this computer
            nicArr = NetworkInterface.GetAllNetworkInterfaces();

            // Add each interface name to the combo box
            for (int i = 0; i < nicArr.Length; i++)
                cmbInterface.Items.Add(nicArr[i].Name);

            // Change the initial selection to the first interface
            cmbInterface.SelectedIndex = 0;
        }
		 
		private void UpdateNetworkInterface()
        {
           try
			{
			// Grab NetworkInterface object that describes the current interface
            NetworkInterface nic = nicArr[cmbInterface.SelectedIndex];

            // Grab the stats for that interface
            IPv4InterfaceStatistics interfaceStats = nic.GetIPv4Statistics();

            // Calculate the speed of bytes going in and out
            
            var bytesSentSpeed = (int)((interfaceStats.BytesSent - double.Parse(lblBytesSent.Text)) / 1024);
            var bytesReceivedSpeed = (int)((interfaceStats.BytesReceived - double.Parse(lblBytesReceived.Text)) / 1024);

            // Update the labels
            
            lblInterfaceType.Text = nic.NetworkInterfaceType.ToString();
			lblBytesSent.Text = interfaceStats.BytesSent.ToString();
			lblBytesReceived.Text = interfaceStats.BytesReceived.ToString();
            lblSpeed.Text = nic.Speed.ToString();
            lblDownload.Text = bytesReceivedSpeed.ToString() + " KB/s";            
            lblUpload.Text = bytesSentSpeed.ToString() + " KB/s";
            
			}
			catch
			{
			}

        }
		
		 void timer_Tick(object sender, EventArgs e)
        {
            UpdateNetworkInterface();
        }
		
		private void exitButtom_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}

		private void maximizeButtom_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			//this.WindowState = System.Windows.WindowState.Maximized;
		}

		private void minimizeButtom_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.WindowState = System.Windows.WindowState.Minimized;
		}

		private void backgroundRect_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.DragMove();
		}

          private void facebook_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("http://www.facebook.com/pages/Software-Art/124544400963976");
			}
			catch
			{
    			MessageBox.Show("Sorry, application error.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void software_art_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("http:\\www.software-art.somee.com");//("Control.exe","/name Microsoft.ActionCenter");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        
        //Load default settings
         public void LoadDefault() 
         {
            //Network Tweak default settings
			this._nt_b.IsChecked = Properties.Settings.Default.nt_b;
            this._nt_b_Copy.IsChecked = Properties.Settings.Default.nt_b_Copy;
			this._nt_b_Copy1.IsChecked = Properties.Settings.Default.nt_b_Copy1;
			this._nt_b_Copy2.IsChecked = Properties.Settings.Default.nt_b_Copy2;
			this._nt_b_Copy3.IsChecked = Properties.Settings.Default.nt_b_Copy3;
			this._nt_b_Copy4.IsChecked = Properties.Settings.Default.nt_b_Copy4;
			this._nt_b_Copy5.IsChecked = Properties.Settings.Default.nt_b_Copy5;
			this._nt_b_Copy6.IsChecked = Properties.Settings.Default.nt_b_Copy6;
			this._nt_b_Copy7.IsChecked = Properties.Settings.Default.nt_b_Copy7;
			
			
			//Performances Tweak Default settings
			this._pt_b1.IsChecked = Properties.Settings.Default.pt_b1;
			this._pt_b2.IsChecked = Properties.Settings.Default.pt_b2;
			this._pt_b3.IsChecked = Properties.Settings.Default.pt_b3;
			this._pt_b4.IsChecked = Properties.Settings.Default.pt_b4;
			this._pt_b5.IsChecked = Properties.Settings.Default.pt_b5;
			this._pt_b6.IsChecked = Properties.Settings.Default.pt_b6;
			
         	this._pt_b.IsChecked = Properties.Settings.Default.pt_b;
         	this._pt_b_Copy.IsChecked = Properties.Settings.Default.pt_b_Copy;
         	this._pt_b_Copy1.IsChecked = Properties.Settings.Default.pt_b_Copy1;
			this._pt_b_Copy2.IsChecked = Properties.Settings.Default.pt_b_Copy2;
			this._pt_b_Copy3.IsChecked = Properties.Settings.Default.pt_b_Copy3;
			
			this._pt_b_Copy11.IsChecked = Properties.Settings.Default.pt_b_Copy11;
			this._pt_b_Copy12.IsChecked = Properties.Settings.Default.pt_b_Copy12;
			this._pt_b_Copy13.IsChecked = Properties.Settings.Default.pt_b_Copy13;
			this._pt_b_Copy14.IsChecked = Properties.Settings.Default.pt_b_Copy14;
			this._pt_b_Copy15.IsChecked = Properties.Settings.Default.pt_b_Copy15;
			this._pt_b_Copy16.IsChecked = Properties.Settings.Default.pt_b_Copy16;
			this._pt_b_Copy17.IsChecked = Properties.Settings.Default.pt_b_Copy17;
			
			//Windows Tweak default settings
			this._wt_b.IsChecked = Properties.Settings.Default.wt_b;
			this._wt_b_Copy.IsChecked = Properties.Settings.Default.wt_b_Copy;
			this._wt_b_Copy1.IsChecked = Properties.Settings.Default.wt_b_Copy1;
			this._wt_b_Copy2.IsChecked = Properties.Settings.Default.wt_b_Copy2;
			this._wt_b_Copy3.IsChecked = Properties.Settings.Default.wt_b_Copy3;
			this._wt_b_Copy4.IsChecked = Properties.Settings.Default.wt_b_Copy4;
			this._wt_b_Copy5.IsChecked = Properties.Settings.Default.wt_b_Copy5;
			this._wt_b_Copy6.IsChecked = Properties.Settings.Default.wt_b_Copy6;
			this._wt_b_Copy7.IsChecked = Properties.Settings.Default.wt_b_Copy7;
			this._wt_b_Copy8.IsChecked = Properties.Settings.Default.wt_b_Copy8;
			this._wt_b_Copy9.IsChecked = Properties.Settings.Default.wt_b_Copy9;
			this._wt_b_Copy10.IsChecked = Properties.Settings.Default.wt_b_Copy10;
			this._wt_b_Copy11.IsChecked = Properties.Settings.Default.wt_b_Copy11;
			this._wt_b_Copy12.IsChecked = Properties.Settings.Default.wt_b_Copy12;
			this._wt_b_Copy13.IsChecked = Properties.Settings.Default.wt_b_Copy13;
			this._wt_b_Copy14.IsChecked = Properties.Settings.Default.wt_b_Copy14;
			this._wt_b_Copy15.IsChecked = Properties.Settings.Default.wt_b_Copy15;
			this._wt_b_Copy16.IsChecked = Properties.Settings.Default.wt_b_Copy16;
			this._wt_b_Copy17.IsChecked = Properties.Settings.Default.wt_b_Copy17;
			this._wt_b_Copy18.IsChecked = Properties.Settings.Default.wt_b_Copy18;
			
			
			//Security Tweak default settings
			this._st_b.IsChecked = Properties.Settings.Default.st_b;
			this._st_b_Copy.IsChecked = Properties.Settings.Default.st_b_Copy;
			this._st_b_Copy1.IsChecked = Properties.Settings.Default.st_b_Copy1;
			this._st_b_Copy2.IsChecked = Properties.Settings.Default.st_b_Copy2;
			this._st_b_Copy3.IsChecked = Properties.Settings.Default.st_b_Copy3;
			this._st_b_Copy4.IsChecked = Properties.Settings.Default.st_b_Copy4;
			this._st_b_Copy5.IsChecked = Properties.Settings.Default.st_b_Copy5;
			this._st_b_Copy6.IsChecked = Properties.Settings.Default.st_b_Copy6;
			this._st_b_Copy7.IsChecked = Properties.Settings.Default.st_b_Copy7;
			this._st_b_Copy8.IsChecked = Properties.Settings.Default.st_b_Copy8;
			this._st_b_Copy9.IsChecked = Properties.Settings.Default.st_b_Copy9;
			this._st_b_Copy10.IsChecked = Properties.Settings.Default.st_b_Copy10;
			this._st_b_Copy11.IsChecked = Properties.Settings.Default.st_b_Copy11;
			this._st_b_Copy12.IsChecked = Properties.Settings.Default.st_b_Copy12;
			this._st_b_Copy13.IsChecked = Properties.Settings.Default.st_b_Copy13;
			this._st_b_Copy14.IsChecked = Properties.Settings.Default.st_b_Copy14;
			this._st_b_Copy15.IsChecked = Properties.Settings.Default.st_b_Copy15;
			this._st_b_Copy16.IsChecked = Properties.Settings.Default.st_b_Copy16;
			this._st_b_Copy17.IsChecked = Properties.Settings.Default.st_b_Copy17;
			this._st_b_Copy18.IsChecked = Properties.Settings.Default.st_b_Copy18;
			this._st_b_Copy19.IsChecked = Properties.Settings.Default.st_b_Copy19;
			this._st_b_Copy20.IsChecked = Properties.Settings.Default.st_b_Copy20;
			this._st_b_Copy21.IsChecked = Properties.Settings.Default.st_b_Copy21;
			this._st_b_Copy22.IsChecked = Properties.Settings.Default.st_b_Copy22;
			this._st_b_Copy23.IsChecked = Properties.Settings.Default.st_b_Copy23;
			this._st_b_Copy24.IsChecked = Properties.Settings.Default.st_b_Copy24;
			
			//Personalization Tweak default settings
			this._mt_b.IsChecked = Properties.Settings.Default.mt_b;
			this._mt_b_Copy.IsChecked = Properties.Settings.Default.mt_b_Copy;
			this._mt_b_Copy1.IsChecked = Properties.Settings.Default.mt_b_Copy1;
			this._mt_b_Copy2.IsChecked = Properties.Settings.Default.mt_b_Copy2;
			this._mt_b_Copy3.IsChecked = Properties.Settings.Default.mt_b_Copy3;
			this._mt_b_Copy4.IsChecked = Properties.Settings.Default.mt_b_Copy4;
			this._mt_b_Copy5.IsChecked = Properties.Settings.Default.mt_b_Copy5;
			this._mt_b_Copy6.IsChecked = Properties.Settings.Default.mt_b_Copy6;
			this._mt_b_Copy7.IsChecked = Properties.Settings.Default.mt_b_Copy7;
			this._mt_b_Copy8.IsChecked = Properties.Settings.Default.mt_b_Copy8;
			this._mt_b_Copy9.IsChecked = Properties.Settings.Default.mt_b_Copy9;
			this._mt_b_Copy10.IsChecked = Properties.Settings.Default.mt_b_Copy10;
			this._mt_b_Copy11.IsChecked = Properties.Settings.Default.mt_b_Copy11;
			this._mt_b_Copy12.IsChecked = Properties.Settings.Default.mt_b_Copy12;
			this._mt_b_Copy13.IsChecked = Properties.Settings.Default.mt_b_Copy13;
			this._mt_b_Copy14.IsChecked = Properties.Settings.Default.mt_b_Copy14;
			this._mt_b_Copy15.IsChecked = Properties.Settings.Default.mt_b_Copy15;
			this._mt_b_Copy16.IsChecked = Properties.Settings.Default.mt_b_Copy15;
			this._mt_b_Copy17.IsChecked = Properties.Settings.Default.mt_b_Copy17;
			this._mt_b_Copy18.IsChecked = Properties.Settings.Default.mt_b_Copy18;
			
			
			//Aditional Tweak default settings
			this._at_b.IsChecked = Properties.Settings.Default.at_b;
			this._at_b1.IsChecked = Properties.Settings.Default.at_b1;
			this._at_b2.IsChecked = Properties.Settings.Default.at_b2;
			this._at_b3.IsChecked = Properties.Settings.Default.at_b3;
			this._at_b4.IsChecked = Properties.Settings.Default.at_b4;
			this._at_b5.IsChecked = Properties.Settings.Default.at_b5;
			this._at_b6.IsChecked = Properties.Settings.Default.at_b6;	
			
			//Settings
			this.usbLock_bt.IsChecked = Properties.Settings.Default.usbLock;
            this._ck1_st.IsChecked = Properties.Settings.Default.ck1_st;
            this._ck2_st.IsChecked = Properties.Settings.Default.ck2_st;
            this._ck3_st.IsChecked = Properties.Settings.Default.ck3_st;
            this._ck4_st.IsChecked = Properties.Settings.Default.ck4_st;
            this._ck5_st.IsChecked = Properties.Settings.Default.ck5_st;
            this._ck6_st.IsChecked = Properties.Settings.Default.ck6_st;
			
			    
         }

		
	
		// Menu
		//======================================================================================================================
		
		
		private void homeMenu_Selected(object sender, RoutedEventArgs e)
        {
            this.homeTab.Visibility = System.Windows.Visibility.Visible;
            this.optimizerTab.Visibility = System.Windows.Visibility.Hidden;
            this.sequrityTab.Visibility = System.Windows.Visibility.Hidden;
            this.networkTab.Visibility = System.Windows.Visibility.Hidden;
            this.customizationTab.Visibility = System.Windows.Visibility.Hidden;
            this.systemToolsTab.Visibility = System.Windows.Visibility.Hidden;
            this.helpTab.Visibility = System.Windows.Visibility.Hidden;
        }

        private void optimizerMenu_Selected(object sender, RoutedEventArgs e)
        {
            this.homeTab.Visibility = System.Windows.Visibility.Hidden;
            this.optimizerTab.Visibility = System.Windows.Visibility.Visible;
            this.sequrityTab.Visibility = System.Windows.Visibility.Hidden;
            this.networkTab.Visibility = System.Windows.Visibility.Hidden;
            this.customizationTab.Visibility = System.Windows.Visibility.Hidden;
            this.systemToolsTab.Visibility = System.Windows.Visibility.Hidden;
            this.helpTab.Visibility = System.Windows.Visibility.Hidden;
        }

        private void sequrityMenu_Selected(object sender, RoutedEventArgs e)
        {
            this.homeTab.Visibility = System.Windows.Visibility.Hidden;
            this.optimizerTab.Visibility = System.Windows.Visibility.Hidden;
            this.sequrityTab.Visibility = System.Windows.Visibility.Visible;
            this.networkTab.Visibility = System.Windows.Visibility.Hidden;
            this.customizationTab.Visibility = System.Windows.Visibility.Hidden;
            this.systemToolsTab.Visibility = System.Windows.Visibility.Hidden;
            this.helpTab.Visibility = System.Windows.Visibility.Hidden;
        }

        private void networkMenu_Selected(object sender, RoutedEventArgs e)
        {
            this.homeTab.Visibility = System.Windows.Visibility.Hidden;
            this.optimizerTab.Visibility = System.Windows.Visibility.Hidden;
            this.sequrityTab.Visibility = System.Windows.Visibility.Hidden;
            this.networkTab.Visibility = System.Windows.Visibility.Visible;
            this.customizationTab.Visibility = System.Windows.Visibility.Hidden;
            this.systemToolsTab.Visibility = System.Windows.Visibility.Hidden;
            this.helpTab.Visibility = System.Windows.Visibility.Hidden;
        }

        private void systemToolsMenu_Selected(object sender, RoutedEventArgs e)
        {
            this.homeTab.Visibility = System.Windows.Visibility.Hidden;
            this.optimizerTab.Visibility = System.Windows.Visibility.Hidden;
            this.sequrityTab.Visibility = System.Windows.Visibility.Hidden;
            this.networkTab.Visibility = System.Windows.Visibility.Hidden;
            this.customizationTab.Visibility = System.Windows.Visibility.Hidden;
            this.systemToolsTab.Visibility = System.Windows.Visibility.Visible;
            this.helpTab.Visibility = System.Windows.Visibility.Hidden;
        }

        private void customizationMenu_Selected(object sender, RoutedEventArgs e)
        {
            this.homeTab.Visibility = System.Windows.Visibility.Hidden;
            this.optimizerTab.Visibility = System.Windows.Visibility.Hidden;
            this.sequrityTab.Visibility = System.Windows.Visibility.Hidden;
            this.networkTab.Visibility = System.Windows.Visibility.Hidden;
            this.customizationTab.Visibility = System.Windows.Visibility.Visible;
            this.systemToolsTab.Visibility = System.Windows.Visibility.Hidden;
            this.helpTab.Visibility = System.Windows.Visibility.Hidden;
			
			this.tweake_main.Visibility=System.Windows.Visibility.Visible;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
            this.PersonalizationTweakSubPanel.Visibility = System.Windows.Visibility.Hidden;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
        }

        private void helpMenu_Selected(object sender, RoutedEventArgs e)
        {
            this.homeTab.Visibility = System.Windows.Visibility.Hidden;
            this.optimizerTab.Visibility = System.Windows.Visibility.Hidden;
            this.sequrityTab.Visibility = System.Windows.Visibility.Hidden;
            this.networkTab.Visibility = System.Windows.Visibility.Hidden;
            this.customizationTab.Visibility = System.Windows.Visibility.Hidden;
            this.systemToolsTab.Visibility = System.Windows.Visibility.Hidden;
            this.helpTab.Visibility = System.Windows.Visibility.Visible;
        }

       
		
		// System tools>> windows tools window
		//=================================================================================================================================
		
		 private void winToolsNext_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	this.winTools_1.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_2.Visibility=System.Windows.Visibility.Visible;
			this.winTools_3.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_4.Visibility=System.Windows.Visibility.Hidden;
        }

        private void winToolsNext_bt_2_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	this.winTools_1.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_2.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_3.Visibility=System.Windows.Visibility.Visible;
			this.winTools_4.Visibility=System.Windows.Visibility.Hidden;
        }

        private void winToolsNext_bt_3_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	this.winTools_1.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_2.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_3.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_4.Visibility=System.Windows.Visibility.Visible;
        }

        private void winToolsNext_bt_4_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	this.winTools_1.Visibility=System.Windows.Visibility.Visible;
			this.winTools_2.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_3.Visibility=System.Windows.Visibility.Hidden;
			this.winTools_4.Visibility=System.Windows.Visibility.Hidden;
        }

		
		
		//Tweak Main Interface 
		//===============================================================================================================
		private void WinTweak_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.tweake_main.Visibility=System.Windows.Visibility.Hidden;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Visible;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PersonalizationTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
		}

		private void SecurityTweak_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.tweake_main.Visibility=System.Windows.Visibility.Hidden;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Visible;
			this.PersonalizationTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
		}

		private void NetworkTweak_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.tweake_main.Visibility=System.Windows.Visibility.Hidden;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Visible;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
            this.PersonalizationTweakSubPanel.Visibility = System.Windows.Visibility.Hidden;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
		}

		private void PerformaneTweak_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.tweake_main.Visibility=System.Windows.Visibility.Hidden;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Visible;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
            this.PersonalizationTweakSubPanel.Visibility = System.Windows.Visibility.Hidden;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
		}

		private void MultimediaTweak_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.tweake_main.Visibility=System.Windows.Visibility.Hidden;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
            this.PersonalizationTweakSubPanel.Visibility = System.Windows.Visibility.Visible;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
		}

		private void AdditionalTweak_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.tweake_main.Visibility=System.Windows.Visibility.Hidden;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
            this.PersonalizationTweakSubPanel.Visibility = System.Windows.Visibility.Hidden;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Visible;
		}
		
		private void PerformanceTweakBack_bt_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.tweake_main.Visibility=System.Windows.Visibility.Visible;
			this.WindowsTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.PerformanceTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.NetworkTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
			this.SecurityTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
            this.PersonalizationTweakSubPanel.Visibility = System.Windows.Visibility.Hidden;
			this.AdditionalTweakSubPanel.Visibility=System.Windows.Visibility.Hidden;
		}

	
		//=============================================================================================================================
		
		
		private void performanceMonitorButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{ 
				Process p =new Process();
                p.StartInfo.FileName = "perfmon.msc";
				p.Start();
			    p.WaitForExit();
				
			//System.Diagnostics.Process.Start("perfmon.msc");
				
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void diskDefragment_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("cmd","/k fsutil repair query c:");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void diskErrorFixer_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("dfrgui.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void diskCleaner_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("cleanmgr.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void diskFrasher_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\tree.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void startupManager_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("msconfig.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void unistaller_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("appwiz.cpl");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void taskManager_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("taskmgr.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void treblshooting_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("control.exe","/name Microsoft.Troubleshooting");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void backupManager_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("sdclt.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void services_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("services.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void conrolPanelGoodMode_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void computerManagement_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("compmgmt.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void hpFormatTools_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\HP Format tools.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void fatTontfs_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void power_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("powercfg.cpl");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        
        }

        private void windowsFileProtection_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("cmd","/k sfc/scannow");
			}
			
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void resourceMonitor_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("resmon.exe");
			}
			
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void taskSchedular_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("taskschd.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void quickFix_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application//FixWin.exe");//powershell.exe
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void windowsMobilityCenter_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("mblctr");//powershell.exe
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
		private void powerShell_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("powershell_ise.exe");//powershell.exe
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void dataRecovery_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\DiskDigger.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void eventViewer_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("eventvwr.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void groupPliocy_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("gpedit.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void cmd_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("cmd","cd\\");
			}
			catch
			{
    		MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void regedit_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
			try
			{
				System.Diagnostics.Process.Start("regedit.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void diskMangement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("diskmgmt.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void systemStatus_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("msinfo32.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void deviceManager_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("devmgmt.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void multimediaInfo_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("dxdiag.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void malwareScaner_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
			try
			{
                System.Diagnostics.Process.Start("Application\\cwshredder.exe");
                //System.Diagnostics.Process.Start("MRT.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void defender_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("dxdiag.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void firewall_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("WF.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void bitlocker_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.BitLockerDriveEncryption");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        

        private void UAC_settings_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("UserAccountControlSettings.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void DEPtools_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("SystemPropertiesDataExecutionPrevention.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void systemRestore_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("rstrui.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void systemPerformanceOption_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("systempropertiesperformance.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
		
		private void systemPoperties_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("SystemPropertiesComputerName.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }
		
		private void uninstaller_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("Application\\advanced-uninstaller.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
        private void internetOption_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
			try
			{
				System.Diagnostics.Process.Start("inetcpl.cpl");
			}
			catch
			{
    		MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void remoteDesktop_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try	
			{
				System.Diagnostics.Process.Start("mstsc.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void sharedFolder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("fsmgmt.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void sharedFolderWizard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("shrpubw");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
		
		
		private void networkMonitor_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("fsmgmt.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }
		
		
		
		private void getMac_bt1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("cmd", "/k getmac");	
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		
		private void connectionTest_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("cmd", "/k ping 8.8.8.8");
				
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void changeMac_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("Application//k-mac.exe");
				
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void ipconfig_bt1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("cmd", "/k ipconfig");
				//System.Diagnostics.Process.Start("ipconfig.exe");//working
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

        private void windowsUpdate_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("cmd","/k sfc/scannow");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

		
        private void controlpanel_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe");//("Control.exe","/name Microsoft.ActionCenter");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void quickClean_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				QuickClean();
				MessageBox.Show("Cleaned junk file successfully. ","System Mechanic",MessageBoxButton.OK,MessageBoxImage.Information);
			}
			catch
			{
    			MessageBox.Show("Quick clean does not perform.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void registryClean_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\Registry Cleaner\\Software-art Registry Cleaner.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
			
        }
		
		private void junkCleaner_bt1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("Application\\Disk Cleaner\\Software-art Disk Cleaner.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

        private void memoryOptimization_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\RamCleaner.vbs");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void defender_bt1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
                System.Diagnostics.Process.Start(@"C:\Program Files\Microsoft Security Client\msseces.exe");
                //System.Diagnostics.Process.Start("Control.exe","/name Microsoft.WindowsDefender");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void autorunManager_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.AutoPlay");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void seqrityCenter_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.SecurityCenter");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void taskManager_bt2_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("TM.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void personalization_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.Personalization");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void screenResolution_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.Display");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void colorManagement_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.ColorManagement");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void taskManager_bt1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.TaskbarAndStartMenu");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void networkSharingCenter_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.NetworkAndSharingCenter");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void homeGroup_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.HomeGroup");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

       
        private void paint_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("mspaint.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void diagnosticsTools_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.Troubleshooting");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void directX_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("dxdiag.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        
        private void systemRestore_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("rstrui.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void unintaller_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	/*try{
			System.Diagnostics.Process.Start("control.exe","/name Microsoft.ProgramsAndFeatures");
			}
			catch()
			{
    		MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}*/

            try	
			{
				System.Diagnostics.Process.Start("appwiz.cpl");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}


        }

        private void winFetures_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				//System.Diagnostics.Process.Start("control.exe","/name Microsoft.ProgramsAndFeatures");
				System.Diagnostics.Process.Start("optionalfeatures");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void addNewDevice_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("devicepairingwizard");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void memoryCheckup_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("mdsched");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void deviceandPrinter_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.DevicesAndPrinters ");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void firewall_bt1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("firewall.cpl");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void bilocker_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("control.exe","/name Microsoft.BitLockerDriveEncryption");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void diskCheck_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("chkdsk.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void search_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void calculator_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("calc.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void Games_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void wordpad_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("write.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void manifire_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("magnify.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void keyboard_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("osk.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void soundRecorder_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("soundrecorder.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void notePad_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("notepad.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void snippingTools_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("snippingtool.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void notes_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("StikyNot.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void MediaCenter_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("wmplayer.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void windowsUpdate_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("wuapp.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void remoteAssistance_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("msra.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void windowsEasyTrasnfer_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("migwiz");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void synCenter_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("control.exe","/name Microsoft.SyncCenter");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void remoteDesktop_bt1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("mstsc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
		
		private void certification_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("certmgr.msc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
		
		private void systemRecovery_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("recdisc");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
		
		private void backupUserAccount_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("credwiz");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}
		
		private void getMac_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("cmd","/k getmac");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void ipconfig_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("cmd","/k ipconfig.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void networkManager_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("control.exe","/name Microsoft.NetworkAndSharingCenter");
			}
			catch
			{
    		
				MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }
 		
		private void psr_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("psr.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }
        
		private void easeAccessCenter_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("utilman");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void ramCleaner_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void cpuZ_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\GPU-Z.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void advanceUnintaller_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\advanced-uninstaller.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void teamviwer_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\TeamViewerQS.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void freeget_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\Freeget.exe");
			}
			catch
			{									
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void ipscan3_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\netscan.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void ultradefragment_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\Ultradefragmenter.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void converter_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\Convertor\\unite convertor.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void processManager_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\procexp.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void junkCleaner_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\cleaner.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void systeminfo_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }
		
		
		// Internet Acceleration
        private void accelerateConnection_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableBucketSize", 1, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "CacheHashTableSize", 384, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "MaxCacheEntryTtlLimit", 64000, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters", "MaxSOACacheEntryTtlLimit", 301, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEfforLimit",0, Microsoft.Win32.RegistryValueKind.DWord);

                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SoftwareMicrosoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPerServer", 16, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SoftwareMicrosoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPer1_0Server", 16, Microsoft.Win32.RegistryValueKind.DWord);

                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "IRPStackSize", 32, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "SizReqBuf", 17424, Microsoft.Win32.RegistryValueKind.DWord);//If ram grater than 512 
               //Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "SizReqBuf", 4356, Microsoft.Win32.RegistryValueKind.DWord);//If ram less than 512 
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DefaultTTL",64, Microsoft.Win32.RegistryValueKind.DWord);

                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\", "TcpNumConnections", 70000, Microsoft.Win32.RegistryValueKind.DWord);

                try
                {
                    const string arg = @"/c cd\ & netsh int tcp set global chimney=enabled 
                                            & netsh int tcp set global autotuninglevel=normal 
                                            & netsch int tcp set global congestionprovider=ctcp                                                                                        
                                            & netsh int tcp set heuristics disabled                                            
                                            & regsvr32 actxprxy.dll 
                                            
                                        ";  // execute multiple line using '&' command
                    Process.Start("CMD.exe", arg);
                }
                catch
                {
                }    
            	RestartExplorer();
				
				MessageBox.Show("Internet acceleration successfully completed.", "System Mechanics", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
            }
        }

       

        private void undelete_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	
        	try
			{
				System.Diagnostics.Process.Start("Application\\DiskDigger.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void shortcutCleaner_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\shortcutsfixer.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

        private void mastermindContacts_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				System.Diagnostics.Process.Start("Application\\Porichito\\Porichito.jar");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

		private void timtimPlayer_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	try
			{
				//System.Diagnostics.Process.Start("Application\\TimTim Player\\Media palyer.exe");
				System.Diagnostics.Process.Start("Application\\FixWin.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
        }

			private void backupAndRestore_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("sdclt");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void userAccount_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("Control.exe","/name Microsoft.UserAccounts");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void driverSignatureVerification_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("sigverif");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void msContacts_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("wab");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void diskCleanup_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("cleanmgr.exe");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void Fonts_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("fonts");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void presentationSettings_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("presentationsettings");
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		//system repair
		private void systemRepair_bt_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 0, Microsoft.Win32.RegistryValueKind.DWord);                //ck1=F
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableRegistryTools", 0, Microsoft.Win32.RegistryValueKind.DWord);          //ck2=F 
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoControlPanel", 0, Microsoft.Win32.RegistryValueKind.DWord);              //ck4=F
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 1, Microsoft.Win32.RegistryValueKind.DWord);          //ck6=T
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0, Microsoft.Win32.RegistryValueKind.DWord);                 //ck9=T
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRun", 0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoViewContextMenu", 0, Microsoft.Win32.RegistryValueKind.DWord);            
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoFind",0, Microsoft.Win32.RegistryValueKind.DWord);
				
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","NoDesktopCleanupWizard",1, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","NoDesktop",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","NoLogOff",0, Microsoft.Win32.RegistryValueKind.DWord);
				
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","NoRecentDocsMenu",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","NoRecentDocsHistory",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","ClearRecentDocsOnExit",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","NoInstrumentation",0, Microsoft.Win32.RegistryValueKind.DWord);
				
				
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies", "NoLowDiskSpaceChecks", 0, Microsoft.Win32.RegistryValueKind.DWord);    
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPerServer", 12, Microsoft.Win32.RegistryValueKind.DWord);            
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPer1_0Server", 12, Microsoft.Win32.RegistryValueKind.DWord);            
				
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop","LowLevelHooksTimeout",5000, Microsoft.Win32.RegistryValueKind.DWord);				
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop","MenuShowDelay",400, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop","WaitToKillAppTimeout",1000, Microsoft.Win32.RegistryValueKind.DWord);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop","HungAppTimeout",10000, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop","AutoEndTasks",1, Microsoft.Win32.RegistryValueKind.DWord);
				
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\ActiveDesktop","NoChangingWallpaper",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\System", "DisableCMD", 0, Microsoft.Win32.RegistryValueKind.DWord);                                   //ck3=F
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\MMC\{8FC0B734-A0E1-11D1-A7D3-0000F87571E3}","Restrict_Run",0, Microsoft.Win32.RegistryValueKind.DWord); //enable gpedit
				
				
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoFolderOptions", 0, Microsoft.Win32.RegistryValueKind.DWord);            //ck8=F
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled", 1, Microsoft.Win32.RegistryValueKind.DWord);                                //ck11=F
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoClose", 0, Microsoft.Win32.RegistryValueKind.DWord);                    //ck12=F
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\AppCompat", "VDMDisallowed", 1, Microsoft.Win32.RegistryValueKind.DWord);                 	
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\AlwaysUnloadDLL", "Default", 0, Microsoft.Win32.RegistryValueKind.DWord);            
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management","ClearPageFileAtShutdown",0, Microsoft.Win32.RegistryValueKind.DWord);//ck14=F
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\srservice", "Start", 2, Microsoft.Win32.RegistryValueKind.DWord);                    					//ck17=F
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows NT\SystemRestore", "DisableConfig", 0, Microsoft.Win32.RegistryValueKind.DWord);				
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", 1, Microsoft.Win32.RegistryValueKind.DWord);      //ck19=F
                
				//Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched","NonBestEfforLimit", 0, Microsoft.Win32.RegistryValueKind.DWord);                     		 //ck20=F
                
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanManServer\Parameters", "AutoShareWks", 0, Microsoft.Win32.RegistryValueKind.DWord);                  //ck21=T
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\WindowsMediaPlayer", "DisableAutoUpdate", 1, Microsoft.Win32.RegistryValueKind.DWord);            
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareServer", 0, Microsoft.Win32.RegistryValueKind.DWord);            				
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\Uninstall","NoAddRemovePrograms",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CrashControl","AutoReboot",1, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control","WaitToKillServiceTimeout",1000, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management","DisablePagingExecutive",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Memory Management","LargeSystemCache",0, Microsoft.Win32.RegistryValueKind.DWord);											
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer","NoPropertiesMyComputer",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\policies\Explorer","NoDrives",0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\RRamdisk\Parameters","UsePAE",1, Microsoft.Win32.RegistryValueKind.DWord);				
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\StandardProfile","EnableFirewall",1, Microsoft.Win32.RegistryValueKind.DWord);				
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server","fDenyTSConnections",1, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\ Control\ SessionManager", "ProtectionMode ",1, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System","AllowBlockingAppsAtShutdown",0, Microsoft.Win32.RegistryValueKind.DWord);
				
				
				// shortcut icon fixer for future work
				
				RestartExplorer();
				System.Windows.MessageBox.Show("System Reparing complete.", "System Mechanics", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
				System.Windows.MessageBox.Show("Sorry, application could not perform this operation.", "System Mechanics", MessageBoxButton.OK, MessageBoxImage.Error);
            }  
		}

		private void usbLock_bt_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (usbLock_bt.IsChecked == true)
            {
                //disable USB storage...
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\UsbStor", "Start", 4, Microsoft.Win32.RegistryValueKind.DWord);
                
                
            }
            else
            {
                //enable USB storage...
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\UsbStor", "Start", 3, Microsoft.Win32.RegistryValueKind.DWord);
               
                
				}
			RestartExplorer();	
            Properties.Settings.Default.usbLock =usbLock_bt.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
  

    //============================================================================================================================
      
      
		private void DisableTaskmanager_bt_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			try{
			
				RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
        		if (objRegistryKey.GetValue("DisableTaskMgr") == null)
           			objRegistryKey.SetValue("DisableTaskMgr", "1");
        		else
            		objRegistryKey.DeleteValue("DisableTaskMgr");
        			objRegistryKey.Close();
			}
			catch
			{
				MessageBox.Show("Sorry, application is not run as a administrative mode. Please run System engineer as a administrative mode.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void DisableTaskmanager_bt_Unchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
        		if (objRegistryKey.GetValue("DisableTaskMgr") == null)
            	objRegistryKey.SetValue("DisableTaskMgr", "0");
        		else
            	objRegistryKey.DeleteValue("DisableTaskMgr");
        		objRegistryKey.Close();
			}
			catch
			{
				MessageBox.Show("Sorry, application is not run as a administrative mode. Please run System engineer as a administrative mode.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void DisableTaskmanager_bt_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			/*try{RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Policies\System");
        if (objRegistryKey.Equals("DisableTaskMgr","0") == true)
			
			 else
           DisableTaskmanager_bt.IsChecked==true;
        objRegistryKey.Close();}
			catch(){MessageBox.Show("Sorry, application Error .","Error",MessageBoxButton.OK,MessageBoxImage.Error);}
		*/
		}

		private void WindowsTweak_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			
		}
		
		
		

		private void scanButtom_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			try
			{
				//System.Diagnostics.Process.Start("Application\\USB Disk Protector\\USB_Antivirus.exe");
				MessageBox.Show("USB Disk Protector runing on taskbar.","System Mechanic",MessageBoxButton.OK,MessageBoxImage.Information);
			}
			catch
			{
    			MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}	
		
//===========================================================================================================================================================
        
		//Windows Tweak
		private void _wt_b_Click(object sender, System.Windows.RoutedEventArgs e)//Disable windows media center
		{
			if (_wt_b.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\WindowsMediaCenter", "MediaCenter", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\WindowsMediaCenter", "MediaCenter", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b = _wt_b.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _wt_b_Copy_Click(object sender, System.Windows.RoutedEventArgs e)//Disable folder option
		{
			if (_wt_b_Copy.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoFolderOptions", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoFolderOptions", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy = _wt_b_Copy.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _wt_b_Copy1_Click(object sender, System.Windows.RoutedEventArgs e)//Disable windows defender
		{
			if (_wt_b_Copy1.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy1 = _wt_b_Copy1.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _wt_b_Copy2_Click(object sender, System.Windows.RoutedEventArgs e)//Disable mobility center
		{
			if (_wt_b_Copy2.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\MobilityCenter", "NoMobilityCenter", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\MobilityCenter", "NoMobilityCenter", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy2 = _wt_b_Copy2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
        
		private void _wt_b_Copy3_Click(object sender, System.Windows.RoutedEventArgs e)//Disable sidebar
		{
			if (_wt_b_Copy3.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Windows\Sidebar", "TurnOffSidebar", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Windows\Sidebar", "TurnOffSidebar", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy3 = _wt_b_Copy3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _wt_b_Copy4_Click(object sender, System.Windows.RoutedEventArgs e)//Disable windows error reporting
		{
			if (_wt_b_Copy4.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Windows Error Reporting", "Disabled", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Windows Error Reporting", "Disabled", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy4 = _wt_b_Copy4.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _wt_b_Copy5_Click(object sender, System.Windows.RoutedEventArgs e)//disable auto play for removeable devices
		{
			if (_wt_b_Copy5.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy5 = _wt_b_Copy5.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy6_Click(object sender, System.Windows.RoutedEventArgs e)//Remove sequrity tab
		{
			if (_wt_b_Copy6.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoSecurityTab", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoSecurityTab", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy6 = _wt_b_Copy6.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy7_Click(object sender, System.Windows.RoutedEventArgs e)//Disable CD Burning 
		{
			if (_wt_b_Copy7.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoCDBurning", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoCDBurning", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy7 = _wt_b_Copy7.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy8_Click(object sender, System.Windows.RoutedEventArgs e)//programe compatibility assistant
		{
			if (_wt_b_Copy8.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\AppCompat", "DisablePCA", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\AppCompat", "DisablePCA", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy8 = _wt_b_Copy8.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _wt_b_Copy9_Click(object sender, System.Windows.RoutedEventArgs e)//programe compatibility wizard
		{
			if (_wt_b_Copy9.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\AppCompat", "DisablePCA", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\AppCompat", "DisablePCA", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy9 = _wt_b_Copy9.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _wt_b_Copy10_Click(object sender, System.Windows.RoutedEventArgs e)//Disable 16 bit application
		{
			if (_wt_b_Copy10.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\AppCompat", "VDMDisallowed", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\AppCompat", "VDMDisallowed", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy10 = _wt_b_Copy10.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy11_Click(object sender, System.Windows.RoutedEventArgs e)//Run 16 bit programes as a separate process
		{
			if (_wt_b_Copy11.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\WOW", "DefaultSeparateVDM", "Yes", Microsoft.Win32.RegistryValueKind.String);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\WOW", "DefaultSeparateVDM", "", Microsoft.Win32.RegistryValueKind.String);
            }
            Properties.Settings.Default.wt_b_Copy11 = _wt_b_Copy11.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy12_Click(object sender, System.Windows.RoutedEventArgs e)//Disable sound when error occur
		{
			if (_wt_b_Copy12.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.wt_b_Copy12 = _wt_b_Copy12.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy13_Click(object sender, System.Windows.RoutedEventArgs e)//Automatic restart when critical error occur
		{
			if (_wt_b_Copy13.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CrashControl", "AutoReboot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CrashControl", "AutoReboot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy13 = _wt_b_Copy13.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy14_Click(object sender, System.Windows.RoutedEventArgs e)//send error reports
		{
			if (_wt_b_Copy14.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PCHealth\ErrorReporting", "DoReport", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PCHealth\ErrorReporting", "DoReport", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy14 = _wt_b_Copy14.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy15_Click(object sender, System.Windows.RoutedEventArgs e)//show error notification window
		{
			if (_wt_b_Copy15.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PCHealth\ErrorReporting", "ShowUI", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PCHealth\ErrorReporting", "ShowUI", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy15 = _wt_b_Copy15.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy16_Click(object sender, System.Windows.RoutedEventArgs e)//Do not save error report on your computer
		{
			if (_wt_b_Copy16.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.wt_b_Copy16 = _wt_b_Copy16.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy17_Click(object sender, System.Windows.RoutedEventArgs e)//Do not send additional information with error report 
		{
			if (_wt_b_Copy17.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PCHealth\ErrorReporting", "DoReport", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\PCHealth\ErrorReporting", "DoReport", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.wt_b_Copy17 = _wt_b_Copy17.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _wt_b_Copy18_Click(object sender, System.Windows.RoutedEventArgs e)//Do not write error information into system log
		{
			if (_wt_b_Copy18.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.wt_b_Copy18 = _wt_b_Copy18.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
				
		
		//Network Tweak
	
		private void _nt_b_Click(object sender, System.Windows.RoutedEventArgs e)//Him nede entire network for nigheber network
		{
			if (_nt_b.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Network", "NoEntireNetwork", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Network", "NoEntireNetwork", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.nt_b = _nt_b.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _nt_b_Copy_Click(object sender, System.Windows.RoutedEventArgs e)//remote desktop sharing
		{
			if (_nt_b_Copy.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server", "fDenyTSConnections", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Terminal Server", "fDenyTSConnections", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.nt_b_Copy = _nt_b_Copy.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _nt_b_Copy1_Click(object sender, System.Windows.RoutedEventArgs e)//disable default admin and disk drive share server
		{
			if (_nt_b_Copy1.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanManServer\Parameters", "AutoShareWks", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanManServer\Parameters", "AutoShareWks", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.nt_b_Copy1 = _nt_b_Copy1.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _nt_b_Copy2_Click(object sender, System.Windows.RoutedEventArgs e)//restric access of IPC$ for anonymous user 
		{
			if (_nt_b_Copy2.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\LSA", "restrictanonymous", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\LSA", "restrictanonymous", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.nt_b_Copy2 = _nt_b_Copy2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
        
		private void _nt_b_Copy3_Click(object sender, System.Windows.RoutedEventArgs e)//disable recent share in network places
		{
			if (_nt_b_Copy3.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRecentDocsNetHood", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRecentDocsNetHood", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.nt_b_Copy3 = _nt_b_Copy3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _nt_b_Copy4_Click(object sender, System.Windows.RoutedEventArgs e)//disable auto discovery of media contents in share networks 
		{
			if (_nt_b_Copy4.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.nt_b_Copy4 = _nt_b_Copy4.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _nt_b_Copy5_Click(object sender, System.Windows.RoutedEventArgs e)//limited reserved bandwidth
		{
			if (_nt_b_Copy5.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEfforLimit",0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEfforLimit",1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.nt_b_Copy5 = _nt_b_Copy5.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _nt_b_Copy6_Click(object sender, System.Windows.RoutedEventArgs e)//Delete temporary internet files on exit
		{
			if (_nt_b_Copy6.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Cache", "Persistent", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Cache", "Persistent", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.nt_b_Copy6 = _nt_b_Copy6.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _nt_b_Copy7_Click(object sender, System.Windows.RoutedEventArgs e)//Increase max connection of windows
		{
			if (_nt_b_Copy7.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SoftwareMicrosoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPerServer", 16, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SoftwareMicrosoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPer1_0Server", 16, Microsoft.Win32.RegistryValueKind.DWord);

            }
            else
            {
               
            }
            Properties.Settings.Default.nt_b_Copy7 = _nt_b_Copy7.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		
		//Security Tweak
		
		private void _st_b_Click(object sender, System.Windows.RoutedEventArgs e) //Disable Taskmanager 
		{
			if (_st_b.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b = _st_b.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _st_b_Copy_Click(object sender, System.Windows.RoutedEventArgs e) //Disable CMD
		{
			if (_st_b_Copy.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\System", "DisableCMD", 2, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\System", "DisableCMD", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy = _st_b_Copy.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _st_b_Copy1_Click(object sender, System.Windows.RoutedEventArgs e)//Disable Registry Tools
		{
			if (_st_b_Copy1.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableRegistryTools", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DisableRegistryTools", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            Properties.Settings.Default.st_b_Copy1 = _st_b_Copy1.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _st_b_Copy2_Click(object sender, System.Windows.RoutedEventArgs e)//Disable Controlpanel
		{
			if (_st_b_Copy2.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoControlPanel", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoControlPanel", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy2 = _st_b_Copy2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
        
		private void _st_b_Copy3_Click(object sender, System.Windows.RoutedEventArgs e) //Diable System Restore
		{
			if (_st_b_Copy3.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows NT\SystemRestore", "DisableConfig", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows NT\SystemRestore", "DisableConfig", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy3 = _st_b_Copy3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _st_b_Copy4_Click(object sender, System.Windows.RoutedEventArgs e) //Disable Internet Conection
		{
			if (_st_b_Copy4.IsChecked == true)
            {
              try
				{
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyHttp1.1", 0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyServer", "ftp=0.0.0.0:80;gopher=0.0.0.0:80;http=0.0.0.0:80;https=0.0.0.0:80", Microsoft.Win32.RegistryValueKind.String);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyServer", "0.0.0.0:80", Microsoft.Win32.RegistryValueKind.String);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyEnable", 1, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SoftwarePolicies\MicrosoftInternet\ ExplorerControl Panel", "Proxy", 1, Microsoft.Win32.RegistryValueKind.DWord);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyOverride","Do not use proxy server for addresses beginning with:", Microsoft.Win32.RegistryValueKind.String);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyOverrideText","Separate multiple addresses with a semi-colon.", Microsoft.Win32.RegistryValueKind.String);
				}
				catch {}
			}
            else
            {
               try
				{
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyHttp1.1", 1, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyServer", "", Microsoft.Win32.RegistryValueKind.String);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyEnable", 0, Microsoft.Win32.RegistryValueKind.DWord);
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SoftwarePolicies\MicrosoftInternet\ ExplorerControl Panel", "Proxy", 0, Microsoft.Win32.RegistryValueKind.DWord);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyOverride","Do not use proxy server for addresses beginning with:", Microsoft.Win32.RegistryValueKind.String);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\MicrosoftWindows\CurrentVersion\Internet Settings", "ProxyOverrideText","Separate multiple addresses with a semi-colon.", Microsoft.Win32.RegistryValueKind.String);
				}
				catch{}
            }
            Properties.Settings.Default.st_b_Copy4 = _st_b_Copy4.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _st_b_Copy5_Click(object sender, System.Windows.RoutedEventArgs e) //Disable autorun
		{
			if (_st_b_Copy5.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy5 = _st_b_Copy5.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy6_Click(object sender, System.Windows.RoutedEventArgs e) //Disable MMC snap
		{
			if (_st_b_Copy6.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\MMC\{58221C67-EA27-11CF-ADCF-00AA00A80033}", "Restrict_Run", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Policies\Microsoft\MMC\{58221C67-EA27-11CF-ADCF-00AA00A80033}", "Restrict_Run", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy6 = _st_b_Copy6.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy7_Click(object sender, System.Windows.RoutedEventArgs e) //Diable auto logon
		{
			if (_st_b_Copy7.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion\winlogon", "AutoAdminLogon", 0, Microsoft.Win32.RegistryValueKind.DWord);
				
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion\winlogon", "AutoAdminLogon", 1, Microsoft.Win32.RegistryValueKind.DWord);
				
            }
            Properties.Settings.Default.st_b_Copy7 = _st_b_Copy7.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy8_Click(object sender, System.Windows.RoutedEventArgs e)//Disable windows update
		{
			if (_st_b_Copy8.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoWindowsUpdate", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoWindowsUpdate", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy8 = _st_b_Copy8.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy9_Click(object sender, System.Windows.RoutedEventArgs e)//Access right's of windows update
		{
			if (_st_b_Copy9.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\WindowsUpdate", "DisableWindowsUpdateAccess", 1, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\WindowsUpdate", "DisableWindowsUpdateAccess", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            Properties.Settings.Default.st_b_Copy9 = _st_b_Copy9.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _st_b_Copy10_Click(object sender, System.Windows.RoutedEventArgs e) //Auto reboot after windows update
		{
			if (_st_b_Copy10.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy10 = _st_b_Copy10.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy11_Click(object sender, System.Windows.RoutedEventArgs e) //Disable Heuristic scanning
		{
			if (_st_b_Copy11.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.st_b_Copy11 = _st_b_Copy11.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		 
		private void _st_b_Copy12_Click(object sender, System.Windows.RoutedEventArgs e) //Disable archives scans
		{
			if (_st_b_Copy12.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.st_b_Copy12 = _st_b_Copy12.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy13_Click(object sender, System.Windows.RoutedEventArgs e) // Disable removable media scans
		{
			if (_st_b_Copy13.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.st_b_Copy13 = _st_b_Copy13.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy14_Click(object sender, System.Windows.RoutedEventArgs e) // Disable e-mail scans
		{
			if (_st_b_Copy14.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.st_b_Copy14 = _st_b_Copy14.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy15_Click(object sender, System.Windows.RoutedEventArgs e)// Disable real-time protection 
		{
			if (_st_b_Copy15.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.st_b_Copy15 = _st_b_Copy15.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy16_Click(object sender, System.Windows.RoutedEventArgs e) // Disable downloads checkup 
		{
			if (_st_b_Copy16.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.st_b_Copy16 = _st_b_Copy16.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy17_Click(object sender, System.Windows.RoutedEventArgs e) // Disable executable files checkup
		{
			if (_st_b_Copy17.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.st_b_Copy17 = _st_b_Copy17.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy18_Click(object sender, System.Windows.RoutedEventArgs e)//wipe page file when shutdwon
		{
			if (_st_b_Copy18.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "ClearPageFileAtShutdown", 1, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "ClearPageFileAtShutdown", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            Properties.Settings.Default.st_b_Copy18 = _st_b_Copy18.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy19_Click(object sender, System.Windows.RoutedEventArgs e)//clear recent file when logoff
		{
			if (_st_b_Copy19.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "ClearRecentDocsOnExit", 1, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "ClearRecentDocsOnExit", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            Properties.Settings.Default.st_b_Copy19 = _st_b_Copy19.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _st_b_Copy20_Click(object sender, System.Windows.RoutedEventArgs e)//Do not create recent document 
		{
			if (_st_b_Copy20.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRecentDocsHistory", 1, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRecentDocsHistory", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            Properties.Settings.Default.st_b_Copy20 = _st_b_Copy20.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _st_b_Copy21_Click(object sender, System.Windows.RoutedEventArgs e)//Do not store password on disk
		{
			if (_st_b_Copy21.IsChecked == true)
            {
                
            }
            else
            {
                
            }
            Properties.Settings.Default.st_b_Copy21 = _st_b_Copy21.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _st_b_Copy22_Click(object sender, System.Windows.RoutedEventArgs e) //Disable hiden share
		{
			if (_st_b_Copy22.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 0, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareServer", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareWks", 1, Microsoft.Win32.RegistryValueKind.DWord);
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\LanmanServer\Parameters", "AutoShareServer", 1, Microsoft.Win32.RegistryValueKind.DWord);            
            }
            Properties.Settings.Default.st_b_Copy22 = _st_b_Copy22.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _st_b_Copy23_Click(object sender, System.Windows.RoutedEventArgs e) //Disable user tracking
		{
			if (_st_b_Copy23.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoInstrumentation", 1, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoInstrumentation", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            Properties.Settings.Default.st_b_Copy23 = _st_b_Copy23.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _st_b_Copy24_Click(object sender, System.Windows.RoutedEventArgs e)//Disable fast user switching
		{
			if (_st_b_Copy24.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "HideFastUserSwitching", 1, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "HideFastUserSwitching", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.st_b_Copy24 = _st_b_Copy24.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		//Performance Tweak
		
		private void _pt_b1_Click(object sender, System.Windows.RoutedEventArgs e)//low disk space
		{
			if (_pt_b1.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies", "NoLowDiskSpaceChecks", 0, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies", "NoLowDiskSpaceChecks", 1, Microsoft.Win32.RegistryValueKind.DWord); 
            }
            Properties.Settings.Default.pt_b1 = _pt_b1.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _pt_b2_Click(object sender, System.Windows.RoutedEventArgs e)//remove shortcut suffix
		{
			if (_pt_b2.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer", "link", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer", "link", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b2 = _pt_b2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _pt_b3_Click(object sender, System.Windows.RoutedEventArgs e)//Disable hibernet
		{
			if (_pt_b3.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b3 = _pt_b3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _pt_b4_Click(object sender, System.Windows.RoutedEventArgs e)//Disable Startup Sound
		{
			if (_pt_b4.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableStartupSound", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableStartupSound", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b4 = _pt_b4.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
        
		private void _pt_b5_Click(object sender, System.Windows.RoutedEventArgs e)//Disable pagefile
		{
			if (_pt_b5.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "DisablePagingExecutive", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "DisablePagingExecutive", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b5 = _pt_b5.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

        private void _pt_b6_Click(object sender, System.Windows.RoutedEventArgs e)//Disable ntfs encrypted
		{
			if (_pt_b6.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\EFS", "EFSConfiguration", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\EFS", "EFSConfiguration", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b6 = _pt_b6.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		//=======================================================================================================================================================		
		

		
		private void _pt_b_Click(object sender, System.Windows.RoutedEventArgs e)//Disable ntfs filesystem compression
		{
			if (_pt_b.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem", " NtfsDisableCompression", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem", " NtfsDisableCompression", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b = _pt_b.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _pt_b_Copy_Click(object sender, System.Windows.RoutedEventArgs e)//enable self-heling compalibility of ntfs FS
		{
			if (_pt_b_Copy.IsChecked == true)
            {
				try
				{
					System.Diagnostics.Process.Start("cmd","/k fsutil repair query c:");
				}
				catch
				{
					MessageBox.Show("Sorry, this application could not found.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
				}
            }
            else
            {
               
            }
            Properties.Settings.Default.pt_b_Copy = _pt_b_Copy.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _pt_b_Copy1_Click(object sender, System.Windows.RoutedEventArgs e)//enable encrypted pagefile
		{
			if (_pt_b_Copy1.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsEncryptPagingFile", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsEncryptPagingFile", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy1 = _pt_b_Copy1.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _pt_b_Copy2_Click(object sender, System.Windows.RoutedEventArgs e)//enable lage system cache
		{
			if (_pt_b_Copy2.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Memory Management", "LargeSystemCache", 1, Microsoft.Win32.RegistryValueKind.DWord);
			   Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Memory Management", "SystemCacheDirtyPageThreshold", 100, Microsoft.Win32.RegistryValueKind.DWord);//Default 0= haf of ram, 100 or other value means 100MB
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "LargeSystemCache", 0, Microsoft.Win32.RegistryValueKind.DWord);
			   Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Memory Management", "SystemCacheDirtyPageThreshold", 0, Microsoft.Win32.RegistryValueKind.DWord);//Default 0= haf of ram, 100 or other value means 100MB            
			}
            Properties.Settings.Default.pt_b_Copy2 = _pt_b_Copy2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
        
		private void _pt_b_Copy3_Click(object sender, System.Windows.RoutedEventArgs e)//delete pagefile when shutdwon
		{
			if (_pt_b_Copy3.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "ClearPageFileAtShutdown", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "ClearPageFileAtShutdown", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy3 = _pt_b_Copy3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

       
		private void _pt_b_Copy11_Click(object sender, System.Windows.RoutedEventArgs e)//Manage the CPU task priority
		{
			if (_pt_b_Copy11.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "CPUPriority", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "CPUPriority", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy11 = _pt_b_Copy11.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _pt_b_Copy12_Click(object sender, System.Windows.RoutedEventArgs e)//Increasing the USB polling-interval
		{
			if (_pt_b_Copy12.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Class\{36FC9E60-C465-11CF-8056-444553540000}\0000", "IdleEnable", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Class\{36FC9E60-C465-11CF-8056-444553540000}\0000", "IdleEnable", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy12 = _pt_b_Copy12.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _pt_b_Copy13_Click(object sender, System.Windows.RoutedEventArgs e)//Decrease DRAM memory wait states
		{
			if (_pt_b_Copy13.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "FastDRAM", 1, Microsoft.Win32.RegistryValueKind.DWord);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "PCIConcur", 1, Microsoft.Win32.RegistryValueKind.DWord);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "AGPConcur", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
              Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "FastDRAM", 0, Microsoft.Win32.RegistryValueKind.DWord); // delete those value
				//Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "PCIConcur", 0, Microsoft.Win32.RegistryValueKind.DWord);
				//Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\VxD\BIOS", "AGPConcur", o, Microsoft.Win32.RegistryValueKind.DWord);
           
            }
            Properties.Settings.Default.pt_b_Copy13 = _pt_b_Copy13.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _pt_b_Copy14_Click(object sender, System.Windows.RoutedEventArgs e)//Optimize large second level cache
		{
			if (_pt_b_Copy14.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "SecondLevelDataCache", 1024, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "SecondLevelDataCache", 256, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy14 = _pt_b_Copy14.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _pt_b_Copy15_Click(object sender, System.Windows.RoutedEventArgs e)//Enable access to explorer context menu
		{
			if (_pt_b_Copy15.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoViewContextMenu", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoViewContextMenu", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy15 = _pt_b_Copy15.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _pt_b_Copy16_Click(object sender, System.Windows.RoutedEventArgs e)//Disable automatic reboot after system crash
		{
			if (_pt_b_Copy16.IsChecked == true)
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CrashControl", "AutoReboot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\CrashControl", "AutoReboot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy16 = _pt_b_Copy16.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _pt_b_Copy17_Click(object sender, System.Windows.RoutedEventArgs e)//Stop error message when booting
		{
			if (_pt_b_Copy17.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.pt_b_Copy17 = _pt_b_Copy17.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		
		// /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
		
		//Personalize and Multimedia Tweak
		
		
		private void _mt_b_Click(object sender, System.Windows.RoutedEventArgs e)//Disable modify start menu
		{
			if (_mt_b.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoSetTaskbar", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoSetTaskbar", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b = _mt_b.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _mt_b_Copy_Click(object sender, System.Windows.RoutedEventArgs e)//Remove run on start menu 
		{
			if (_mt_b_Copy.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRun", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRun", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy = _mt_b_Copy.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy1_Click(object sender, System.Windows.RoutedEventArgs e)//Remove recent item on start menu
		{
			if (_mt_b_Copy1.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRecentDocsMenu", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoRecentDocsMenu", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy1 = _mt_b_Copy1.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy2_Click(object sender, System.Windows.RoutedEventArgs e)//Disable show all programs 
		{
			if (_mt_b_Copy2.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoStartMenuMorePrograms", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoStartMenuMorePrograms", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy2 = _mt_b_Copy2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy3_Click(object sender, System.Windows.RoutedEventArgs e)//Speed up start menu
		{
			if (_mt_b_Copy3.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "MenuShowDelay", 100, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "MenuShowDelay", 400, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy3 = _mt_b_Copy3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy4_Click(object sender, System.Windows.RoutedEventArgs e)//Show desktop icon
		{
			if (_mt_b_Copy4.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideIcons", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideIcons", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy4 = _mt_b_Copy4.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy5_Click(object sender, System.Windows.RoutedEventArgs e)//Disable notification area 
		{
			if (_mt_b_Copy5.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoTrayItemsDisplay", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoTrayItemsDisplay", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy5 = _mt_b_Copy5.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy6_Click(object sender, System.Windows.RoutedEventArgs e)//Hide inactive icons 
		{
			if (_mt_b_Copy6.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer", "EnableAutoTray", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer", "EnableAutoTray", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy6 = _mt_b_Copy6.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy7_Click(object sender, System.Windows.RoutedEventArgs e)//Show balloons text
		{
			if (_mt_b_Copy7.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "EnableBalloonTips", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "EnableBalloonTips", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy7 = _mt_b_Copy7.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy8_Click(object sender, System.Windows.RoutedEventArgs e)//Disable desktop aero 
		{
			if (_mt_b_Copy8.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "Composition", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
              Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "Composition", 0, Microsoft.Win32.RegistryValueKind.DWord);
            
            }
            Properties.Settings.Default.mt_b_Copy8 = _mt_b_Copy8.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _mt_b_Copy9_Click(object sender, System.Windows.RoutedEventArgs e)//Disable windows previews (thumbnails )
		{
			if (_mt_b_Copy9.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy9 = _mt_b_Copy9.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy10_Click(object sender, System.Windows.RoutedEventArgs e)//Use small icon
		{
			if (_mt_b_Copy10.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy10 = _mt_b_Copy10.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy11_Click(object sender, System.Windows.RoutedEventArgs e)//Show hidden files with system files  
		{
			if (_mt_b_Copy11.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy11 = _mt_b_Copy11.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy12_Click(object sender, System.Windows.RoutedEventArgs e)   //Show file extension 
		{
			if (_mt_b_Copy12.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy12 = _mt_b_Copy12.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy13_Click(object sender, System.Windows.RoutedEventArgs e)//Use checkbox 
		{
			if (_mt_b_Copy13.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy13 = _mt_b_Copy13.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy14_Click(object sender, System.Windows.RoutedEventArgs e)//Remove shortcut arrow 
		{
			if (_mt_b_Copy2.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy2 = _mt_b_Copy2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy15_Click(object sender, System.Windows.RoutedEventArgs e)//Remove shortcut suffix 
		{
			if (_mt_b_Copy3.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy3 = _mt_b_Copy3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy16_Click(object sender, System.Windows.RoutedEventArgs e)//Show windows version on desktop
		{
			if (_mt_b_Copy4.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy4 = _mt_b_Copy4.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _mt_b_Copy17_Click(object sender, System.Windows.RoutedEventArgs e)//Launch folder separate process
		{
			if (_mt_b_Copy17.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy17 = _mt_b_Copy17.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _mt_b_Copy18_Click(object sender, System.Windows.RoutedEventArgs e) //Disable thumbnail cache
		{
			if (_mt_b_Copy18.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
               Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Windows", "NoPopupsOnBoot", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.mt_b_Copy18 = _mt_b_Copy18.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
	
		
		//Additional Tweak//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		
		
		 private void _at_b_Click(object sender, System.Windows.RoutedEventArgs e)//show take ownership
		{
			if (_at_b.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.at_b = _at_b.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _at_b1_Click(object sender, System.Windows.RoutedEventArgs e)//show open cmd here as administrator 
		{
			if (_at_b1.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.at_b1 = _at_b1.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _at_b2_Click(object sender, System.Windows.RoutedEventArgs e)//pin to sart menu and taskbar
		{
			if (_at_b2.IsChecked == true)
            {
               // Microsoft.Win32.Registry.SetValue(@"HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\{a2a9545d-a0c2-42b4-9708-a0b2badd77c8}", "@", "Start Menu Pin", Microsoft.Win32.RegistryValueKind.String);
               // Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\*\shellex\ContextMenuHandlers\{a2a9545d-a0c2-42b4-9708-a0b2badd77c8}", "@", "Taskband Pin", Microsoft.Win32.RegistryValueKind.String);
                
            }
            else
            {
              // Microsoft.Win32.Registry.SetValue(@"-HKEY_CLASSES_ROOT\*\shellex\ContextMenuHandlers\{a2a9545d-a0c2-42b4-9708-a0b2badd77c8}", "@", "", Microsoft.Win32.RegistryValueKind.String);
              // Microsoft.Win32.Registry.SetValue(@"-HKEY_LOCAL_MACHINE\SOFTWARE\Classes\*\shellex\ContextMenuHandlers\{a2a9545d-a0c2-42b4-9708-a0b2badd77c8}", "@", "", Microsoft.Win32.RegistryValueKind.String);
                
            }
            Properties.Settings.Default.at_b2 = _at_b2.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _at_b3_Click(object sender, System.Windows.RoutedEventArgs e)//take ownership for files
		{
			if (_at_b3.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.at_b3 = _at_b3.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _at_b4_Click(object sender, System.Windows.RoutedEventArgs e)//show encrypt decrypt conext menu 
		{
			if (_at_b4.IsChecked == true)
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "EncryptionContextMenu", 1, Microsoft.Win32.RegistryValueKind.DWord);
                              
            }
            else
            {
                Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "EncryptionContextMenu", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            Properties.Settings.Default.at_b4 = _at_b4.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		private void _at_b5_Click(object sender, System.Windows.RoutedEventArgs e)//show copy to folder
		{
			if (_at_b5.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.at_b5 = _at_b5.IsChecked.Value;
            Properties.Settings.Default.Save();
		}
		
		
		private void _at_b6_Click(object sender, System.Windows.RoutedEventArgs e)//show move to folder
		{
			if (_at_b6.IsChecked == true)
            {
               
            }
            else
            {
               
            }
            Properties.Settings.Default.at_b6 = _at_b6.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		 // ///////////////////////////Help and support Tab//////////////////////////////////////////////
		
		private void Update_bt_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("http://www.software-art.somee.com");
			}
			catch
			{
    			MessageBox.Show("Sorry, application error.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		private void HelpAndSupport_bt_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("http://www.facebook.com/pages/Software-Art/124544400963976");
			}
			catch
			{
    			MessageBox.Show("Sorry, application error.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		
		private void _ck1_st_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			 if (_ck1_st.IsChecked == true)
            {
                registryKey.SetValue("System Mechanic", System.Windows.Forms.Application.ExecutablePath);
            }
            else
            {
               registryKey.DeleteValue("System Mechanic");
            }
            
            Properties.Settings.Default.ck1_st = _ck1_st.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _ck2_st_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			 if (_ck2_st.IsChecked == true)
            {
				 
            }
            else
            {

            }

            Properties.Settings.Default.ck2_st = _ck2_st.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _ck3_st_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			 if (_ck3_st.IsChecked == true)
            {
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else
            {
				Microsoft.Win32.Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }

            Properties.Settings.Default.ck3_st = _ck3_st.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _ck4_st_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			 if (_ck4_st.IsChecked == true)
            {
				this.Topmost=true;
            }
            else
            {
				this.Topmost=false;
            }

            Properties.Settings.Default.ck4_st = _ck4_st.IsChecked.Value;
            Properties.Settings.Default.Save();
		}

		private void _ck5_st_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: Add event handler implementation here.
		}

		private void _ck6_st_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("http://www.facebook.com/pages/Software-Art/124544400963976");
			}
			catch
			{
    			MessageBox.Show("Sorry, application error.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
			}
		}

		
	
		
	}
}