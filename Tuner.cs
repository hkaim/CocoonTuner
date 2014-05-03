using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace CocoonTuner
{
    class Tuner : INotifyPropertyChanged 
    {
        public Tuner()
        {        
        }


        public ICommand TuneCommand
        {
            get
            {
                return new DelegateCommand(X => { Tune(); });
            }
        }

        public ICommand Preset1Command
        {
            get
            {
                return new DelegateCommand(X => { SavePreset(1); });
            }
        }
        public ICommand Preset2Command
        {
            get
            {
                return new DelegateCommand(X => { SavePreset(2); });
            }
        }
        public ICommand Preset3Command
        {
            get
            {
                return new DelegateCommand(X => { SavePreset(3); });
            }
        }


        private string GetRessource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = name;            
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        private void SendControlMessage(string data)
        {
            string url = string.Format(Properties.Settings.Default.CocoonUrl, this.Ip);
            HttpWebRequest httpWebRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "text/xml; charset=utf-8";
            httpWebRequest.UserAgent = Properties.Settings.Default.UserAgent;

            using (StreamWriter sw = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                sw.Write(data);
            }
            HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
            if (httpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Unsuccess :(\r\nServer:" + httpWebResponse.StatusCode, "Result");
                return;
            }
            MessageBox.Show("Success :)", "Result");
        }

        private static string XmlEncode(string data)
        {
            return System.Security.SecurityElement.Escape(data);
        }

        private void Tune()
        {
            string data = GetRessource("CocoonTuner.SetVTunerUrl.xml");
            data = string.Format(data, XmlEncode(this.Title), XmlEncode(this.Url), "mtMp3");
            SendControlMessage(data);
        }

        private void SavePreset(int id)
        {
            string data = GetRessource("CocoonTuner.SetPreset.xml");
            data = string.Format(data, id);
            SendControlMessage(data);
        }
        
        public string Title
        {
            get
            {
                return Properties.Settings.Default.Title;
            }
            set
            {
                Properties.Settings.Default.Title = value;                
            }
        }



        public string Ip
        {
            get
            {
                return Properties.Settings.Default.Ip;
            }
            set
            {
                Properties.Settings.Default.Ip = value;
            }
        }

        
        public string Url
        {
            get
            {
                return Properties.Settings.Default.Url;
            }
            set
            {
                Properties.Settings.Default.Url = value;
            }
        }


        public int Type
        {
            get
            {
                return Properties.Settings.Default.Type;
            }
            set
            {
                Properties.Settings.Default.Type = value;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
