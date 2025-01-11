using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenSim.Framework;
using OpenSim.Framework.Servers;
using OpenSim.Framework.Servers.HttpServer;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Server.Base;
using Mono.Addins;
using log4net;
using Nini.Config;

// External library that doesn't exist in the OpenSim distribution
using CsvHelper;

namespace WhoGotWhat
{
    public class FormUploadGetHandler : BaseStreamHandler
    {
        #region Private Fields

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string m_pathToFile;

        #endregion Private Fields

        #region Public Constructors

        public FormUploadGetHandler(string file) :
            base("GET", "/diva/WhoGotWhat")
        {
            m_pathToFile = file;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override byte[] ProcessRequest(string path, Stream requestData,
                IOSHttpRequest httpRequest, IOSHttpResponse httpResponse)
        {
            // string result = string.Empty;
            string resource = GetParam(path);

            string html = string.Empty;
            using (StreamReader sr = new StreamReader(m_pathToFile))
                html = sr.ReadToEnd();

            httpResponse.ContentType = "text/html";
            return Encoding.UTF8.GetBytes(html);
        }

        #endregion Protected Methods
    }

    public class FormUploadPostHandler : BaseStreamHandler
    {
        #region Private Fields

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string m_PathToFile;

        #endregion Private Fields

        #region Public Constructors

        public FormUploadPostHandler(string path) :
            base("POST", "/diva/WhoGotWhat")
        {
            m_PathToFile = path;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override byte[] ProcessRequest(string path, Stream requestData,
                IOSHttpRequest httpRequest, IOSHttpResponse httpResponse)
        {
            // It's a POST, so we need to read the data on the stream, the lines after the blank line
            StreamReader sr = new StreamReader(requestData);
            string body = sr.ReadToEnd();
            sr.Close();
            body = body.Trim();
            string theirpassword;

            // Here the data on the stream is transformed into a nice dictionary of keys & values
            Dictionary<string, object> postdata = ServerUtils.ParseQueryString(body);

            string avatarname = string.Empty, regionname = string.Empty, primname = string.Empty;
            if (postdata.ContainsKey("name") && !string.IsNullOrEmpty(postdata["name"].ToString()))
                avatarname = postdata["name"].ToString();
            if (postdata.ContainsKey("region") && !string.IsNullOrEmpty(postdata["region"].ToString()))
                regionname = postdata["region"].ToString();
            if (postdata.ContainsKey("primname") && !string.IsNullOrEmpty(postdata["primname"].ToString()))
                primname = postdata["primname"].ToString();
            if (postdata.ContainsKey("password") && !string.IsNullOrEmpty(postdata["password"].ToString()))
                theirpassword = postdata["password"].ToString();

            AddToCSVFile(avatarname, regionname, primname);

            string result = "Recorded";
            httpResponse.ContentType = "text/html";
            httpResponse.StatusCode = 200;

            return Encoding.UTF8.GetBytes(result);
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddToCSVFile(string avatar, string region, string primname)
        {
            string d = DateTime.Now.ToString("G");
            using (var tr = new StreamWriter(m_PathToFile, true))
            using (var writer = new CsvWriter(tr))
            {
                writer.WriteField(d);
                writer.WriteField(avatar);
                writer.WriteField(region);
                writer.WriteField(primname);
                writer.NextRecord();
            }
        }

        #endregion Private Methods
    }

    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule", Id = "WhoGotWhatModule")]
    public class WhoGotWhatModule : ISharedRegionModule
    {
        #region Class and Instance Members

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool m_Enabled;
        private string m_Password;

        private static string AssemblyDirectory
        {
            get
            {
                string location = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(location);
            }
        }

        #endregion Class and Instance Members

        #region ISharedRegionModule

        public string Name
        {
            get { return "WhoGotWhat"; }
        }

        public Type ReplaceableInterface
        {
            get { return null; }
        }

        public void AddRegion(Scene scene)
        {
        }

        public void Close()
        {
        }

        public void Initialise(IConfigSource config)
        {
            // We only load the configuration file if the main config doesn't know about this module already
            IConfig cnf = config.Configs["WhoGotWhat"];
            if (cnf == null)
            {
                LoadConfiguration(config);
                cnf = config.Configs["WhoGotWhat"];
                if (cnf == null)
                {
                    m_log.WarnFormat("[WhoGotWhat]: Configuration section WhoGotWhat not found. Unable to proceed.");
                    return;
                }
            }

            m_Enabled = cnf.GetBoolean("Enabled", m_Enabled);
            m_Password = cnf.GetString("MachineID", m_Password);

            if (m_Enabled)
            {
                string pathToCsvFile = Path.Combine(AssemblyDirectory, "WhoGotWhat.csv");
                string pathToHtmlFile = Path.Combine(AssemblyDirectory, "WhoGotWhat.html");
                m_log.InfoFormat("[WhoGotWhat]: WhoGotWhat is on. File is {0}", pathToCsvFile);

                if (!File.Exists(pathToCsvFile))
                {
                    m_log.WarnFormat("[WhoGotWhat]: Creating WhoGotWhat.csv");
                    using (var tr = new StreamWriter(pathToCsvFile))
                    using (var writer = new CsvWriter(tr))
                    {
                        writer.WriteField("Date");
                        writer.WriteField("Name");
                        writer.WriteField("Region");
                        writer.WriteField("ProductName");
                        writer.NextRecord();
                    }
                }

                MainServer.Instance.AddStreamHandler(new FormUploadGetHandler(pathToHtmlFile));
                MainServer.Instance.AddStreamHandler(new FormUploadPostHandler(pathToCsvFile));
            }
        }

        public void PostInitialise()
        {
        }

        public void RegionLoaded(Scene scene)
        {
        }

        public void RemoveRegion(Scene scene)
        {
        }

        #endregion ISharedRegionModule

        #region Private Methods

        private void LoadConfiguration(IConfigSource config)
        {
            string configPath;
            bool created;
            if (!Util.MergeConfigurationFile(config, "WhoGotWhat.ini", Path.Combine(AssemblyDirectory, "WhoGotWhat.ini"), out configPath, out created))
            {
                m_log.WarnFormat("[WhoGotWhat]: Configuration file not merged");
                return;
            }

            if (created)
            {
                m_log.ErrorFormat("[WhoGotWhat]: PLEASE EDIT {0} BEFORE RUNNING THIS ADDIN", configPath);
                throw new Exception("Addin must be configured prior to running");
            }
        }

        #endregion Private Methods
    }
}
