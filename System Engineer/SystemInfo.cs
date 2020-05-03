using System;
using System.Management;
using Microsoft.Win32;

namespace System_Engineer
{
	public class SystemInfo
	{
		public SystemInfo()
		{
			// Insert code required on object creation below this point.
		}
		
		
		
		public static string ProcessorInfo()
        {
            
               	String processor = String.Empty; 
           		try
            	{   
					ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT name,manufacturer,NumberOfCores,NumberOfLogicalProcessors FROM Win32_Processor");
               		ManagementObjectCollection objCol = searcher.Get();					
					foreach (ManagementObject mgtObject in objCol)
                	{
                    	processor = "" + mgtObject["name"].ToString()
                                  + "  Manufacturer : " + mgtObject["manufacturer"].ToString()
                                 // + "\tVolage: " + mgtObject["CurrentVoltage"].ToString()
                                  //+ "\tL2 Cache: " + mgtObject["L2CacheSize"].ToString()
                                  //+ "\tL3 cache: " + mgtObject["L3CacheSize"].ToString()
                                  + "  Number of core : " + mgtObject["NumberOfCores"].ToString()
                                  + "  Logical Unite : " + mgtObject["NumberOfLogicalProcessors"].ToString()
                                  //+ "\tProcessor Type: " + mgtObject["ProcessorType"].ToString()
						;

                	}
					return processor;
            	}
            	catch 
            	{
                	return processor = "Unknown";
            	}
				 
				
        }

        public static string HDDInfo()
        {
           		string hdd="";
             	try
            	{   
				 
					ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer,Model,Size FROM Win32_DiskDrive");           
					ManagementObjectCollection objCol = searcher.Get();
					
					foreach (ManagementObject mgtObject in objCol)
                		{
                    
                                hdd=  "" + (Convert.ToInt64(mgtObject["Size"]) / (1024 * 1024 * 1024)).ToString() + "GB"
									+"  Manufacturer : " + mgtObject["Manufacturer"].ToString()
                                    + "  Model : " + mgtObject["Model"].ToString()
                                 // + "\tSerialNumber " + mgtObject["SerialNumber"].ToString()
                                  
                                //  + "\tTotalHeads: " + mgtObject["TotalHeads"].ToString()
                                 // + "\tTotalTracks: " + mgtObject["TotalTracks"].ToString()
                                  ;

                		}
						return hdd;
            	}
            	catch 
            	{
              		return hdd= "Unknown";
            	}
			
        }

        public static string Ram()
        {
			
				try
            	{
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");    
				ManagementObjectCollection objCol = searcher.Get();
				
                foreach (ManagementObject mgtObject in objCol)
                {
                   return "" + (Convert.ToInt64(mgtObject["Capacity"]) / (1024 * 1024)).ToString() + "MB" 
                                  ;

                }
            	}
            	catch
            	{
                	
            	}
				return "Unknown";
				
        }

		
		public static string MemoryInfo()
        {
            
                
				string ramMenufac=String.Empty;
				try
            	{
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_PhysicalMemory");    
				ManagementObjectCollection objCol = searcher.Get();
				
                foreach (ManagementObject mgtObject in objCol)
                {
                    ramMenufac= "Manufacturer : " + mgtObject["Manufacturer"].ToString()
                                //  + "  SerialNumber " + mgtObject["SerialNumber"].ToString()
                                  ;

                }
				return ramMenufac;
            	}
				
            	catch 
            	{
                return ramMenufac = "Manufacturer : Unknown";
            	}
				
        }
		
		
		
		
        public static string MotherboardInfo()
        {
           
            String motherBoard="";  
			try
            {
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer,name,Product FROM Win32_BaseBoard");
                ManagementObjectCollection objCol = searcher.Get();
				
                foreach (ManagementObject mgtObject in objCol)
                {
                   motherBoard=     "" + mgtObject["Product"].ToString()
									+" " + mgtObject["Manufacturer"].ToString()
                                  
									// + "  Model: " + mgtObject["name"].ToString()                                   
                                   //+ "\nSerialNumber " + mgtObject["SerialNumber"].ToString()
                                  ;

                }
				return motherBoard;
            }
            catch   //(Exception ex)
            {
               return motherBoard="Unknown";     //MessageBox.Show("" + ex);
            }
			
        }


       

        public static string BIOSInfo()
        {
           	string bios=null;
			try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_BIOS ");
                ManagementObjectCollection objCol = searcher.Get();
				
                foreach (ManagementObject mgtObject in objCol)
                {
                    bios=  "" + mgtObject["Manufacturer"].ToString();

                }
				return bios;
            }
            catch 
            {
                return bios="Unknown";
            }
			
        }


        public static string VideoInfo()
        {
            string graphicsCard=null;
			try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT AdapterRAM,VideoProcessor FROM Win32_VideoController");
                ManagementObjectCollection objCol = searcher.Get();
				
                foreach (ManagementObject mgtObject in objCol)
                {
                    graphicsCard= "" + (Convert.ToInt64(mgtObject["AdapterRAM"]) / (1024 * 1024)).ToString() + "MB"
                        			// + "\nManufacturer " + mgtObject["Manufacturer"].ToString()
                        			// + "\nVideoArchitecture: " + mgtObject["VideoArchitecture"].ToString()
                                     + "  " + mgtObject["VideoProcessor"].ToString()
                                   ;

                }
				return graphicsCard;
            }
            catch 
            {
              return graphicsCard="Unknown";  
            }
			
        }
		
		
		 public static string OSInfo()
        {
           	string os=null;
			try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption,OSArchitecture FROM Win32_OperatingSystem");
                ManagementObjectCollection objCol = searcher.Get();
			
                foreach (ManagementObject mgtObject in objCol)
                {
                   		os=  "" + mgtObject["Caption"].ToString()
                                  + "  OS Architecture : " + mgtObject["OSArchitecture"].ToString()
                                 // + "  CSName " + mgtObject["CSName"].ToString()
                                 // + "  OSType: " + mgtObject["OSType"].ToString()
                                 // + "  Manufacturer " + mgtObject["Manufacturer"].ToString()
                                 
                                //  + "  Organization: " + mgtObject["Organization"].ToString()
                                //  + "  Version: " + mgtObject["Version"].ToString()
                                 // + "  OperatingSystemSKU: " + mgtObject["OperatingSystemSKU"].ToString()
                                 // + "  OSProductSuit: " + mgtObject["OSProductSuite"].ToString()
                                  ;

                }
				return os;
            }
            catch 
            {
                return "Unknown";//MessageBox.Show("" + ex);
            }
			
			
        }
		
		
		
		public static string ProductBrand()
		{
		
			using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS"))
            {
			
				
				//Product Name
				if (regKey != null)
                {
                    string SystemProductName = regKey.GetValue("SystemProductName") as string;

                    if (!string.IsNullOrEmpty(SystemProductName))
                         return "System Brand : "+SystemProductName;
                    else
                        return "System Brand : Clone PC";
                }
                else
                   return  "System Brand : Clone PC";

			}

		}  
		
		
		
		
		
		
		
		
		
		
		
		
		
		//System Information retrive from registry
		
		public void Systeminfo()
        {           
		   /*	
			using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS"))
            {
                
			//Processor description
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0"))
            {
                if (regKey != null)
                {
                    string procName = regKey.GetValue("ProcessorNameString") as string;

                    if (!string.IsNullOrEmpty(procName))
                        this.textCPU.Text = procName;
                    else
                        this.textCPU.Text = "Unknown";
                }
                else
                    this.textCPU.Text = "Unknown";
            }
				
				
				//Mother Board description
				if (regKey != null)
                {
                    string motherboard = regKey.GetValue("BaseBoardManufacturer") as string;

                    if (!string.IsNullOrEmpty(motherboard))
                        this.textMotherboard.Text = motherboard;
                    else
                        this.textMotherboard.Text = "Unknown";
                }
                else
                    this.textMotherboard.Text = "Unknown";
				
				
				//bios vendor
				if (regKey != null)
                {
                    string bios = regKey.GetValue("BIOSVendor") as string;

                    if (!string.IsNullOrEmpty(bios))
                        this.textBIOS.Text = bios;
                    else
                        this.textBIOS.Text = "Unknown";
                }
                else
                    this.textBIOS.Text = "Unknown";
				
				
		
				//system manufacturer
				if (regKey != null)
                {
                    string SystemManufacturer = regKey.GetValue("SystemManufacturer") as string;

                    if (!string.IsNullOrEmpty(SystemManufacturer))
                        this.textSystemMenufacturer.Text = "System Brand : "+SystemManufacturer;
                    else
                        this.textSystemMenufacturer.Text = "System Brand : Clone PC";
                }
                else
                    this.textSystemMenufacturer.Text = "System Brand : Clone PC";
				*/
				
	
					
            }

        }

	
	}
