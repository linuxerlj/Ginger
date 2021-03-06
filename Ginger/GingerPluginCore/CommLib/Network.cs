#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GingerPluginCore
{
    // TODO; dup class with GingerUtils - combine
    public class Network
    {

        public static string GetFirstLocalHostIPAddress()
        {
            string networkIP = string.Empty;
            if (OperatingSystem.IsWindows())
            {
                networkIP = GetFirstLocalHostIPAddress_Windows();
            }
            else if (OperatingSystem.IsLinux())
            {
                networkIP = GetFirstLocalHostIPAddress_Linux();
            }
            return networkIP;
        }


        public static string GetFirstLocalHostIPAddress_Linux()
        {
            List<UnicastIPAddressInformation> unicastIPAddressInformationList = GetIPAddressCollectionList().ToList();

            string network = unicastIPAddressInformationList.FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                                                                        ?.Address
                                                                        ?.ToString()
                                                                        ?? string.Empty;
            return network;
        }

        public static List<string> GetLocalHostIPAddressesList()
        {
            List<UnicastIPAddressInformation> unicastIPAddressInformationList = GetIPAddressCollectionList().ToList();

            List<string> networkList = new List<string>();

            foreach (UnicastIPAddressInformation unicastIPAddressInformation in unicastIPAddressInformationList)
            {
                string network = unicastIPAddressInformation.Address.ToString();

                if (!string.IsNullOrEmpty(network))
                {
                    networkList.Add(network);
                }
            }

            return networkList;

        }

        public static UnicastIPAddressInformationCollection GetIPAddressCollectionList()
        {
            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(ni =>
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                    && ni.OperationalStatus == OperationalStatus.Up
                    && ni.GetIPProperties().GatewayAddresses.FirstOrDefault() != null
                    && ni.GetIPProperties().UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork) != null);

            IPInterfaceProperties ipInterfaceProperties = networkInterface?.GetIPProperties();

            UnicastIPAddressInformationCollection ipAddressCollection = ipInterfaceProperties.UnicastAddresses;

            return ipAddressCollection;
        }


        public static string GetFirstLocalHostIPAddress_Windows()
        {
            string LocalHostIP = string.Empty;
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> IPList = ipEntry.AddressList.ToList();
            Console.WriteLine("Number of IP Addresses Found: " + IPList.Count);

            if (IPList.Count() == 1)
            {
                // if we have only one return it
                LocalHostIP = IPList[0].ToString();
            }
            else if (IPList.Count() > 1)
            {
                int i = 0;
                foreach (IPAddress ip in IPList)
                {
                    i++;
                    Console.WriteLine("IP Address [" + i + "] : " + ip.ToString());

                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        LocalHostIP = ip.ToString();
                    }
                }
            }

            return LocalHostIP;
        }


    }
}
